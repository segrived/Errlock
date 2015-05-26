using System;

namespace Errlock.Console
{
    public static class ConsoleHelpers
    {
        public static void ShowPrompt(string text = "", ConsoleColor color = ConsoleColor.Green)
        {
            WriteColor(String.Format(">>> {0}", text), color);
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
            System.Console.ForegroundColor = color;
            System.Console.Write(message);
            System.Console.ResetColor();
        }

        public static void WriteColorLine(string message, ConsoleColor color)
        {
            WriteColor(message, color);
            System.Console.WriteLine();
        }
    }
}