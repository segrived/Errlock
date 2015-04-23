using System;

namespace ErrlockConsole
{
    public class ConsoleRequester<T>
    {
        readonly Func<string, T> _stringConverter;
        readonly Predicate<T> _isValidRequest;

        public ConsoleRequester(Func<string, T> converter, Predicate<T> predicate)
        {
            this._stringConverter = converter;
            this._isValidRequest = predicate;
        }

        private static void ShowPrompt()
        {
            ConsoleHelpers.ShowWithColor("> ", ConsoleColor.Green, false);
        }

        private static void ShowError(string errorText)
        {
            string message = String.Format("[!] Ошибка: {0}", errorText);
            ConsoleHelpers.ShowWithColor(message, ConsoleColor.Red);
        }

        public T Request()
        {
            while (true) {
                try {
                    ShowPrompt();
                    var value = _stringConverter(Console.ReadLine());
                    if (!_isValidRequest.Invoke(value)) {
                        ShowError("Ввод не удовлетворяет предикату");
                    } else {
                        return value;
                    }
                } catch {
                    ShowError("Неверный ввод");
                }
            }
        }
    }
}