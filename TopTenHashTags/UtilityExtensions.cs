namespace TopTenHashTags;

public static class UtilityExtensions
{
    /// <summary>
    /// Returns a partially-redacted version of the <paramref name="plaintext"/> with the first
    /// <paramref name="showFirst"/> and the last <paramref name="showLast"/> characters exposed.
    /// </summary>
    /// <param name="plaintext">The plaintext to redact</param>
    /// <param name="showFirst">The number of initial characters to leave intact, if possible</param>
    /// <param name="showLast">The number of characters to leave intact at the end, if possible</param>
    /// <param name="minStars">The minimum number of stars to show (defaults to 3)</param>
    /// <returns>The redacted string</returns>
    /// <remarks>
    /// This method always performs some redaction; it returns only asterisks if the plaintext would be otherwise unchanged.
    /// In addition, if <paramref name="plaintext"/> is <code>null</code> or shorter than <paramref name="minStars"/>,
    /// then a string of <paramref name="minStars"/> asterisks is returned.
    /// </remarks>
    public static string Redact(this string? plaintext, int showFirst = 3, int showLast = 3, int minStars = 3)
    {
        showFirst.ThrowIfNegative(nameof(showFirst));
        showLast.ThrowIfNegative(nameof(showLast));

        plaintext ??= "";

        var starCount = plaintext.Length - (showFirst + showLast);

        return starCount <= minStars
            ? new string('*', int.Max(plaintext.Length, minStars))
            : plaintext[..showFirst] + new string('*', starCount) + plaintext[^showLast..];
    }

    /// <summary>
    /// Throws an <c>ArgumentOutOfRangeException</c> if the <paramref name="argumentValue"/> is negative.
    /// </summary>
    /// <param name="argumentValue">The value to verify</param>
    /// <param name="argumentName">The caller's name of the parameter being inspected</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="argumentValue"/> is negative;
    /// otherwise no action is taken.</exception>
    public static void ThrowIfNegative(this int argumentValue, string argumentName)
    {
        if (argumentValue < 0)
            throw new ArgumentOutOfRangeException(argumentName, "cannot be negative");
    }
}