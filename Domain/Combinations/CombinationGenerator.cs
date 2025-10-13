using Domain.Helpers;

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

            long ringCount = listOfRings?.Count ?? 0;
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
            long maxValidToStore = 10000000,
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

            long ringCount = listOfRings?.Count ?? 0;

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
                ValidCombinations = validCombinationCount,
                ElapsedTime = sw.Elapsed,
                ValidCombinationsCollection = [.. finalResultCollection.TakeLong(maxValidToStore)],
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
        /// BENCHMARK - DETERMINISTIC with proper warmup
        /// </summary>
        public static ExecutionEstimate EstimateExecutionTime<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings,
            Func<List<T>, bool> validator,
            long safeSampleSize,
            bool skipGarbageCollection = false)
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

            long ringCount = listOfRings?.Count ?? 0;

            // Determine strategy EXACTLY as main method
            bool useRingPairs = ringCount >= 2;
            List<List<T>> workingLists;
            List<T> parallelSource;
            List<List<T>> remainingLists;

            if (useRingPairs)
            {
                parallelSource = null; // Will use ring pairs
                remainingLists = filteredLists;
                workingLists = null;
            }
            else
            {
                workingLists = [.. filteredLists];
                if (ringCount == 1) workingLists.Add(listOfRings);

                if (workingLists.Count == 0)
                {
                    return new ExecutionEstimate { TotalCombinations = totalCombinations, ErrorMessage = "No lists." };
                }

                workingLists = [.. workingLists.OrderByDescending(l => l.Count)];
                parallelSource = workingLists[0];
                remainingLists = [.. workingLists.Skip(1)];
            }

            // Determine sample count
            BigInteger combsPerItem = 1;
            if (useRingPairs)
            {
                foreach (var list in filteredLists) combsPerItem *= list.Count;
            }
            else
            {
                foreach (var list in remainingLists) combsPerItem *= list.Count;
                if (remainingLists.Count == 0) combsPerItem = 1;
            }

            long itemsToSample = combsPerItem == 0 ? 1 :
                Math.Min(
                    Math.Max(1, (long)Math.Ceiling(safeSampleSize / (double)combsPerItem)),
                    useRingPairs ? 100 : (parallelSource?.Count ?? 1)
                );

            itemsToSample = Math.Max(2, Math.Min(itemsToSample, Environment.ProcessorCount * 4));

            // CRITICAL: WARMUP PHASE (run 3 times to let JIT compile)
            for (int warmupRound = 0; warmupRound < 3; warmupRound++)
            {
                long warmupCount = 0;
                long warmupItems = Math.Min(itemsToSample / 2, 2);

                if (useRingPairs)
                {
                    foreach (var pair in GetUniquePairs(listOfRings).TakeLong(warmupItems))
                    {
                        foreach (var combination in GenerateIterativeCombinations(remainingLists).Take(100))
                        {
                            var full = new List<T>(pair.Count + combination.Count);
                            full.AddRange(pair);
                            full.AddRange(combination);
                            try { validator?.Invoke(full); } catch { }
                            warmupCount++;
                        }
                    }
                }
                else
                {
                    var warmupSourceItems = parallelSource.TakeLong(warmupItems).ToList();
                    if (remainingLists.Count == 0)
                    {
                        foreach (var item in warmupSourceItems)
                        {
                            var single = new List<T> { item };
                            try { validator?.Invoke(single); } catch { }
                            warmupCount++;
                        }
                    }
                    else
                    {
                        foreach (var item in warmupSourceItems)
                        {
                            var prefix = new List<T> { item };
                            foreach (var combination in GenerateIterativeCombinations(remainingLists).Take(100))
                            {
                                var full = new List<T>(prefix.Count + combination.Count);
                                full.AddRange(prefix);
                                full.AddRange(combination);
                                try { validator?.Invoke(full); } catch { }
                                warmupCount++;
                            }
                        }
                    }
                }
            }


            if (!skipGarbageCollection)
            {
                // Force GC before measurement for clean state
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            // MEASUREMENT: Multi-threaded (this is what we care about)
            long multiThreadProcessed = 0;
            var swMulti = Stopwatch.StartNew();

            try
            {
                if (useRingPairs)
                {
                    var pairsToProcess = GetUniquePairs(listOfRings).TakeLong(itemsToSample).ToList();

                    Parallel.ForEach(
                        pairsToProcess,
                        new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                        (pair) =>
                        {
                            foreach (var combination in GenerateIterativeCombinations(remainingLists))
                            {
                                if (Interlocked.Read(ref multiThreadProcessed) >= safeSampleSize) break;

                                var fullCombination = new List<T>(pair.Count + combination.Count);
                                fullCombination.AddRange(pair);
                                fullCombination.AddRange(combination);

                                try { validator?.Invoke(fullCombination); }
                                catch { }

                                Interlocked.Increment(ref multiThreadProcessed);
                            }
                        });
                }
                else
                {
                    var itemsToProcess = parallelSource.TakeLong(itemsToSample).ToList();

                    if (remainingLists.Count == 0)
                    {
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
                        Parallel.ForEach(
                            itemsToProcess,
                            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                            (item) =>
                            {
                                var prefix = new List<T> { item };

                                foreach (var combination in GenerateIterativeCombinations(remainingLists))
                                {
                                    if (Interlocked.Read(ref multiThreadProcessed) >= safeSampleSize) break;

                                    var fullCombination = new List<T>(prefix.Count + combination.Count);
                                    fullCombination.AddRange(prefix);
                                    fullCombination.AddRange(combination);

                                    try { validator?.Invoke(fullCombination); }
                                    catch { }

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
                    ErrorMessage = $"Benchmark error: {ex.Message}"
                };
            }

            swMulti.Stop();

            if (multiThreadProcessed == 0)
            {
                return new ExecutionEstimate
                {
                    TotalCombinations = totalCombinations,
                    ErrorMessage = "No samples collected."
                };
            }

            // Calculate speed and estimate
            double multiThreadSpeed = multiThreadProcessed / Math.Max(swMulti.Elapsed.TotalSeconds, 0.001);
            double estimatedSeconds = GetBigNumberRatio(totalCombinations, new BigInteger((long)multiThreadSpeed));

            TimeSpan estimatedDuration = estimatedSeconds > TimeSpan.MaxValue.TotalSeconds
                ? TimeSpan.MaxValue
                : TimeSpan.FromSeconds(estimatedSeconds);

            // Calculate efficiency (approximate based on typical parallel overhead)
            double theoreticalMaxSpeed = multiThreadSpeed * 1.25; // Account for 15% overhead
            double parallelEfficiency = multiThreadSpeed / (theoreticalMaxSpeed * Environment.ProcessorCount);

            return new ExecutionEstimate
            {
                TotalCombinations = totalCombinations,
                EstimatedDuration = estimatedDuration,
                CombinationsPerSecond = multiThreadSpeed,
                SampleSize = multiThreadProcessed,
                MeasurementDuration = swMulti.Elapsed,
                ProcessorCount = Environment.ProcessorCount,
                ParallelEfficiency = Math.Min(parallelEfficiency, 1.0),
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
        public long ValidCombinations { get; set; }
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
