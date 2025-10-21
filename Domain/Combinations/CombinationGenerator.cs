using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

using Domain.Enums;
using Domain.Helpers;
using Domain.Main;

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
            CombinationFilterStrategy filterStrategy = CombinationFilterStrategy.Comprehensive,
            HashSet<int> tieredItemIds = null,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            // ✅ Pre-filter for Strict mode (most efficient!)
            if (filterStrategy == CombinationFilterStrategy.Strict && tieredItemIds != null)
            {
                listOfItemClassesWithoutRings = [.. listOfItemClassesWithoutRings
                    .Select(list => list.Where(item =>
                    {
                        if (item is Item itm)
                            return tieredItemIds.Contains(itm.Id);
                        return true;
                    }).ToList())];

                if (listOfRings != null)
                {
                    listOfRings = [.. listOfRings.Where(item =>
                    {
                        if (item is Item itm)
                            return tieredItemIds.Contains(itm.Id);
                        return true;
                    })];
                }
            }

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

                                // ✅ Balanced mode check
                                if (filterStrategy == CombinationFilterStrategy.Balanced && tieredItemIds != null)
                                {
                                    bool hasAnyTieredItem = fullCombination.Any(item =>
                                    {
                                        if (item is Item itm) return tieredItemIds.Contains(itm.Id);
                                        return true;
                                    });

                                    if (!hasAnyTieredItem)
                                        continue; // Skip this combination
                                }

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

                                // ✅ Balanced mode check
                                if (filterStrategy == CombinationFilterStrategy.Balanced && tieredItemIds != null)
                                {
                                    bool hasAnyTieredItem = singleItemList.Any(i =>
                                    {
                                        if (i is Item itm)
                                            return tieredItemIds.Contains(itm.Id);
                                        return true;
                                    });

                                    if (!hasAnyTieredItem)
                                        return; // Skip
                                }

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

                                    // ✅ Balanced mode check
                                    if (filterStrategy == CombinationFilterStrategy.Balanced && tieredItemIds != null)
                                    {
                                        bool hasAnyTieredItem = fullCombination.Any(i =>
                                        {
                                            if (i is Item itm)
                                                return tieredItemIds.Contains(itm.Id);
                                            return true;
                                        });

                                        if (!hasAnyTieredItem)
                                            continue; // Skip
                                    }

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
}
