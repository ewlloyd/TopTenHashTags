using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace TopTenHashTags
{
    public interface ILeaderboard<T>
        where T : IComparable<T>
    {
        void TallyHashtag(T key);
        void TallyTweet();
        ImmutableList<IResult<T>> Results { get; }
        int TotalTweets { get; }
        IResults<T> GetResults();
    }

    public interface IResults<T>
    {
        int TotalTweets { get; }
        ImmutableList<IResult<T>> Results { get; }
    }

    public interface IResult<out T>
    {
        T Value { get; }
        int Count { get; }
    }

    /// <summary>
    /// Implements a top-N leaderboard in the context of a constantly-incrementing corpus of tracked entities.
    /// </summary>
    /// <typeparam name="T">The type of key we're keeping counts of.</typeparam>
    public class Leaderboard<T> : ILeaderboard<T>
        where T : IComparable<T>
    {
        private readonly ConcurrentDictionary<T, Entry> _allEntries = new();
        private readonly int _maxCount;
        private List<Entry> _leaderboard;
        private readonly object _leaderboardLock = new();
        private int _totalTweets;

        public Leaderboard(int maxCount)
        {
            _maxCount = maxCount;
            _leaderboard = new List<Entry>(_maxCount);
        }

        public int TotalTweets => _totalTweets;

        /// <summary>
        /// The current leaderboard results
        /// </summary>
        public ImmutableList<IResult<T>> Results { get; private set; } = ImmutableList<IResult<T>>.Empty;

        public IResults<T> GetResults()
        {
            return new ResultDto(_totalTweets, Results);
        }

        public void TallyTweet()
        {
            Interlocked.Increment(ref _totalTweets);
        }

        public void TallyHashtag(T key)
        {
            var newCount = _allEntries.AddOrUpdate(key,
                _ => new Entry(key),
                (_, i) => i.Increment());
            UpdateLeaderboard(newCount);
        }

        /// <summary>
        /// Updates the leaderboard in a (hopefully!) thread-safe manner
        /// </summary>
        /// <param name="entry">The entry that's just been updated</param>
        /// <exception cref="ApplicationException">Thrown if the Monitor on the lock object is unavailable within a second</exception>
        private void UpdateLeaderboard(Entry entry)
        {
            try
            {
                if (!Monitor.TryEnter(_leaderboardLock, 1000))
                    // if this flames out, something's stuck or we're getting inundated with data.
                    throw new ApplicationException("Unable to acquire a timely lock on the leaderboard object");

                if (!_leaderboard.Contains(entry))
                    _leaderboard.Add(entry);

                // NOTE that we're only sorting the top _maxcount entries plus the most-recently updated entry:
                _leaderboard = _leaderboard.Order().Take(_maxCount).ToList();
                Results = _leaderboard.Select(entry1 => new HashtagResult(entry1.Key, entry1.Count)).ToImmutableList<IResult<T>>();
            }
            finally
            {
                Monitor.Exit(_leaderboardLock);
            }
        }

        public record ResultDto(int TotalTweets, ImmutableList<IResult<T>> Results) : IResults<T>;

        /// <summary>
        /// A DTO for the output results
        /// </summary>
        /// <param name="Value">The value we're maintaining a census of</param>
        /// <param name="Count">The current tally of this value</param>
        public record HashtagResult(T Value, int Count) : IResult<T>;

        /// <summary>
        /// An inner class that represents a candidate entry on the leaderboard.
        /// </summary>
        public class Entry : IComparable<Entry> // public only for testability
        {
            private int _count;

            public Entry(T key) : this(key, 1) { }
            public Entry(T key, int initialCount)
            {
                Key = key;
                _count = initialCount;
            }

            public T Key { get; init; }
            public int Count => _count;

            public Entry Increment()
            {
                Interlocked.Add(ref _count, 1);
                return this;
            }

            public int CompareTo(Entry? other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (other == null) return 1;

                // Sort descending by count
                var ret = -Count.CompareTo(other.Count);
                return ret == 0
                    ? Key.CompareTo(other.Key) // sort ascending alphabetic as a tiebreaker
                    : ret;
            }

            public static bool operator <(Entry? left, Entry? right) => left == null || left.CompareTo(right) < 0;

            public static bool operator >(Entry? left, Entry? right) => left != null && left.CompareTo(right) > 0;
        }
    }
}
