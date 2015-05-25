using System.Collections.Generic;
using System.Linq;

namespace Errlock.Lib.Helpers
{
    public static class EnumerableHelpers
    {
        /// <summary>
        /// Присваивает каждому элементу коллекции индекс и возвращает экземпляр класса Dictiorary,
        /// где ключ это индект, а значение - элемент коллекции
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="input">Исходная коллекция</param>
        /// <returns>Словарь, где ключ это индекс, а значение - элемент коллекции</returns>
        public static Dictionary<int, T> ToDictByIndex<T>(this IEnumerable<T> input)
        {
            var inputList = input.ToList();
            int inputLength = inputList.Count();
            var resultCollection = Enumerable.Range(0, inputLength)
                             .Zip(inputList, (a, b) => new KeyValuePair<int, T>(a, b))
                             .ToDictionary(v => v.Key, v => v.Value);
            return resultCollection;
        }

        /// <summary>
        /// Выполянет декартово произведение между двумя коллекциями строк, генерируя
        /// новые строки, разделяя значения параметром, переданным аргументом
        /// <paramref name="sep"/> 
        /// </summary>
        /// <param name="input1">Первая коллекция</param>
        /// <param name="input2">Вторая коллекция</param>
        /// <param name="sep">Разделитель элементов</param>
        /// <returns>Элементы, сгенерированные с помощью декартового произведения</returns>
        public static IEnumerable<string> StringCartesianProduct(
            IEnumerable<string> input1, IEnumerable<string> input2, string sep)
        {
            var enumerable = input2.ToList();
            foreach (string a in input1) {
                foreach (string b in enumerable) {
                    yield return string.Join(sep, a, b);
                    yield return string.Join(sep, b, a);
                }
            }
        }
    }
}
