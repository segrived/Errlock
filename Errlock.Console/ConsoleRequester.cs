using System;
using System.Collections.Generic;
using System.Linq;

namespace ErrlockConsole
{
    public static class ConsoleRequester
    {
        public static int RequestInt(string prompt)
        {
            return new ConsoleRequester<int>(Int32.Parse).RequestValue(prompt);
        }

        public static string RequestString(string prompt)
        {
            return new ConsoleRequester<string>().RequestValue(prompt);
        }

        public static bool RequestBool(string prompt)
        {
            var trueVariations = new List<string> { "true", "1", "yes", "да" };
            return new ConsoleRequester<bool>(s => trueVariations.Contains(s.ToLower()))
                .RequestValue(prompt);
        }

        public static T RequestListItem<T>(IEnumerable<T> items, Func<T, string> itemToStringFunc)
        {
            var sessionList = items.ToList();
            var indexesList = Enumerable.Range(0, sessionList.Count).ToList();
            var zipped = indexesList
                .Zip(sessionList, (i, x) => new KeyValuePair<int, T>(i, x))
                .ToDictionary(i => i.Key, i => i.Value);
            foreach (var kvPair in zipped) {
                ConsoleHelpers.WriteColor(kvPair.Key + ". ", ConsoleColor.Magenta);
                string valueStr = itemToStringFunc.Invoke(kvPair.Value);
                ConsoleHelpers.WriteColorLine(valueStr, ConsoleColor.Gray);
            }
            var requester = new ConsoleRequester<int>(Int32.Parse);
            requester.AddPredicate(x => x < 0 || x >= indexesList.Count, "Неверный номер");
            int selectedNumber = requester.RequestValue("Укажите номер: ");
            var selectedItem = zipped[selectedNumber];
            string selectedValueStr = itemToStringFunc.Invoke(selectedItem);

            string message = String.Format("Было выбрано значение `{0}`", selectedValueStr);
            ConsoleHelpers.ShowOkMessage(message);
            return selectedItem;
        }
    }

    public class ConsoleRequester<T>
    {
        private const string DefaultErrorMessage = "Ввод не удовлетворяет предикат";

        /// <summary>
        /// Конвертор по умолчанию, возвращает введенную строку
        /// </summary>
        private readonly Func<string, T> _defaultConverter =
            _ => (T)Convert.ChangeType(_, typeof(T));

        private readonly List<Tuple<Predicate<T>, string>> _predicatesList =
            new List<Tuple<Predicate<T>, string>>();

        private readonly Func<string, T> _stringConverter;

        public ConsoleRequester(Func<string, T> converter)
        {
            this._stringConverter = converter;
        }

        public ConsoleRequester()
        {
            this._stringConverter = _defaultConverter;
        }

        public void AddPredicate(Predicate<T> predicate, string message = DefaultErrorMessage)
        {
            this._predicatesList.Add(Tuple.Create(predicate, message));
        }

        /// <summary>
        /// Запрашивает ввод данных с консоли
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="maxAttempts">
        /// Количество максимальных попыток. Если за указанное количество 
        /// попыток ввод не разу не прошел проверку одним из добавленных предикатов, 
        /// функция возращает null
        /// </param>
        /// <returns>Обработанное значение, введенное с консоли</returns>
        public T RequestValue(string prompt = "", int maxAttempts = -1)
        {
            int attempts = 0;
            while (maxAttempts == -1 || attempts < maxAttempts) {
                try {
                    ConsoleHelpers.ShowPrompt(prompt);
                    var value = _stringConverter(Console.ReadLine());
                    var pred = this._predicatesList.FirstOrDefault(p => p.Item1.Invoke(value));
                    if (pred == null) {
                        return value;
                    }
                    ConsoleHelpers.ShowError(pred.Item2);
                } catch {
                    ConsoleHelpers.ShowError("Неверный ввод");
                }
                attempts++;
            }
            ConsoleHelpers.ShowError("Превышено максимальное количество попыток ввода");
            return default(T);
        }
    }
}