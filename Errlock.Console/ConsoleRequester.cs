using System;
using System.Collections.Generic;
using System.Linq;

namespace Errlock.Console
{
    public static class ConsoleRequester
    {
        /// <summary>
        /// Запрашивает целое число с консоли
        /// </summary>
        /// <param name="prompt">Строка запроса</param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int RequestInt(string prompt, int def)
        {
            return new ConsoleRequester<int>(Int32.Parse).RequestValue(prompt, defaultValue: def);
        }

        /// <summary>
        /// Запрашивает строку с консоли
        /// </summary>
        /// <param name="prompt">Строка запроса</param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string RequestString(string prompt, string def)
        {
            return new ConsoleRequester<string>().RequestValue(prompt, defaultValue: def);
        }

        /// <summary>
        /// Запрашивает булево значение с консоли. Значения "true", "1", "yes", "да" и "+"
        /// интерпретируются как True, все остальное как False. Регистр при сравнении не учитывается
        /// </summary>
        /// <param name="prompt">Строка запроса</param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static bool RequestBool(string prompt, bool def)
        {
            var trueVariations = new List<string> { "true", "1", "yes", "да", "+" };
            return new ConsoleRequester<bool>(s => trueVariations.Contains(s.ToLower()))
                .RequestValue(prompt, defaultValue: def);
        }

        /// <summary>
        /// Запрашивает элемент коллекции с консоли
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="items">Исходная коллекция, из которой необходимо выбрать элемент</param>
        /// <param name="itemToStringFunc">
        /// Функция, преобразовывающая элемент в строку, для вывода этой строки на экран
        /// в качестве значения
        /// </param>
        /// <param name="title">
        /// Заголовок, отображает над элементами коллеции, указывать необзяательно
        /// </param>
        /// <returns>Выбранный элемент исходной коллекции типа <typeparam name="T" /></returns>
        public static T RequestListItem<T>(
            IEnumerable<T> items, Func<T, string> itemToStringFunc,
            string title = null)
        {
            var sourceList = items.ToList();
            if (sourceList.Count == 0) {
                throw new EmptyCollectionException("Пустая коллекция");
            }
            var indexesList = Enumerable.Range(1, sourceList.Count).ToList();
            var zipped = indexesList
                .Zip(sourceList, (i, x) => new KeyValuePair<int, T>(i, x))
                .ToDictionary(i => i.Key, i => i.Value);

            if (! String.IsNullOrEmpty(title)) {
                ConsoleHelpers.WriteColorLine(title, ConsoleColor.DarkYellow);
            }

            foreach (var kvPair in zipped) {
                ConsoleHelpers.WriteColor(kvPair.Key + ". ", ConsoleColor.Magenta);
                string valueStr = itemToStringFunc.Invoke(kvPair.Value);
                ConsoleHelpers.WriteColorLine(valueStr, ConsoleColor.Gray);
            }
            var requester = new ConsoleRequester<int>(Int32.Parse);
            requester.AddPredicate(x => x < 1 || x > indexesList.Count, "Неверный номер");
            int selectedNumber = requester.RequestValue("Введите номер необходимого варианта: ");
            var selectedItem = zipped[selectedNumber];
#if DEBUG
            string selectedValueStr = itemToStringFunc.Invoke(selectedItem);
            string message = String.Format("Было выбрано значение `{0}`", selectedValueStr);
            ConsoleHelpers.WriteColorLine(message, ConsoleColor.DarkGray);
#endif
            return selectedItem;
        }
    }

    public class ConsoleRequester<T>
    {
        private const string DefaultErrorMessage = "Ввод не удовлетворяет предикату";

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

        /// <summary>
        /// Добавляет ограничение на вводимые данные
        /// </summary>
        /// <param name="predicate">
        /// Предикат. При вызове метода RequestValue, в случае, если предикат примененный к
        /// значению возвращает True, будет отображено сообщение об ошибке и будет предложено 
        /// ввести значение повторно
        /// </param>
        /// <param name="message">
        /// Сообщение об ошибке, если не указано, то по умолчанию
        /// будет использоваться системное сообщение
        /// </param>
        public void AddPredicate(Predicate<T> predicate, string message = DefaultErrorMessage)
        {
            this._predicatesList.Add(Tuple.Create(predicate, message));
        }

        /// <summary>
        /// Запрашивает ввод данных с консоли
        /// </summary>
        /// <param name="prompt">Строка запроса</param>
        /// <param name="color">Цвет строки запроса</param>
        /// <param name="defaultValue">
        /// Значение по умолчанию, будет возвращено, если строка запроса оказалась пустой
        /// </param>
        /// <returns>Обработанное значение, введенное с консоли</returns>
        public T RequestValue(string prompt = "", ConsoleColor color = ConsoleColor.Green, 
            T defaultValue = default(T))
        {
            while (true) {
                try {
                    string fullPrompt = prompt;
                    if (! String.IsNullOrWhiteSpace(prompt)) {
                        if (defaultValue != null) {
                            fullPrompt += String.Format(" (по умолчанию {0})", defaultValue);
                        }
                        fullPrompt += ": ";
                    }
                    ConsoleHelpers.ShowPrompt(fullPrompt, color);
                    string requestString = System.Console.ReadLine();
                    var value = String.IsNullOrWhiteSpace(requestString) 
                        ? defaultValue 
                        : _stringConverter(requestString);
#if DEBUG
                    ConsoleHelpers.WriteColorLine("Выбрано значение " + value, ConsoleColor.DarkGray);
#endif
                    var pred = this._predicatesList.FirstOrDefault(p => p.Item1.Invoke(value));
                    if (pred == null) {
                        return value;
                    }
                    ConsoleHelpers.ShowError(pred.Item2);
                } catch {
                    ConsoleHelpers.ShowError("Неверный ввод");
                }
            }
        }
    }
}