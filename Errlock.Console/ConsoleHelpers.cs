using System;

namespace ErrlockConsole
{
    public static class ConsoleHelpers
    {
        public static void Show(string message, bool withNewLine = true)
        {
            if (withNewLine) {
                Console.WriteLine(message);
            } else {
                Console.Write(message);
            }
        }

        public static void ShowWithColor(
            string message, ConsoleColor color, bool withNewLine = true)
        {
            Console.ForegroundColor = color;
            if (withNewLine) {
                Console.WriteLine(message);
            } else {
                Console.Write(message);
            }
            Console.ResetColor();
        }
    }
}