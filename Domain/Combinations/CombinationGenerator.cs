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
		/// MAIN GENERATION METHOD - Optimized with batching and reduced allocations
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

			// Calculate max combination size for pre-allocation
			int maxCombinationSize = filteredLists.Count + (ringCount >= 2 ? 2 : (int)ringCount);

			// Progress throttling
			long lastReportedCount = 0;
			var progressLock = new object();

			try
			{
				if (ringCount >= 2)
				{
					// ✅ OPTIMIZED: Parallelize over ring pairs (now returns tuples)
					var ringPairsList = GetUniquePairs(listOfRings).ToList();

					Parallel.ForEach(
						ringPairsList,
						new ParallelOptions
						{
							CancellationToken = cancellationToken,
							MaxDegreeOfParallelism = Environment.ProcessorCount
						},
						() => new List<T>(maxCombinationSize), // ✅ Thread-local reusable list
						(pair, state, reusableList) =>
						{
							if (cancellationToken.IsCancellationRequested)
							{
								cancelled = true;
								return reusableList;
							}

							foreach (var combination in GenerateIterativeCombinations(filteredLists))
							{
								if (cancelled || cancellationToken.IsCancellationRequested)
								{
									cancelled = true;
									return reusableList;
								}

								// ✅ Reuse list instead of allocating new one
								reusableList.Clear();
								reusableList.Add(pair.Item1);
								reusableList.Add(pair.Item2);
								reusableList.AddRange(combination); // combination is now T[] (no allocation)

								// ✅ OPTIMIZED: Balanced mode check with for loop
								if (filterStrategy == CombinationFilterStrategy.Balanced && tieredItemIds != null)
								{
									bool hasAnyTieredItem = false;
									for (int i = 0; i < reusableList.Count; i++)
									{
										var item = reusableList[i];
										if (item is not Item)
										{
											hasAnyTieredItem = true;
											break;
										}
										if (item is Item itm && tieredItemIds.Contains(itm.Id))
										{
											hasAnyTieredItem = true;
											break;
										}
									}

									if (!hasAnyTieredItem)
										continue;
								}

								bool isValid = validator(reusableList);

								if (isValid)
								{
									if (Interlocked.Read(ref validCombinationCount) < maxValidToStore)
									{
										// ✅ Create copy only when storing
										finalResultCollection.Add([.. reusableList]);
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

							return reusableList;
						},
						_ => { } // Cleanup (nothing to dispose)
					);
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

					// Sort lists by size descending for better parallelization
					allLists = [.. allLists.OrderByDescending(l => l.Count)];

					int largestListIndex = 0;
					var parallelizationSource = allLists[largestListIndex];
					var remainingLists = allLists.Skip(1).ToList();

					if (allLists.Count == 1)
					{
						// ✅ OPTIMIZED: Single list case with thread-local reusable list
						Parallel.ForEach(
							parallelizationSource,
							new ParallelOptions
							{
								CancellationToken = cancellationToken,
								MaxDegreeOfParallelism = Environment.ProcessorCount
							},
							() => new List<T>(1), // Thread-local list
							(item, state, reusableList) =>
							{
								if (cancellationToken.IsCancellationRequested)
								{
									cancelled = true;
									return reusableList;
								}

								reusableList.Clear();
								reusableList.Add(item);

								// ✅ OPTIMIZED: Balanced mode check
								if (filterStrategy == CombinationFilterStrategy.Balanced && tieredItemIds != null)
								{
									bool hasAnyTieredItem = false;
									for (int i = 0; i < reusableList.Count; i++)
									{
										var itm = reusableList[i];
										if (itm is not Item)
										{
											hasAnyTieredItem = true;
											break;
										}
										if (itm is Item itmCast && tieredItemIds.Contains(itmCast.Id))
										{
											hasAnyTieredItem = true;
											break;
										}
									}

									if (!hasAnyTieredItem)
										return reusableList;
								}

								bool isValid = validator(reusableList);
								if (isValid)
								{
									if (Interlocked.Read(ref validCombinationCount) < maxValidToStore)
									{
										finalResultCollection.Add([.. reusableList]);
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

								return reusableList;
							},
							_ => { }
						);
					}
					else
					{
						// ✅ OPTIMIZED: Multiple lists with thread-local reusable list
						Parallel.ForEach(
							parallelizationSource,
							new ParallelOptions
							{
								CancellationToken = cancellationToken,
								MaxDegreeOfParallelism = Environment.ProcessorCount
							},
							() => new List<T>(maxCombinationSize), // Thread-local list
							(item, state, reusableList) =>
							{
								if (cancellationToken.IsCancellationRequested)
								{
									cancelled = true;
									return reusableList;
								}

								foreach (var combination in GenerateIterativeCombinations(remainingLists))
								{
									if (cancelled || cancellationToken.IsCancellationRequested)
									{
										cancelled = true;
										return reusableList;
									}

									reusableList.Clear();
									reusableList.Add(item);
									reusableList.AddRange(combination);

									// ✅ OPTIMIZED: Balanced mode check
									if (filterStrategy == CombinationFilterStrategy.Balanced && tieredItemIds != null)
									{
										bool hasAnyTieredItem = false;
										for (int i = 0; i < reusableList.Count; i++)
										{
											var itm = reusableList[i];
											if (itm is not Item)
											{
												hasAnyTieredItem = true;
												break;
											}
											if (itm is Item itmCast && tieredItemIds.Contains(itmCast.Id))
											{
												hasAnyTieredItem = true;
												break;
											}
										}

										if (!hasAnyTieredItem)
											continue;
									}

									bool isValid = validator(reusableList);
									if (isValid)
									{
										if (Interlocked.Read(ref validCombinationCount) < maxValidToStore)
										{
											finalResultCollection.Add([.. reusableList]);
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

								return reusableList;
							},
							_ => { }
						);
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
		/// ✅ OPTIMIZED: Returns T[] instead of List<T> to avoid allocation
		/// The same array is reused across iterations, so callers MUST copy values immediately
		/// </summary>
		private static IEnumerable<T[]> GenerateIterativeCombinations<T>(List<List<T>> lists)
		{
			if (lists == null || lists.Count == 0)
			{
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

				// ✅ Return array directly - caller MUST copy values before next iteration
				yield return resultBuffer;

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
		/// ✅ OPTIMIZED: Returns (T, T) tuples instead of List<T> to avoid allocation
		/// </summary>
		private static IEnumerable<(T, T)> GetUniquePairs<T>(List<T> list)
		{
			for (int i = 0; i < list.Count - 1; i++)
			{
				for (int j = i + 1; j < list.Count; j++)
				{
					yield return (list[i], list[j]);
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
