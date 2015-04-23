using System.Collections.Generic;
using System.Linq;

namespace ErrlockConsole
{
    public static class EnumerableExtensions
    {
        public static Dictionary<int, T> ToDictByIndex<T>(this IEnumerable<T> input)
        {
            var inputList = input.ToList();
            int inputLength = inputList.Count();
            return Enumerable.Range(0, inputLength)
                             .Zip(inputList, (a, b) => new KeyValuePair<int, T>(a, b))
                             .ToDictionary(v => v.Key, v => v.Value);
        }
    }
}