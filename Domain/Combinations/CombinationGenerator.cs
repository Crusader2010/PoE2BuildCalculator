using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

namespace Domain.Combinations
{
    public static class CombinationGenerator
    {
        private static readonly ConcurrentDictionary<int, BigInteger> _totalCombinationsCache = new();

        public static BigInteger ComputeTotalCombinations<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings)
        {
            int cacheKey = GetCacheKey(listOfItemClassesWithoutRings, listOfRings);

            if (_totalCombinationsCache.TryGetValue(cacheKey, out BigInteger cached))
            {
                return cached;
            }

            var filteredLists = listOfItemClassesWithoutRings
                .Where(list => list != null && list.Count > 0)
                .ToList();

            BigInteger otherCombinations = 1;
            foreach (var list in filteredLists)
            {
                otherCombinations *= list.Count;
            }

            int ringCount = listOfRings?.Count ?? 0;
            BigInteger result;

            if (ringCount == 0)
            {
                result = otherCombinations;
            }
            else if (ringCount == 1)
            {
                result = otherCombinations * 1;
            }
            else
            {
                BigInteger pairCount = new BigInteger(ringCount) * (ringCount - 1) / 2;
                result = pairCount * otherCombinations;
            }

            _totalCombinationsCache.TryAdd(cacheKey, result);
            return result;
        }

        private static int GetCacheKey<T>(List<List<T>> lists, List<T> rings)
        {
            int hash = 17;
            foreach (var list in lists.Where(l => l != null && l.Count > 0))
            {
                hash = hash * 31 + list.Count;
            }
            hash = hash * 31 + (rings?.Count ?? 0);
            return hash;
        }

        /// <summary>
        /// MAIN GENERATION METHOD - Optimized with batching
        /// </summary>
        public static CombinationResult<T> GenerateCombinationsParallel<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings,
            Func<List<T>, bool> validator,
            IProgress<CombinationProgress> progress = null,
            int maxValidToStore = 1000000,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var filteredLists = listOfItemClassesWithoutRings
                .Where(list => list != null && list.Count > 0)
                .ToList();

            var totalCombinations = ComputeTotalCombinations(listOfItemClassesWithoutRings, listOfRings);

            if (totalCombinations == 0)
            {
                return new CombinationResult<T>
                {
                    TotalCombinations = 0,
                    ErrorMessage = "No combinations possible."
                };
            }

            long processedCount = 0;
            long validCombinationCount = 0;
            var finalResultCollection = new ConcurrentBag<List<T>>();
            bool cancelled = false;

            int ringCount = listOfRings?.Count ?? 0;

            // Progress throttling
            long lastReportedCount = 0;
            var progressLock = new object();

            try
            {
                if (ringCount >= 2)
                {
                    // Parallelize over ring pairs
                    var ringPairsList = GetUniquePairs(listOfRings).ToList();

                    Parallel.ForEach(
                        ringPairsList,
                        new ParallelOptions
                        {
                            CancellationToken = cancellationToken,
                            MaxDegreeOfParallelism = Environment.ProcessorCount
                        },
                        (pair) =>
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                cancelled = true;
                                return;
                            }

                            foreach (var combination in GenerateIterativeCombinations(filteredLists))
                            {
                                if (cancelled || cancellationToken.IsCancellationRequested)
                                {
                                    cancelled = true;
                                    return;
                                }

                                var fullCombination = new List<T>(pair.Count + combination.Count);
                                fullCombination.AddRange(pair);
                                fullCombination.AddRange(combination);

                                bool isValid = validator(fullCombination);

                                if (isValid)
                                {
                                    if (Interlocked.Read(ref validCombinationCount) < maxValidToStore)
                                    {
                                        finalResultCollection.Add(fullCombination);
                                    }
                                    Interlocked.Increment(ref validCombinationCount);
                                }

                                long currentProcessed = Interlocked.Increment(ref processedCount);

                                // Report every 100K for performance
                                if (progress != null && currentProcessed - lastReportedCount >= 100000)
                                {
                                    lock (progressLock)
                                    {
                                        if (currentProcessed - lastReportedCount >= 100000)
                                        {
                                            lastReportedCount = currentProcessed;
                                            ReportProgress(progress, currentProcessed,
                                                validCombinationCount, totalCombinations, sw.Elapsed);
                                        }
                                    }
                                }
                            }
                        });
                }
                else
                {
                    // Build all lists (including single ring if exists)
                    var allLists = new List<List<T>>(filteredLists);
                    if (ringCount == 1)
                    {
                        allLists.Add(listOfRings);
                    }

                    if (allLists.Count == 0)
                    {
                        throw new InvalidOperationException("No item lists to process.");
                    }

                    // CRITICAL: Sort lists by size descending for better parallelization
                    allLists = [.. allLists.OrderByDescending(l => l.Count)];

                    // Find LARGEST list for parallelization
                    int largestListIndex = 0; // Already sorted, so first is largest
                    var parallelizationSource = allLists[largestListIndex];
                    var remainingLists = allLists.Skip(1).ToList();

                    // Handle single list case
                    if (allLists.Count == 1)
                    {
                        // Single list - still parallelize but just yield items
                        Parallel.ForEach(
                            parallelizationSource,
                            new ParallelOptions
                            {
                                CancellationToken = cancellationToken,
                                MaxDegreeOfParallelism = Environment.ProcessorCount
                            },
                            (item) =>
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    cancelled = true;
                                    return;
                                }

                                var singleItemList = new List<T> { item };
                                bool isValid = validator(singleItemList);

                                if (isValid)
                                {
                                    if (Interlocked.Read(ref validCombinationCount) < maxValidToStore)
                                    {
                                        finalResultCollection.Add(singleItemList);
                                    }
                                    Interlocked.Increment(ref validCombinationCount);
                                }

                                long currentProcessed = Interlocked.Increment(ref processedCount);

                                if (progress != null && currentProcessed - lastReportedCount >= 100000)
                                {
                                    lock (progressLock)
                                    {
                                        if (currentProcessed - lastReportedCount >= 100000)
                                        {
                                            lastReportedCount = currentProcessed;
                                            ReportProgress(progress, currentProcessed,
                                                validCombinationCount, totalCombinations, sw.Elapsed);
                                        }
                                    }
                                }
                            });
                    }
                    else
                    {
                        // Multiple lists - normal processing
                        Parallel.ForEach(
                            parallelizationSource,
                            new ParallelOptions
                            {
                                CancellationToken = cancellationToken,
                                MaxDegreeOfParallelism = Environment.ProcessorCount
                            },
                            (item) =>
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    cancelled = true;
                                    return;
                                }

                                var prefix = new List<T> { item };

                                foreach (var combination in GenerateIterativeCombinations(remainingLists))
                                {
                                    if (cancelled || cancellationToken.IsCancellationRequested)
                                    {
                                        cancelled = true;
                                        return;
                                    }

                                    var fullCombination = new List<T>(prefix.Count + combination.Count);
                                    fullCombination.AddRange(prefix);
                                    fullCombination.AddRange(combination);

                                    bool isValid = validator(fullCombination);

                                    if (isValid)
                                    {
                                        if (Interlocked.Read(ref validCombinationCount) < maxValidToStore)
                                        {
                                            finalResultCollection.Add(fullCombination);
                                        }
                                        Interlocked.Increment(ref validCombinationCount);
                                    }

                                    long currentProcessed = Interlocked.Increment(ref processedCount);

                                    if (progress != null && currentProcessed - lastReportedCount >= 100000)
                                    {
                                        lock (progressLock)
                                        {
                                            if (currentProcessed - lastReportedCount >= 100000)
                                            {
                                                lastReportedCount = currentProcessed;
                                                ReportProgress(progress, currentProcessed,
                                                    validCombinationCount, totalCombinations, sw.Elapsed);
                                            }
                                        }
                                    }
                                }
                            });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
            }

            sw.Stop();

            if (progress != null)
            {
                ReportProgress(progress, processedCount, validCombinationCount,
                             totalCombinations, sw.Elapsed, isFinal: true);
            }

            if (cancelled)
            {
                throw new OperationCanceledException("Combination generation was cancelled.");
            }

            return new CombinationResult<T>
            {
                TotalCombinations = totalCombinations,
                ProcessedCombinations = processedCount,
                ValidCombinations = (int)validCombinationCount,
                ElapsedTime = sw.Elapsed,
                ValidCombinationsCollection = [.. finalResultCollection.Take(maxValidToStore)],
                ErrorMessage = null
            };
        }

        private static void ReportProgress(
            IProgress<CombinationProgress> progress,
            long processedCount,
            long validCount,
            BigInteger totalCombinations,
            TimeSpan elapsed,
            bool isFinal = false)
        {
            long currentValid = Interlocked.Read(ref validCount);

            double percent = totalCombinations > 0
                ? Math.Min(100.0, GetBigNumberRatio(processedCount, totalCombinations) * 100.0)
                : 0;

            if (isFinal)
            {
                percent = 100.0;
            }

            progress.Report(new CombinationProgress
            {
                TotalCombinations = totalCombinations,
                ProcessedCombinations = processedCount,
                ValidCombinations = currentValid,
                PercentComplete = percent,
                ElapsedTime = elapsed
            });
        }

        /// <summary>
        /// BENCHMARK - DETERMINISTIC and mirrors main method exactly
        /// Samples FIRST N combinations using SAME strategy as main method
        /// </summary>
        public static ExecutionEstimate EstimateExecutionTime<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings,
            Func<List<T>, bool> validator,
            int sampleSize = 50000)
        {
            var filteredLists = listOfItemClassesWithoutRings
                .Where(list => list != null && list.Count > 0)
                .ToList();

            var totalCombinations = ComputeTotalCombinations(listOfItemClassesWithoutRings, listOfRings);

            if (totalCombinations == 0)
            {
                return new ExecutionEstimate
                {
                    TotalCombinations = 0,
                    ErrorMessage = "No combinations possible."
                };
            }

            int ringCount = listOfRings?.Count ?? 0;

            // Cap sample size
            int safeSampleSize = Math.Min(sampleSize, 100000);

            // Determine strategy EXACTLY as main method does
            List<List<T>> allListsForBenchmark;
            List<T> parallelizationSource;
            List<List<T>> remainingLists;
            bool useRingPairs;

            if (ringCount >= 2)
            {
                // Strategy A: Use ring pairs (same as main method)
                useRingPairs = true;
                parallelizationSource = GetUniquePairs(listOfRings).Take(Math.Max(1, safeSampleSize / 1000)).SelectMany(pair => pair).Distinct().ToList();

                // For deterministic sampling, we'll iterate over ring pairs
                // But we need to prepare data structures
                allListsForBenchmark = null;
                remainingLists = filteredLists;
            }
            else
            {
                // Strategy B: Use largest list (same as main method)
                useRingPairs = false;

                allListsForBenchmark = new List<List<T>>(filteredLists);
                if (ringCount == 1)
                {
                    allListsForBenchmark.Add(listOfRings);
                }

                if (allListsForBenchmark.Count == 0)
                {
                    return new ExecutionEstimate
                    {
                        TotalCombinations = totalCombinations,
                        ErrorMessage = "No lists to process."
                    };
                }

                // CRITICAL: Sort descending (SAME as main method)
                allListsForBenchmark = allListsForBenchmark.OrderByDescending(l => l.Count).ToList();

                parallelizationSource = allListsForBenchmark[0];
                remainingLists = allListsForBenchmark.Skip(1).ToList();
            }

            // Calculate combinations per parallel item to determine sample count
            BigInteger combinationsPerItem;
            if (useRingPairs)
            {
                // Each ring pair generates: product of all filtered lists
                combinationsPerItem = 1;
                foreach (var list in filteredLists)
                {
                    combinationsPerItem *= list.Count;
                }
            }
            else
            {
                // Each item from largest list generates: product of remaining lists
                combinationsPerItem = 1;
                foreach (var list in remainingLists)
                {
                    combinationsPerItem *= list.Count;
                }

                // Handle single list case
                if (remainingLists.Count == 0)
                {
                    combinationsPerItem = 1;
                }
            }

            // Determine how many parallel items to sample
            int itemsToSample;
            if (combinationsPerItem == 0)
            {
                itemsToSample = 1;
            }
            else if (combinationsPerItem >= safeSampleSize)
            {
                // One item produces enough combinations
                itemsToSample = 1;
            }
            else
            {
                // Need multiple items
                itemsToSample = (int)Math.Min(
                    (long)Math.Ceiling((double)safeSampleSize / (double)combinationsPerItem),
                    useRingPairs ? GetUniquePairs(listOfRings).Count() : parallelizationSource.Count
                );
            }

            itemsToSample = Math.Max(1, Math.Min(itemsToSample, Environment.ProcessorCount * 2));

            // STEP 1: Single-threaded benchmark (deterministic)
            long singleThreadProcessed = 0;
            var swSingle = Stopwatch.StartNew();

            try
            {
                if (useRingPairs)
                {
                    // Process first N ring pairs
                    int pairCount = 0;
                    foreach (var pair in GetUniquePairs(listOfRings))
                    {
                        if (pairCount >= itemsToSample) break;
                        pairCount++;

                        foreach (var combination in GenerateIterativeCombinations(remainingLists))
                        {
                            if (singleThreadProcessed >= safeSampleSize) break;

                            var fullCombination = new List<T>(pair.Count + combination.Count);
                            fullCombination.AddRange(pair);
                            fullCombination.AddRange(combination);

                            try { validator?.Invoke(fullCombination); }
                            catch { }

                            singleThreadProcessed++;
                        }

                        if (singleThreadProcessed >= safeSampleSize) break;
                    }
                }
                else
                {
                    // Process first N items from largest list
                    var itemsToProcess = parallelizationSource.Take(itemsToSample).ToList();

                    if (remainingLists.Count == 0)
                    {
                        // Single list case
                        foreach (var item in itemsToProcess)
                        {
                            var singleList = new List<T> { item };
                            try { validator?.Invoke(singleList); }
                            catch { }
                            singleThreadProcessed++;
                        }
                    }
                    else
                    {
                        // Multiple lists
                        foreach (var item in itemsToProcess)
                        {
                            if (singleThreadProcessed >= safeSampleSize) break;

                            var prefix = new List<T> { item };

                            foreach (var combination in GenerateIterativeCombinations(remainingLists))
                            {
                                if (singleThreadProcessed >= safeSampleSize) break;

                                var fullCombination = new List<T>(prefix.Count + combination.Count);
                                fullCombination.AddRange(prefix);
                                fullCombination.AddRange(combination);

                                try { validator?.Invoke(fullCombination); }
                                catch { }

                                singleThreadProcessed++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                swSingle.Stop();
                return new ExecutionEstimate
                {
                    TotalCombinations = totalCombinations,
                    ErrorMessage = $"Single-thread benchmark error: {ex.Message}"
                };
            }

            swSingle.Stop();

            if (singleThreadProcessed == 0)
            {
                return new ExecutionEstimate
                {
                    TotalCombinations = totalCombinations,
                    ErrorMessage = "No samples collected."
                };
            }

            double singleThreadSpeed = singleThreadProcessed / Math.Max(swSingle.Elapsed.TotalSeconds, 0.001);

            // STEP 2: Multi-threaded benchmark (SAME data, deterministic)
            long multiThreadProcessed = 0;
            var swMulti = Stopwatch.StartNew();

            try
            {
                if (useRingPairs)
                {
                    // Process SAME ring pairs in parallel
                    var pairsToProcess = GetUniquePairs(listOfRings).Take(itemsToSample).ToList();

                    Parallel.ForEach(
                        pairsToProcess,
                        new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                        (pair) =>
                        {
                            long localCount = 0;
                            foreach (var combination in GenerateIterativeCombinations(remainingLists))
                            {
                                if (Interlocked.Read(ref multiThreadProcessed) >= safeSampleSize) break;

                                var fullCombination = new List<T>(pair.Count + combination.Count);
                                fullCombination.AddRange(pair);
                                fullCombination.AddRange(combination);

                                try { validator?.Invoke(fullCombination); }
                                catch { }

                                localCount++;
                                Interlocked.Increment(ref multiThreadProcessed);
                            }
                        });
                }
                else
                {
                    // Process SAME items in parallel
                    var itemsToProcess = parallelizationSource.Take(itemsToSample).ToList();

                    if (remainingLists.Count == 0)
                    {
                        // Single list case
                        Parallel.ForEach(
                            itemsToProcess,
                            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                            (item) =>
                            {
                                var singleList = new List<T> { item };
                                try { validator?.Invoke(singleList); }
                                catch { }
                                Interlocked.Increment(ref multiThreadProcessed);
                            });
                    }
                    else
                    {
                        // Multiple lists
                        Parallel.ForEach(
                            itemsToProcess,
                            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                            (item) =>
                            {
                                long localCount = 0;
                                var prefix = new List<T> { item };

                                foreach (var combination in GenerateIterativeCombinations(remainingLists))
                                {
                                    if (Interlocked.Read(ref multiThreadProcessed) >= safeSampleSize) break;

                                    var fullCombination = new List<T>(prefix.Count + combination.Count);
                                    fullCombination.AddRange(prefix);
                                    fullCombination.AddRange(combination);

                                    try { validator?.Invoke(fullCombination); }
                                    catch { }

                                    localCount++;
                                    Interlocked.Increment(ref multiThreadProcessed);
                                }
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                swMulti.Stop();
                return new ExecutionEstimate
                {
                    TotalCombinations = totalCombinations,
                    ErrorMessage = $"Multi-thread benchmark error: {ex.Message}"
                };
            }

            swMulti.Stop();

            if (multiThreadProcessed == 0)
            {
                // Fallback to single-thread
                double estimatedSeconds = GetBigNumberRatio(totalCombinations,
                    new BigInteger((long)singleThreadSpeed));

                return new ExecutionEstimate
                {
                    TotalCombinations = totalCombinations,
                    EstimatedDuration = TimeSpan.FromSeconds(Math.Min(estimatedSeconds, TimeSpan.MaxValue.TotalSeconds)),
                    CombinationsPerSecond = singleThreadSpeed,
                    SampleSize = singleThreadProcessed,
                    MeasurementDuration = swSingle.Elapsed,
                    ProcessorCount = Environment.ProcessorCount,
                    ParallelEfficiency = 0,
                    ErrorMessage = "Using single-thread fallback"
                };
            }

            // Calculate EXACT parallel efficiency
            double multiThreadSpeed = multiThreadProcessed / Math.Max(swMulti.Elapsed.TotalSeconds, 0.001);
            double actualSpeedup = multiThreadSpeed / singleThreadSpeed;
            double parallelEfficiency = actualSpeedup / Environment.ProcessorCount;

            // Use measured multi-thread speed for final estimate
            double finalEstimatedSeconds = GetBigNumberRatio(totalCombinations,
                new BigInteger((long)multiThreadSpeed));

            TimeSpan estimatedDuration = finalEstimatedSeconds > TimeSpan.MaxValue.TotalSeconds
                ? TimeSpan.MaxValue
                : TimeSpan.FromSeconds(finalEstimatedSeconds);

            return new ExecutionEstimate
            {
                TotalCombinations = totalCombinations,
                EstimatedDuration = estimatedDuration,
                CombinationsPerSecond = multiThreadSpeed,
                SampleSize = singleThreadProcessed + multiThreadProcessed,
                MeasurementDuration = swSingle.Elapsed + swMulti.Elapsed,
                ProcessorCount = Environment.ProcessorCount,
                ParallelEfficiency = parallelEfficiency,
                ErrorMessage = null
            };
        }

        private static IEnumerable<List<T>> GenerateIterativeCombinations<T>(List<List<T>> lists)
        {
            if (lists == null || lists.Count == 0)
            {
                yield return new List<T>();
                yield break;
            }

            if (lists.Any(l => l == null || l.Count == 0))
            {
                yield break;
            }

            int numLists = lists.Count;
            var indices = new int[numLists];
            var resultBuffer = new T[numLists];

            while (true)
            {
                for (int i = 0; i < numLists; i++)
                {
                    resultBuffer[i] = lists[i][indices[i]];
                }

                yield return new List<T>(resultBuffer);

                int k = numLists - 1;
                while (k >= 0)
                {
                    indices[k]++;
                    if (indices[k] < lists[k].Count)
                    {
                        break;
                    }
                    indices[k] = 0;
                    k--;
                }

                if (k < 0)
                {
                    yield break;
                }
            }
        }

        private static IEnumerable<List<T>> GetUniquePairs<T>(List<T> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    yield return new List<T> { list[i], list[j] };
                }
            }
        }

        public static double GetBigNumberRatio(BigInteger numerator, BigInteger denominator)
        {
            if (denominator.IsZero)
                return 0;

            if (numerator <= (BigInteger)double.MaxValue && denominator <= (BigInteger)double.MaxValue)
            {
                return (double)numerator / (double)denominator;
            }

            double lnNumerator = BigInteger.Log(BigInteger.Abs(numerator));
            double lnDenominator = BigInteger.Log(BigInteger.Abs(denominator));
            double ratio = Math.Exp(lnNumerator - lnDenominator);

            if (numerator.Sign != denominator.Sign)
                ratio = -ratio;

            return ratio;
        }
    }

    public class CombinationProgress
    {
        public BigInteger TotalCombinations { get; set; }
        public long ProcessedCombinations { get; set; }
        public long ValidCombinations { get; set; }
        public double PercentComplete { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }

    public class CombinationResult<T>
    {
        public BigInteger TotalCombinations { get; set; }
        public long ProcessedCombinations { get; set; }
        public int ValidCombinations { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public List<List<T>> ValidCombinationsCollection { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ExecutionEstimate
    {
        public BigInteger TotalCombinations { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public double CombinationsPerSecond { get; set; }
        public long SampleSize { get; set; }
        public TimeSpan MeasurementDuration { get; set; }
        public int ProcessorCount { get; set; }
        public double ParallelEfficiency { get; set; }
        public string ErrorMessage { get; set; }

        public string GetFormattedSummary()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                return $"ERROR: {ErrorMessage}";
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== EXECUTION TIME ESTIMATE ===");
            sb.AppendLine();
            sb.AppendLine($"Total Combinations: {TotalCombinations:N0}");
            sb.AppendLine($"CPU Cores: {ProcessorCount}");
            sb.AppendLine();
            sb.AppendLine("--- Benchmark Results (Deterministic Sample) ---");
            sb.AppendLine($"Sample Size: {SampleSize:N0} combinations");
            sb.AppendLine($"Measurement Time: {MeasurementDuration.TotalSeconds:F3} seconds");
            sb.AppendLine($"Measured Speed: {CombinationsPerSecond:N0} comb/sec (parallel)");
            sb.AppendLine();
            sb.AppendLine($"Parallel Efficiency: {ParallelEfficiency * 100:F1}%");
            sb.AppendLine($"Actual Speedup: {ParallelEfficiency * ProcessorCount:F2}x over single-thread");
            sb.AppendLine();
            sb.AppendLine($"ESTIMATED TOTAL TIME: {FormatTimeSpan(EstimatedDuration)}");
            sb.AppendLine();

            if (EstimatedDuration.TotalHours > 24)
            {
                sb.AppendLine($"⚠ WARNING: ~{EstimatedDuration.TotalDays:F1} DAYS!");
                sb.AppendLine("Consider stricter validation rules.");
            }
            else if (EstimatedDuration.TotalHours > 1)
            {
                sb.AppendLine($"⚠ NOTE: ~{EstimatedDuration.TotalHours:F1} HOURS");
            }
            else if (EstimatedDuration.TotalMinutes > 5)
            {
                sb.AppendLine($"⏱ ~{EstimatedDuration.TotalMinutes:F1} MINUTES");
            }
            else
            {
                sb.AppendLine("✓ Should complete quickly");
            }

            return sb.ToString();
        }
        private static string FormatTimeSpan(TimeSpan ts)
        {
            if (ts == TimeSpan.MaxValue)
                return "More than max time";

            if (ts.TotalDays >= 365)
                return $"{ts.TotalDays / 365:F1} years";
            if (ts.TotalDays >= 1)
                return $"{ts.TotalDays:F1} days";
            if (ts.TotalHours >= 1)
                return $"{ts.TotalHours:F1} hours";
            if (ts.TotalMinutes >= 1)
                return $"{ts.TotalMinutes:F1} minutes";
            return $"{ts.TotalSeconds:F1} seconds";
        }
    }
}