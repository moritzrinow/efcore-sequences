namespace EntityFrameworkCore.Extensions.Sequences
{
  /// <summary>
  /// Represents data used to update a sequence.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DbSequenceUpdate<T>
    where T : struct
  {
    /// <summary>
    /// If set, the sequence will be restarted at the specified value.
    /// </summary>
    public T? RestartWith { get; set; }

    /// <summary>
    /// If set, the sequence will use the specified value as increment by.
    /// </summary>
    public T? IncrementBy { get; set; }

    /// <summary>
    /// If set, the sequence will use the specified value as the minimum value.
    /// </summary>
    public T? MinValue { get; set; }

    /// <summary>
    /// If specified, the sequence will use the specified value as the maximum value.
    /// </summary>
    public T? MaxValue { get; set; }

    /// <summary>
    /// If specified, the sequence will cycle depending on the value.
    /// </summary>
    public bool? Cycle { get; set; }

    /// <summary>
    /// If specified, the sequence will use the specified value as the cache size.
    /// </summary>
    public long? CacheSize { get; set; }
  }
}