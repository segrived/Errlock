using System;

namespace ErrlockConsole
{
    public static class ConsoleHelpers
    {
        public static void ShowPrompt(string text = "", ConsoleColor color = ConsoleColor.Green)
        {
            WriteColor(String.Format("> {0}", text), color);
        }

        public static void ShowError(string errorText)
        {
            string message = String.Format("[!] Ошибка: {0}", errorText);
            WriteColorLine(message, ConsoleColor.Red);
        }

        public static void ShowOkMessage(string message)
        {
            WriteColorLine(message, ConsoleColor.Green);
        }

        public static void WriteColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void PrintColor(this string message, ConsoleColor color)
        {
            WriteColor(message, color);
        }

        public static void WriteColorLine(string message, ConsoleColor color)
        {
            WriteColor(message, color);
            Console.WriteLine();
        }
    }
}