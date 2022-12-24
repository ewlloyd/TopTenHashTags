namespace TopTenHashTags.Tests
{
    public class UtilityTests
    {
        [Theory]
        [InlineData("FirstTestString", "Fir*********ing")]
        [InlineData("short", "*****")]
        [InlineData("", "***")]
        public void Redact3And3Works(string plaintext, string expected)
        {
            Assert.Equal(expected, plaintext.Redact());
        }

        [Theory]
        [InlineData("FourHideThisFive!", "Four********Five!", 4, 5)]
        [InlineData("ShouldBeAllStars", "****************", 0, 0)]
        [InlineData("JustTheFrontPlease", "************Please", 0, 6)]
        [InlineData("JustTheBackPlease", "JustTheBack******", 11, 0)]
        [InlineData("TooShort", "********", 4, 4)]
        [InlineData("NotEnoughStars", "**************", 5, 6)]
        public void RedactCustomEndsWorks(string plaintext, string expected, int showFirst, int showLast)
        {
            Assert.Equal(expected, plaintext.Redact(showFirst, showLast));
        }

        [Theory]
        [InlineData("StillNotEnoughStars", "*******************", 8, 8, 4)]
        public void RedactCustomEndsAndStarsWorks(string plaintext, string expected, int showFirst, int showLast, int minStars)
        {
            Assert.Equal(expected, plaintext.Redact(showFirst, showLast, minStars));
        }


        [Theory]
        [InlineData("JustTryToRedactMe!", -1, 3, "showFirst")]
        [InlineData("JustTryToRedactMe!", 3, -1, "showLast")]
        public void RedactChecksArgs(string plaintext, int showFirst, int showLast, string badArgument)
        {
            Assert.Throws<ArgumentOutOfRangeException>(badArgument, () => plaintext.Redact(showFirst, showLast));
        }
    }
}