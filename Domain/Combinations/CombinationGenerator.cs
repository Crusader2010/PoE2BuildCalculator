using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

namespace Domain.Combinations
{
    public static class CombinationGenerator
    {
        // Thread-safe cache for total combinations
        private static readonly ConcurrentDictionary<int, BigInteger> _totalCombinationsCache = new();

        /// <summary>
        /// Computes total combinations with caching
        /// </summary>
        public static BigInteger ComputeTotalCombinations<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings)
        {
            // Create cache key based on list sizes
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
        /// MAIN GENERATION METHOD - FIXED for stability
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
            long validCombinationCount = 0; // Global counter
            var finalResultCollection = new ConcurrentBag<List<T>>(); // Thread-safe collection
            bool cancelled = false;

            int ringCount = listOfRings?.Count ?? 0;

            // Progress reporting throttle
            long lastReportedCount = 0;
            var progressLock = new object();

            try
            {
                if (ringCount >= 2)
                {
                    var ringPairsList = GetUniquePairs(listOfRings).ToList();

                    Parallel.ForEach(
                        ringPairsList,
                        new ParallelOptions
                        {
                            CancellationToken = cancellationToken,
                            MaxDegreeOfParallelism = Environment.ProcessorCount
                        },
                        (pair) => // No TLS - use ConcurrentBag directly
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
                                    // Only store if under limit
                                    if (Interlocked.Read(ref validCombinationCount) < maxValidToStore)
                                    {
                                        finalResultCollection.Add(fullCombination);
                                    }
                                    Interlocked.Increment(ref validCombinationCount);
                                }

                                long currentProcessed = Interlocked.Increment(ref processedCount);

                                // Throttled progress reporting
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
                    var allLists = new List<List<T>>(filteredLists);
                    if (ringCount == 1)
                    {
                        allLists.Add(listOfRings);
                    }

                    if (allLists.Count == 0)
                    {
                        throw new InvalidOperationException("No item lists to process.");
                    }

                    // Find LARGEST list for parallelization
                    int largestListIndex = 0;
                    int maxSize = allLists[0].Count;

                    for (int i = 1; i < allLists.Count; i++)
                    {
                        if (allLists[i].Count > maxSize)
                        {
                            maxSize = allLists[i].Count;
                            largestListIndex = i;
                        }
                    }

                    var parallelizationSource = allLists[largestListIndex];
                    var remainingLists = new List<List<T>>();

                    for (int i = 0; i < allLists.Count; i++)
                    {
                        if (i != largestListIndex)
                        {
                            remainingLists.Add(allLists[i]);
                        }
                    }

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
            catch (OperationCanceledException)
            {
                cancelled = true;
            }

            sw.Stop();

            // Final progress
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
        /// BENCHMARK - FIXED to not freeze computer
        /// </summary>
        public static ExecutionEstimate EstimateExecutionTime<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings,
            Func<List<T>, bool> validator,
            int sampleSize = 10000)
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

            // CRITICAL FIX: Cap sample size to prevent memory issues
            int safeSampleSize = Math.Min(sampleSize, 100000);

            // Measure actual single-threaded performance
            var sw = Stopwatch.StartNew();
            long processedInSample = 0;
            int validCount = 0;

            try
            {
                if (ringCount >= 2)
                {
                    foreach (var pair in GetUniquePairs(listOfRings))
                    {
                        if (processedInSample >= safeSampleSize) break;

                        foreach (var combination in GenerateIterativeCombinations(filteredLists))
                        {
                            // CRITICAL: Check INSIDE inner loop
                            if (processedInSample >= safeSampleSize) break;

                            var fullCombination = new List<T>(pair.Count + combination.Count);
                            fullCombination.AddRange(pair);
                            fullCombination.AddRange(combination);

                            try
                            {
                                bool isValid = validator?.Invoke(fullCombination) ?? true;
                                if (isValid) validCount++;
                            }
                            catch { }

                            processedInSample++;
                        }

                        if (processedInSample >= safeSampleSize) break;
                    }
                }
                else
                {
                    var allLists = new List<List<T>>(filteredLists);
                    if (ringCount == 1)
                    {
                        allLists.Add(listOfRings);
                    }

                    if (allLists.Count > 0)
                    {
                        // Find largest list
                        int largestIdx = 0;
                        int maxSz = allLists[0].Count;
                        for (int i = 1; i < allLists.Count; i++)
                        {
                            if (allLists[i].Count > maxSz)
                            {
                                maxSz = allLists[i].Count;
                                largestIdx = i;
                            }
                        }

                        var source = allLists[largestIdx];
                        var remaining = new List<List<T>>();
                        for (int i = 0; i < allLists.Count; i++)
                        {
                            if (i != largestIdx) remaining.Add(allLists[i]);
                        }

                        foreach (var item in source)
                        {
                            if (processedInSample >= safeSampleSize) break;

                            var prefix = new List<T> { item };

                            foreach (var combination in GenerateIterativeCombinations(remaining))
                            {
                                if (processedInSample >= safeSampleSize) break;

                                var fullCombination = new List<T>(prefix.Count + combination.Count);
                                fullCombination.AddRange(prefix);
                                fullCombination.AddRange(combination);

                                try
                                {
                                    bool isValid = validator?.Invoke(fullCombination) ?? true;
                                    if (isValid) validCount++;
                                }
                                catch { }

                                processedInSample++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new ExecutionEstimate
                {
                    TotalCombinations = totalCombinations,
                    ErrorMessage = $"Error during sampling: {ex.Message}"
                };
            }

            sw.Stop();

            if (processedInSample == 0)
            {
                return new ExecutionEstimate
                {
                    TotalCombinations = totalCombinations,
                    ErrorMessage = "Could not collect any samples."
                };
            }

            // Calculate with realistic parallel efficiency
            double singleThreadSpeed = processedInSample / Math.Max(sw.Elapsed.TotalSeconds, 0.001);

            // Conservative parallel speedup (accounts for overhead, contention, GC)
            double parallelEfficiency = 0.70; // 70% efficiency
            double estimatedParallelSpeed = singleThreadSpeed * Environment.ProcessorCount * parallelEfficiency;

            double estimatedSeconds = GetBigNumberRatio(totalCombinations,
                new BigInteger((long)estimatedParallelSpeed));

            TimeSpan estimatedDuration = estimatedSeconds > TimeSpan.MaxValue.TotalSeconds
                ? TimeSpan.MaxValue
                : TimeSpan.FromSeconds(estimatedSeconds);

            return new ExecutionEstimate
            {
                TotalCombinations = totalCombinations,
                EstimatedDuration = estimatedDuration,
                CombinationsPerSecond = estimatedParallelSpeed,
                SampleSize = processedInSample,
                MeasurementDuration = sw.Elapsed,
                ProcessorCount = Environment.ProcessorCount,
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
            sb.AppendLine($"Parallel Efficiency: 70%");
            sb.AppendLine();
            sb.AppendLine($"Sample Size: {SampleSize:N0} combinations");
            sb.AppendLine($"Sample Duration: {MeasurementDuration.TotalSeconds:F3} seconds");
            sb.AppendLine($"Estimated Speed: {CombinationsPerSecond:N0} comb/sec");
            sb.AppendLine();
            sb.AppendLine($"ESTIMATED TIME: {FormatTimeSpan(EstimatedDuration)}");
            sb.AppendLine();

            if (EstimatedDuration.TotalHours > 24)
            {
                sb.AppendLine($"⚠ WARNING: ~{EstimatedDuration.TotalDays:F1} DAYS!");
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