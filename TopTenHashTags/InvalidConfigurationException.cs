using System.Diagnostics.CodeAnalysis;

namespace TopTenHashTags;

public class InvalidConfigurationException : ApplicationException
{
    public string ConfigurationKey { get; init; }
    public InvalidConfigurationException(string configurationKey, string? message)
        : base(message)
    {
        ConfigurationKey = configurationKey;
    }

    public static void ThrowIfNullOrWhitespace(string configurationKey, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            Throw(configurationKey);
    }

    [DoesNotReturn]
    private static void Throw(string configurationKey)
    {
        throw new InvalidConfigurationException(configurationKey,
            $"A configuration value is required for the configuration key \"{configurationKey}\"");
    }
}