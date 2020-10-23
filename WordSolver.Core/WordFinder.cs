using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordSolver.Core
{
    public class WordFinder : IWordFinder
    {
        readonly List<string> wordList;

        public WordFinder(string wordFileName)
        {
            var wordArray = File.ReadAllLines(wordFileName);
            wordList = new List<string>(wordArray); // TODO validation/exception handling
        }

        public IEnumerable<string> FindWords(string letters, int lowerBound = 3)
        {
            // TODO validate/massage input (make lowercase, only include a-z characters)
            // TODO another approach for long inputs would be to check line by line in the dictionary file for matches with input letters

            var combos = UniqueLetterCombos(letters, lowerBound);
            var combosWithVowels = combos.Where(c => HasAVowel(c)); // A word must have at least one vowel
            var permutations = combosWithVowels.SelectMany(c => Permutations(c)); // (SelectMany flattens the result so it's not an enumerable of lists)
            var words = permutations
                .Distinct()
                .AsParallel()
                .Where(perm => wordList.Contains(perm))
                .OrderBy(s => s.Length)
                .ThenBy(s => s);

            return words;
        }

        // Get all letter combos, e.g. abc => abc, ab, ac, bc, a, b, c
        private HashSet<string> UniqueLetterCombos(string input, int lowerBound = 3)
        {
            // Base case
            if (input.Length <= lowerBound)
            {
                return new HashSet<string>{input};
            }

            // https://stackoverflow.com/a/6441603 gives a non-LINQ string sort option which may be faster
            // Also sorting is only needed the first time, maybe fix this later if overhead is too much
            string sortedInput = String.Concat(input.OrderBy(c => c));

            HashSet<string> combos = new HashSet<string>{};

            for(int i = 0; i < input.Length; i++)
            {
                foreach (string combo in UniqueLetterCombos(input.Remove(i, 1), lowerBound))
                {
                    combos.Add(combo);
                }
            }

            combos.Add(input);
            return combos;
        }

        // Get all permutations of a string, e.g. abc => abc, bac, bca, acb, cab, cba
        // (This is a direct implementation of Heap's Algorithm - https://en.wikipedia.org/wiki/Heap%27s_algorithm)
        private List<string> Permutations(string input)
        {
            List<string> rtn = new List<string>();
            int[] c = new int[input.Length];
            char[] inChars = input.ToArray();

            rtn.Add(input);

            int i = 0;
            while (i < input.Length)
            {
                if (c[i] < i)
                {
                    if (i % 2 == 0)
                    {
                        char tmp = inChars[0];
                        inChars[0] = inChars[i];
                        inChars[i] = tmp;
                    }
                    else
                    {
                        char tmp = inChars[c[i]];
                        inChars[c[i]] = inChars[i];
                        inChars[i] = tmp;
                    }
                    rtn.Add(new string(inChars));
                    c[i]++;
                    i = 0;
                }
                else
                {
                    c[i] = 0;
                    i++;
                }
            }

            return rtn;
        }

        private bool HasAVowel(string input)
        {
            foreach (var letter in input)
            {
                if (letter == 'a' ||
                    letter == 'e' ||
                    letter == 'i' ||
                    letter == 'o' ||
                    letter == 'u' ||
                    letter == 'y')

                    return true;
            }
            return false;
        }
    }
}