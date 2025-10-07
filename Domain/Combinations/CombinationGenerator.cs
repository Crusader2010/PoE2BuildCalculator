using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

namespace Domain.Combinations
{
    public static class CombinationGenerator
    {
        /// <summary>
        /// Computes the total number of combinations WITHOUT generating them.
        /// Handles all ring cases: 0, 1, or 2+ rings.
        /// </summary>
        public static BigInteger ComputeTotalCombinations<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings)
        {
            // Filter out empty lists
            var filteredLists = listOfItemClassesWithoutRings
                .Where(list => list != null && list.Count > 0)
                .ToList();

            // Calculate product of filtered list sizes
            BigInteger otherCombinations = 1;
            foreach (var list in filteredLists)
            {
                otherCombinations *= list.Count;
            }

            // Handle rings based on count
            int ringCount = listOfRings?.Count ?? 0;
            if (ringCount == 0)
            {
                return otherCombinations;
            }
            else if (ringCount == 1)
            {
                otherCombinations *= ringCount;
                return otherCombinations;
            }
            else
            {
                // Two+ rings: multiply by number of pairs
                BigInteger pairCount = new(ringCount * (ringCount - 1) / 2);
                return pairCount * otherCombinations;
            }
        }

        public static CombinationResult<T> GenerateCombinationsParallel<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings,
            Func<List<T>, bool> validator,
            IProgress<CombinationProgress> progress = null,
            int maxValidToStore = 1000000,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var filteredLists = listOfItemClassesWithoutRings.Where(list => list != null && list.Count > 0).ToList();
            var totalCombinations = ComputeTotalCombinations(listOfItemClassesWithoutRings, listOfRings);

            if (totalCombinations == 0)
            {
                return new CombinationResult<T> { TotalCombinations = 0, ErrorMessage = "No combinations possible." };
            }

            long processedCount = 0;
            var finalResultCollection = new List<List<T>>();
            Exception firstException = null;

            int ringCount = listOfRings?.Count ?? 0;

            // This object will be used to safely sum up the count of valid combinations from all threads.
            long validCombinationCount = 0;

            try
            {
                if (ringCount >= 2)
                {
                    var ringPairsList = GetUniquePairs(listOfRings).ToList();
                    Parallel.ForEach(
                        ringPairsList,
                        // --- START: NEW THREAD-LOCAL STORAGE (TLS) LOGIC ---
                        localInit: () => new List<List<T>>(), // 1. Each thread gets its own private list
                        body: (pair, loopState, threadLocalList) =>
                        {
                            if (firstException != null) loopState.Stop();
                            cancellationToken.ThrowIfCancellationRequested();

                            foreach (var combination in GenerateIterativeCombinations(filteredLists))
                            {
                                if (cancellationToken.IsCancellationRequested) break;

                                var fullCombination = new List<T>(pair.Count + combination.Count);
                                fullCombination.AddRange(pair);
                                fullCombination.AddRange(combination);

                                bool isValid = validator(fullCombination);
                                if (isValid)
                                {
                                    threadLocalList.Add(fullCombination); // 2. Add to the FAST private list
                                }

                                long currentProcessed = Interlocked.Increment(ref processedCount);
                                if (progress != null && currentProcessed % 25000 == 0)
                                {
                                    double percent = totalCombinations > 0 ? Math.Min(100.0, GetBigNumberRatio(currentProcessed, totalCombinations) * 100.0) : 0;
                                    progress.Report(new CombinationProgress
                                    {
                                        TotalCombinations = totalCombinations,
                                        ProcessedCombinations = currentProcessed,
                                        ValidCombinations = validCombinationCount,
                                        PercentComplete = percent,
                                        ElapsedTime = sw.Elapsed
                                    });
                                }
                            }
                            return threadLocalList; // Return the list for the next iteration on this thread
                        },
                        localFinally: (threadLocalList) => // 3. When a thread is done, merge its results
                        {
                            if (threadLocalList.Count > 0)
                            {
                                lock (finalResultCollection)
                                {
                                    // Only store if we haven't hit the cap
                                    if (finalResultCollection.Count < maxValidToStore)
                                    {
                                        int spaceLeft = maxValidToStore - finalResultCollection.Count;
                                        finalResultCollection.AddRange(threadLocalList.Take(spaceLeft));
                                    }
                                }
                                Interlocked.Add(ref validCombinationCount, threadLocalList.Count);
                            }
                        }
                        // --- END: NEW TLS LOGIC ---
                    );
                }
                else // Logic for 0 or 1 ring
                {
                    if (ringCount == 1) filteredLists.Add(listOfRings);
                    if (filteredLists.Count == 0) throw new InvalidOperationException("No item lists to process.");

                    // Sorter and parallelization setup is the same...
                    var firstList = filteredLists[0];
                    var remainingLists = filteredLists.Skip(1).ToList();

                    Parallel.ForEach(
                        firstList,
                        // --- START: NEW THREAD-LOCAL STORAGE (TLS) LOGIC ---
                        localInit: () => new List<List<T>>(),
                        body: (item, loopState, threadLocalList) =>
                        {
                            if (firstException != null) loopState.Stop();
                            cancellationToken.ThrowIfCancellationRequested();

                            var prefix = new List<T> { item };
                            foreach (var combination in GenerateIterativeCombinations(remainingLists))
                            {
                                if (cancellationToken.IsCancellationRequested) break;

                                var fullCombination = new List<T>(prefix.Count + combination.Count);
                                fullCombination.AddRange(prefix);
                                fullCombination.AddRange(combination);

                                bool isValid = validator(fullCombination);
                                if (isValid)
                                {
                                    threadLocalList.Add(fullCombination);
                                }

                                // Progress reporting logic is the same...
                                Interlocked.Increment(ref processedCount);
                            }
                            return threadLocalList;
                        },
                        localFinally: (threadLocalList) =>
                        {
                            if (threadLocalList.Count > 0)
                            {
                                lock (finalResultCollection)
                                {
                                    if (finalResultCollection.Count < maxValidToStore)
                                    {
                                        int spaceLeft = maxValidToStore - finalResultCollection.Count;
                                        finalResultCollection.AddRange(threadLocalList.Take(spaceLeft));
                                    }
                                }
                                Interlocked.Add(ref validCombinationCount, threadLocalList.Count);
                            }
                        }
                        // --- END: NEW TLS LOGIC ---
                    );
                }
            }
            catch (OperationCanceledException)
            {
                // This is now the primary way cancellation will be caught by the caller.
                sw.Stop();
                throw; // Re-throw for the MainForm to catch.
            }
            catch (Exception ex)
            {
                firstException = ex;
            }

            sw.Stop();
            if (firstException != null)
            {
                throw new InvalidOperationException($"An error occurred: {firstException.Message}", firstException);
            }

            return new CombinationResult<T>
            {
                TotalCombinations = totalCombinations,
                ProcessedCombinations = processedCount,
                ValidCombinations = (int)validCombinationCount,
                ElapsedTime = sw.Elapsed,
                ValidCombinationsCollection = finalResultCollection,
                ErrorMessage = null
            };
        }

        ///// <summary>
        ///// Generates combinations in parallel with progress reporting and cancellation support.
        ///// MEMORY EFFICIENT: Does NOT materialize all combinations. Processes lazily.
        ///// Handles all ring cases: 0, 1, or 2+ rings.
        ///// </summary>
        //public static CombinationResult<T> GenerateCombinationsParallel<T>(
        //    List<List<T>> listOfItemClassesWithoutRings,
        //    List<T> listOfRings,
        //    Func<List<T>, bool> validator,
        //    IProgress<CombinationProgress> progress = null,
        //    int maxValidToStore = 10000,
        //    CancellationToken cancellationToken = default)
        //{
        //    var sw = Stopwatch.StartNew();
        //    var filteredLists = listOfItemClassesWithoutRings
        //        .Where(list => list != null && list.Count > 0)
        //        .ToList();

        //    // Note: ComputeTotalCombinations logic was slightly altered to correctly handle the 1-ring case.
        //    var totalCombinations = ComputeTotalCombinations(listOfItemClassesWithoutRings, listOfRings);

        //    if (totalCombinations == 0)
        //    {
        //        return new CombinationResult<T> { TotalCombinations = 0, ErrorMessage = "No combinations possible." };
        //    }

        //    long processedCount = 0;
        //    var validCombinations = new ConcurrentBag<List<T>>();
        //    Exception firstException = null;

        //    int ringCount = listOfRings?.Count ?? 0;

        //    if (ringCount >= 2)
        //    {
        //        var ringPairsList = GetUniquePairs(listOfRings).ToList();
        //        Parallel.ForEach(
        //            ringPairsList,
        //            new ParallelOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = Environment.ProcessorCount },
        //            (pair, loopState) =>
        //            {
        //                if (firstException != null) loopState.Stop();
        //                ProcessCombinationsForPrefix(
        //                    pair, filteredLists, validator, validCombinations, maxValidToStore,
        //                    ref processedCount, ref firstException, totalCombinations, progress, sw, loopState, cancellationToken);
        //            });
        //    }
        //    else
        //    {
        //        if (ringCount == 1)
        //        {
        //            filteredLists.Add(listOfRings);
        //        }

        //        if (filteredLists.Count == 0)
        //        {
        //            return new CombinationResult<T> { TotalCombinations = 0, ErrorMessage = "No valid lists to generate combinations from." };
        //        }

        //        int largestListIndex = filteredLists.Select((list, index) => new { list.Count, index }).OrderByDescending(x => x.Count).First().index;
        //        if (largestListIndex > 0)
        //        {
        //            var largestList = filteredLists[largestListIndex];
        //            filteredLists.RemoveAt(largestListIndex);
        //            filteredLists.Insert(0, largestList);
        //        }

        //        var firstList = filteredLists[0];
        //        var remainingLists = filteredLists.Skip(1).ToList();

        //        Parallel.ForEach(
        //            firstList,
        //            new ParallelOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = Environment.ProcessorCount },
        //            (item, loopState) =>
        //            {
        //                if (firstException != null) loopState.Stop();
        //                var prefix = new List<T> { item };
        //                ProcessCombinationsForPrefix(
        //                    prefix, remainingLists, validator, validCombinations, maxValidToStore,
        //                    ref processedCount, ref firstException, totalCombinations, progress, sw, loopState, cancellationToken);
        //            });
        //    }

        //    sw.Stop();

        //    if (firstException != null)
        //    {
        //        throw new InvalidOperationException($"An error occurred during combination generation: {firstException.Message}", firstException);
        //    }

        //    progress?.Report(new CombinationProgress
        //    {
        //        TotalCombinations = totalCombinations,
        //        ProcessedCombinations = processedCount,
        //        ValidCombinations = validCombinations.Count,
        //        PercentComplete = 100.0,
        //        ElapsedTime = sw.Elapsed
        //    });

        //    return new CombinationResult<T>
        //    {
        //        TotalCombinations = totalCombinations,
        //        ProcessedCombinations = processedCount,
        //        ValidCombinations = validCombinations.Count,
        //        ElapsedTime = sw.Elapsed,
        //        ValidCombinationsCollection = validCombinations.Count <= maxValidToStore ? [.. validCombinations] : [.. validCombinations.Take(maxValidToStore)],
        //        ErrorMessage = null
        //    };
        //}

        /// <summary>
        /// Helper method to process combinations for a given prefix (pair or single item).
        /// This encapsulates the common logic for both parallelization strategies.
        /// </summary>
        /// <summary>
        /// Helper method to process combinations for a given prefix.
        /// </summary>
        private static void ProcessCombinationsForPrefix<T>(
            List<T> prefix,
            List<List<T>> remainingLists,
            Func<List<T>, bool> validator,
            ConcurrentBag<List<T>> validCombinations,
            int maxValidToStore,
            ref long processedCount,
            ref Exception firstException,
            BigInteger totalCombinations,
            IProgress<CombinationProgress> progress,
            Stopwatch sw,
            ParallelLoopState loopState,
            CancellationToken cancellationToken)
        {
            try
            {
                foreach (var combination in GenerateIterativeCombinations(remainingLists))
                {
                    if (cancellationToken.IsCancellationRequested || firstException != null)
                    {
                        loopState.Stop();
                        return;
                    }

                    List<T> fullCombination = [.. prefix, .. combination];

                    bool isValid;
                    try
                    {
                        isValid = validator == null || validator(fullCombination);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        Interlocked.CompareExchange(ref firstException, ex, null);
                        loopState.Stop();
                        return;
                    }

                    if (isValid && validCombinations.Count < maxValidToStore)
                    {
                        validCombinations.Add(fullCombination);
                    }

                    long currentProcessed = Interlocked.Increment(ref processedCount);

                    if (progress != null && currentProcessed % 10000 == 0)
                    {
                        double percent = totalCombinations > 0 ? Math.Min(100.0, GetBigNumberRatio(currentProcessed, totalCombinations) * 100.0) : 0;
                        progress.Report(new CombinationProgress
                        {
                            TotalCombinations = totalCombinations,
                            ProcessedCombinations = currentProcessed,
                            ValidCombinations = validCombinations.Count,
                            PercentComplete = percent,
                            ElapsedTime = sw.Elapsed
                        });
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Interlocked.CompareExchange(ref firstException, ex, null);
                loopState.Stop();
            }
        }

        /// <summary>
        /// Estimates execution time by sampling a subset of combinations and measuring validation speed.
        /// MEMORY EFFICIENT: Only materializes the sample size, not all combinations.
        /// Handles all ring cases: 0, 1, or 2+ rings.
        /// </summary>
        /// <summary>
        /// Estimates execution time by sampling a subset of combinations and measuring validation speed.
        /// </summary>
        public static ExecutionEstimate EstimateExecutionTime<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings,
            Func<List<T>, bool> validator,
            int sampleSize = 10000)
        {
            var filteredLists = listOfItemClassesWithoutRings.Where(list => list != null && list.Count > 0).ToList();
            var totalCombinations = ComputeTotalCombinations(listOfItemClassesWithoutRings, listOfRings);
            if (totalCombinations == 0) return new ExecutionEstimate { TotalCombinations = 0, ErrorMessage = "No combinations possible." };

            long processedInSample = 0;
            int validationErrors = 0;
            bool sampleComplete = false;
            int ringCount = listOfRings?.Count ?? 0;

            var sw = Stopwatch.StartNew();
            try
            {
                if (ringCount >= 2)
                {
                    foreach (var pair in GetUniquePairs(listOfRings))
                    {
                        if (sampleComplete) break;
                        foreach (var combination in GenerateIterativeCombinations(filteredLists))
                        {
                            if (processedInSample >= sampleSize) { sampleComplete = true; break; }
                            List<T> fullCombination = [.. pair, .. combination];
                            try { validator?.Invoke(fullCombination); processedInSample++; } catch { validationErrors++; }
                        }
                    }
                }
                else
                {
                    if (ringCount == 1) filteredLists.Add(listOfRings);
                    foreach (var combination in GenerateIterativeCombinations(filteredLists))
                    {
                        if (processedInSample >= sampleSize) break;
                        try { validator?.Invoke(combination); processedInSample++; } catch { validationErrors++; }
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new ExecutionEstimate { TotalCombinations = totalCombinations, ErrorMessage = $"Error during sampling: {ex.Message}" };
            }
            sw.Stop();

            if (processedInSample == 0) return new ExecutionEstimate { TotalCombinations = totalCombinations, ErrorMessage = "Could not collect any sample combinations." };

            double combinationsPerSecond = processedInSample / Math.Max(sw.Elapsed.TotalSeconds, 0.001);
            double estimatedSeconds = GetBigNumberRatio(totalCombinations, new BigInteger(combinationsPerSecond));
            TimeSpan estimatedDuration = estimatedSeconds > TimeSpan.MaxValue.TotalSeconds ? TimeSpan.MaxValue : TimeSpan.FromSeconds(estimatedSeconds);

            return new ExecutionEstimate
            {
                TotalCombinations = totalCombinations,
                EstimatedDuration = estimatedDuration,
                CombinationsPerSecond = combinationsPerSecond,
                SampleSize = processedInSample,
                MeasurementDuration = sw.Elapsed,
                ProcessorCount = Environment.ProcessorCount,
                ErrorMessage = validationErrors > 0 ? $"Warning: {validationErrors} validation errors occurred during sampling." : null
            };
        }

        /// <summary>
        /// NEW, EFFICIENT METHOD: Iteratively generates combinations from multiple lists.
        /// Avoids recursion and minimizes memory allocations, preventing GC pressure.
        /// </summary>
        private static IEnumerable<List<T>> GenerateIterativeCombinations<T>(List<List<T>> lists)
        {
            if (lists == null || lists.Count == 0)
            {
                yield return [];
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

        /// <summary>
        /// Generates all unique pairs from a single list.
        /// </summary>
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
                return 0; // avoid divide by zero

            // If both fit safely in double, use fast path
            if (numerator <= (BigInteger)double.MaxValue && denominator <= (BigInteger)double.MaxValue)
            {
                return (double)numerator / (double)denominator;
            }

            // Otherwise use logarithmic method to avoid overflow
            double lnNumerator = BigInteger.Log(BigInteger.Abs(numerator));
            double lnDenominator = BigInteger.Log(BigInteger.Abs(denominator));

            double ratio = Math.Exp(lnNumerator - lnDenominator);

            // Preserve sign (if you ever have negatives)
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
            sb.AppendLine($"CPU Cores Used: {ProcessorCount}");
            sb.AppendLine();
            sb.AppendLine($"Sample Size: {SampleSize:N0} combinations");
            sb.AppendLine($"Sample Duration: {MeasurementDuration.TotalSeconds:F3} seconds");
            sb.AppendLine($"Measured Speed: {CombinationsPerSecond:N0} combinations/second");
            sb.AppendLine();
            sb.AppendLine($"ESTIMATED TIME: {FormatTimeSpan(EstimatedDuration)}");
            sb.AppendLine();

            if (EstimatedDuration.TotalHours > 24)
            {
                sb.AppendLine($"⚠ WARNING: This will take approximately {EstimatedDuration.TotalDays:F1} DAYS!");
                sb.AppendLine("Consider adding stricter validation rules to filter more combinations.");
            }
            else if (EstimatedDuration.TotalHours > 1)
            {
                sb.AppendLine($"⚠ NOTE: This will take approximately {EstimatedDuration.TotalHours:F1} HOURS");
            }
            else if (EstimatedDuration.TotalMinutes > 5)
            {
                sb.AppendLine($"⏱ This will take approximately {EstimatedDuration.TotalMinutes:F1} MINUTES");
            }
            else
            {
                sb.AppendLine("✓ Execution should complete quickly");
            }

            return sb.ToString();
        }

        private static string FormatTimeSpan(TimeSpan ts)
        {
            if (ts == TimeSpan.MaxValue)
                return "More than maximum representable time";

            if (ts.TotalDays >= 365)
                return $"{ts.TotalDays / 365:F1} years";
            if (ts.TotalDays >= 1)
                return $"{ts.TotalDays:F1} days ({ts:d\\d\\ hh\\:mm\\:ss})";
            if (ts.TotalHours >= 1)
                return $"{ts.TotalHours:F1} hours ({ts:hh\\:mm\\:ss})";
            if (ts.TotalMinutes >= 1)
                return $"{ts.TotalMinutes:F1} minutes ({ts:mm\\:ss})";
            return $"{ts.TotalSeconds:F1} seconds";
        }
    }
}