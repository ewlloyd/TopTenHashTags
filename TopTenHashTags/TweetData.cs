namespace TopTenHashTags;

public class TweetData
{
    public Tweet? Data { get; set; }
}

public class Tweet
{
    public string? Text { get; set; }
    public Entities? Entities { get; set; }
}

public class Entities
{
    public Hashtag[] Hashtags { get; set; } = null!;
}

public class Hashtag
{
    public string Tag { get; set; } = null!;
    public override string ToString() => Tag;
}
