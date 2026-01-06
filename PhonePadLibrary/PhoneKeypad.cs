using System;
using System.Text;

namespace PhonePadLibrary
{
    /// <summary>
    /// Provides stateless functionality to decode old-school T9 numeric keypad entries.
    /// </summary>
    public static class PhoneKeypad
    {
        // Configuration: Define control keys as constants for maintainability.
        private const char SendKey = '#';
        private const char BackspaceKey = '*';

        // Mapping: Array index corresponds to the digit pressed.
        // Index 0 is mapped to Space, Index 1 is mapped to Symbols.
        private static readonly string[] KeypadMap = 
        {
            " ",    // 0
            "&'(",  // 1
            "ABC",  // 2
            "DEF",  // 3
            "GHI",  // 4
            "JKL",  // 5
            "MNO",  // 6
            "PQRS", // 7
            "TUV",  // 8
            "WXYZ"  // 9
        };

        /// <summary>
        /// Decodes a raw input string from a numeric keypad into alphabetic text.
        /// </summary>
        /// <param name="input">The raw input string (e.g., "222 2").</param>
        /// <returns>The decoded string.</returns>
        public static string OldPhonePad(string? input)
        {
            // Fail-fast guard clause for null or empty input.
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var output = new StringBuilder();
            char? currentButton = null;
            int currentCount = 0;

            foreach (char c in input)
            {
                // 1. Termination Check
                if (c == SendKey)
                {
                    CommitPendingKey(output, currentButton, currentCount);
                    return output.ToString();
                }

                // 2. Digit Handling (The State Machine)
                if (char.IsDigit(c))
                {
                    if (currentButton == c)
                    {
                        // Same button pressed repeatedly: Increment cycle count.
                        currentCount++;
                    }
                    else
                    {
                        // New button pressed: Commit the previous state and start a new one.
                        CommitPendingKey(output, currentButton, currentCount);
                        currentButton = c;
                        currentCount = 1;
                    }
                    continue;
                }

                // 3. Special Handling (Spaces/Pauses)
                // Any non-digit character (like space) acts as a commit signal for the current button.
                CommitPendingKey(output, currentButton, currentCount);
                
                // Reset state
                currentButton = null;
                currentCount = 0;

                // 4. Backspace Handling
                // Performed after commit to ensure we delete the correct committed character.
                if (c == BackspaceKey)
                {
                    if (output.Length > 0)
                    {
                        output.Length--; // Efficiently remove the last character from the buffer.
                    }
                }
            }

            // Flush: If the input stream ends without a terminator (#), commit the final pending state.
            CommitPendingKey(output, currentButton, currentCount);
            
            return output.ToString();
        }

        /// <summary>
        /// Helper to calculate the correct character based on button press count and append it to the buffer.
        /// </summary>
        private static void CommitPendingKey(StringBuilder buffer, char? button, int count)
        {
            // Validate that we have a valid digit state to commit.
            if (button == null || !char.IsDigit(button.Value)) return;

            int index = button.Value - '0';

            // Boundary check to prevent IndexOutOfRangeException.
            if (index < 0 || index >= KeypadMap.Length || KeypadMap[index] == null) return;

            string letters = KeypadMap[index];
            
            // Mathematical cycling: (count - 1) converts 1-based counts to 0-based indices.
            // The modulo operator (%) handles wrapping (e.g., pressing '2' four times returns to 'A').
            int letterIndex = (count - 1) % letters.Length;
            
            buffer.Append(letters[letterIndex]);
        }
    }
}