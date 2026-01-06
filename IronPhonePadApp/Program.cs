using System;
using PhonePadLibrary;

namespace IronPhonePadApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initial UI State
            string currentScreenText = "READY...";
            string lastInput = "";

            while (true)
            {
                DrawPhoneInterface(currentScreenText, lastInput);

                Console.Write("   Type Input > ");
                string input = Console.ReadLine() ?? string.Empty;

                // 1. Command Handling: Exit
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                // 2. Command Handling: Manual Reset
                if (string.Equals(input, "clear", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(input, "cls", StringComparison.OrdinalIgnoreCase))
                {
                    currentScreenText = "READY...";
                    lastInput = "";
                    continue;
                }

                lastInput = input;
                try
                {
                    // Delegate logic to the library
                    currentScreenText = PhoneKeypad.OldPhonePad(input);
                }
                catch (Exception)
                {
                    currentScreenText = "ERROR";
                }
            }
        }

        static void DrawPhoneInterface(string screenText, string lastInput)
        {
            // Clear the console to prevent artifacting (ghosting).
            CleanScreen();
            
            Console.WriteLine();
            Console.WriteLine("   IRON SOFTWARE CODING CHALLENGE");
            Console.WriteLine("   ------------------------------");
            Console.WriteLine();
            
            // ASCII Phone UI
            Console.WriteLine("      _____________________ ");
            Console.WriteLine("     /                     \\");
            Console.WriteLine("    |  [___]  NOKIA  (o)    |");
            Console.WriteLine("    |  ___________________  |");
            Console.WriteLine("    | |                   | |");
            
            // Screen Scrolling Logic: Mimic a real phone display window
            string line = FormatScreenText(screenText, 17);
            Console.WriteLine($"    | | {line} | |");
            
            Console.WriteLine("    | |___________________| |");
            Console.WriteLine("    |                       |");
            Console.WriteLine("    |[  <  ] [ MENU] [  >  ]|");
            Console.WriteLine("    |                       |");
            Console.WriteLine("    | [ 1 ]   [ 2 ]   [ 3 ] |");
            Console.WriteLine("    | & ' (   a b c   d e f |");
            Console.WriteLine("    |                       |");
            Console.WriteLine("    | [ 4 ]   [ 5 ]   [ 6 ] |");
            Console.WriteLine("    | g h i   j k l   m n o |");
            Console.WriteLine("    |                       |");
            Console.WriteLine("    | [ 7 ]   [ 8 ]   [ 9 ] |");
            Console.WriteLine("    | pqrs    t u v   wxyz  |");
            Console.WriteLine("    |                       |");
            Console.WriteLine("    | [ * ]   [ 0 ]   [ # ] |");
            Console.WriteLine("    | < DEL   _ SPC   SEND  |");
            Console.WriteLine("     \\_____________________/");
            Console.WriteLine();
            
            // Display: Raw Input (Verification)
            if (!string.IsNullOrEmpty(lastInput))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"   Raw Input: {lastInput}");
                Console.ResetColor();
            }

            // Display: Full Decoded Output
            if (!string.IsNullOrEmpty(screenText) && screenText != "READY...")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   > DECODED: {screenText}");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("   (Type digits. Type 'clear' to reset. Type 'exit' to quit)");
            Console.WriteLine();
        }

        /// <summary>
        /// Formats text to fit the ASCII screen, scrolling from the right if the text is too long.
        /// </summary>
        static string FormatScreenText(string text, int width)
        {
            if (string.IsNullOrEmpty(text)) return new string(' ', width);
            
            if (text.Length > width)
            {
                return text.Substring(text.Length - width);
            }

            int padding = (width - text.Length) / 2;
            return text.PadLeft(padding + text.Length).PadRight(width);
        }

        /// <summary>
        /// Attempts to clear the console buffer. Includes a fallback for IDE internal consoles.
        /// </summary>
        static void CleanScreen()
        {
            try { Console.Clear(); } catch { }
        }
    }
}