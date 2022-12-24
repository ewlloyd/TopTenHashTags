using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TopTenHashTags;

internal class Ingestor : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Ingestor> _logger;
    private readonly ILeaderboard<string> _leaderboard;
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Ingestor(HttpClient client, IConfiguration configuration, ILogger<Ingestor> logger,
        ILeaderboard<string> leaderboard)
    {
        _client = client;
        _configuration = configuration;
        _logger = logger;
        _leaderboard = leaderboard;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Running Ingestor...");
        var bearerToken = _configuration["TwitterApi:BearerToken"];
        var baseUri = _configuration["TwitterApi:BaseUri"];
        InvalidConfigurationException.ThrowIfNullOrWhitespace("TwitterApi:BaseUri", baseUri);

        _logger.LogInformation("BearerToken: {bearerToken}", bearerToken.Redact());
        _logger.LogInformation("BaseUri: {baseUri}", baseUri);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        await ReadTweetStream(baseUri!, stoppingToken);
    }

    private async Task ReadTweetStream(string baseUri, CancellationToken cancellationToken)
    {
        try
        {
            var requestUrl = baseUri + "/2/tweets/sample/stream?tweet.fields=entities";
            _logger.LogInformation("Requesting from {requestUrl}", requestUrl);
            var response = await _client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Request accepted, awaiting stream start...");
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                
                if (string.IsNullOrWhiteSpace(line))
                {
                    _logger.LogInformation("Line #{TotalTweets} of the stream is blank.", _leaderboard.TotalTweets);
                    continue;
                }

                var tweet = JsonSerializer.Deserialize<TweetData>(line, _serializerOptions);
                var tags = tweet?.Data?.Entities?.Hashtags;
                if (tags != null)
                {
                    _logger.LogInformation("#{tags}", string.Join(", #", tags.Select(tag => tag.ToString())));
                    foreach (var hashtag in tags)
                    {
                        _leaderboard.TallyHashtag(hashtag.Tag);
                    }
                }

                _leaderboard.TallyTweet();
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task cancellation noted. Shutting down...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestor failure");
        }
    }
}
