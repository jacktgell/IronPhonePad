using Xunit;
using PhonePadLibrary;

namespace PhonePadTests
{
    public class PhoneKeypadTests
    {
        [Theory]
        [InlineData("33#", "E")]
        [InlineData("227*#", "B")]
        [InlineData("4433555 555666#", "HELLO")]
        public void OldPhonePad_ShouldReturnCorrectOutput_ForProvidedRequirements(string input, string expected)
        {
            string result = PhoneKeypad.OldPhonePad(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void OldPhonePad_ShouldDecodeComplexTuringExample()
        {
            // Scenario from requirements: "8 88777444666*664#" -> "TURING"
            string input = "8 88777444666*664#";
            string expected = "TURING"; 

            string result = PhoneKeypad.OldPhonePad(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("2#", "A")]
        [InlineData("22#", "B")]
        [InlineData("222#", "C")]
        [InlineData("2222#", "A")] // Wraps around (A -> B -> C -> A)
        [InlineData("7777#", "S")] // 4-char key (P -> Q -> R -> S)
        [InlineData("9999#", "Z")] // 4-char key (W -> X -> Y -> Z)
        public void OldPhonePad_ShouldCycleKeysCorrectly(string input, string expected)
        {
            string result = PhoneKeypad.OldPhonePad(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("22*#", "")]        // Type B, Backspace -> Empty
        [InlineData("22*2#", "A")]      // Type B, Backspace, Type A -> A
        [InlineData("*******#", "")]    // Backspace on empty buffer (Should not crash)
        [InlineData("4433555*******#", "")] // Delete entire word
        public void OldPhonePad_ShouldHandleBackspacesRobustly(string? input, string expected)
        {
            string result = PhoneKeypad.OldPhonePad(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void OldPhonePad_ShouldIgnoreInputAfterSendKey()
        {
            // The '#' character signifies the end of the message.
            string input = "222#999999"; 
            string expected = "C"; 

            string result = PhoneKeypad.OldPhonePad(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("#")]
        public void OldPhonePad_ShouldHandleEmptyOrNullInputGracefully(string? input) 
        {
            string result = PhoneKeypad.OldPhonePad(input);
            Assert.True(string.IsNullOrEmpty(result));
        }
        
        [Theory]
        [InlineData("456", "GJM")]   // Regression: Missing '#' terminator
        [InlineData("22", "B")]      // Flush state at end of string
        [InlineData("2233", "BE")]   // Multiple flush operations
        public void OldPhonePad_ShouldFlushPendingState_WhenSendKeyIsMissing(string input, string expected)
        {
            string result = PhoneKeypad.OldPhonePad(input);
            Assert.Equal(expected, result);
        }
    }
}