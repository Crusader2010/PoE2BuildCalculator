namespace Manager
{
    public static class CombinationGenerator
    {
        /// <summary>
        /// Efficiently generates all possible combinations and filters them with a provided validator.
        /// </summary>
        /// <param name="listOfStatsWithoutRings">The list of lists where one item is taken from each.</param>
        /// <param name="listOfRings">The special list where two different items must be taken.</param>
        /// <param name="validator">A function that returns true for valid combinations, false otherwise.</param>
        /// <returns>An IEnumerable of lists, yielding one valid combination at a time.</returns>
        public static IEnumerable<List<T>> GenerateEfficientCombinations<T>(
            List<List<T>> listOfStatsWithoutRings,
            List<T> listOfRings,
            Func<List<T>, bool> validator)
        {
            if (listOfRings == null || listOfRings.Count < 2)
            {
                throw new ArgumentException("The special list must contain at least two different values.", nameof(listOfRings));
            }

            var generalCombinations = GenerateRecursiveCombinations(listOfStatsWithoutRings, [], 0);

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

        /// <summary>
        /// Recursively generates combinations from multiple lists using yield return.
        /// </summary>
        private static IEnumerable<List<T>> GenerateRecursiveCombinations<T>(
            List<List<T>> lists,
            List<T> currentCombination,
            int listIndex)
        {
            if (listIndex == lists.Count)
            {
                yield return currentCombination;
                yield break;
            }

            foreach (var item in lists[listIndex])
            {
                currentCombination.Add(item);
                foreach (var subCombination in GenerateRecursiveCombinations(lists, currentCombination, listIndex + 1))
                {
                    yield return subCombination;
                }

                currentCombination.RemoveAt(currentCombination.Count - 1);
            }
        }

        /// <summary>
        /// Lazily generates all unique pairs from a single list.
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