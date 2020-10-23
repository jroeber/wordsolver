using System.Collections.Generic;

namespace WordSolver.Core
{
    public interface IWordFinder
    {
        IEnumerable<string> FindWords(string letters, int lowerBound);
    }
}