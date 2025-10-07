using System.Numerics;

namespace Domain.Combinations
{
    public static class CombinationGenerator
    {
        /// <summary>
        /// Generates all possible combinations and filters them with a provided validator.
        /// </summary>
        /// <param name="listOfItemClassesWithoutRings">The list of items split per item class, except rings. One item is taken from each sublist.</param>
        /// <param name="listOfRings">The list of items with class = Ring, from which two different items must be taken.</param>
        /// <param name="validator">A function that returns true for valid combinations, false otherwise.</param>
        /// <returns>An IEnumerable of lists, yielding one valid combination at a time.</returns>
        public static IEnumerable<List<T>> GenerateCombinations<T>(
            List<List<T>> listOfItemClassesWithoutRings,
            List<T> listOfRings,
            Func<List<T>, bool> validator)
        {
            if (listOfRings == null || listOfRings.Count < 2)
            {
                throw new ArgumentException("The special list must contain at least two different values.", nameof(listOfRings));
            }

            var generalCombinations = GenerateRecursiveCombinations(listOfItemClassesWithoutRings, [], 0);

            foreach (var pair in GetUniquePairs(listOfRings))
            {
                foreach (var generalCombination in generalCombinations)
                {
                    List<T> combinedList = [.. pair, .. generalCombination];

                    // Only yield the combination if it passes the validation check.
                    if (validator(combinedList))
                    {
                        yield return combinedList;
                    }
                }
            }
        }

        public static BigInteger GetTotalCombinationsCount<T>(
            int pairListCount,
            List<List<T>> otherLists)
        {
            // The total is the product of the count of every list.
            BigInteger totalCount = new(pairListCount);

            foreach (var list in otherLists)
            {
                // Multiplying BigInteger with BigInteger prevents overflow
                totalCount *= new BigInteger(list.Count);
            }

            return totalCount;
        }

        /// <summary>
        /// Recursively generates combinations from multiple lists using yield return.
        /// Each recursive call creates a NEW list by copying the parent's combination and adding
        ///     the current item. This ensures each yielded combination is a distinct, immutable snapshot
        ///     that won't be affected by subsequent recursive calls.
        /// </summary>
        private static IEnumerable<List<T>> GenerateRecursiveCombinations<T>(
            List<List<T>> lists,
            List<T> currentCombination,
            int listIndex)
        {
            if (listIndex == lists.Count)
            {
                // Yield a NEW copy of the combination to prevent mutation issues
                yield return new List<T>(currentCombination);
                yield break;
            }

            foreach (var item in lists[listIndex])
            {
                // Create a NEW list for each branch instead of mutating the shared one
                var newCombination = new List<T>(currentCombination) { item };

                foreach (var subCombination in GenerateRecursiveCombinations(lists, newCombination, listIndex + 1))
                {
                    yield return subCombination;
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
    }
}