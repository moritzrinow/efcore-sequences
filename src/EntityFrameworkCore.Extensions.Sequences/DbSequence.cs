namespace EntityFrameworkCore.Extensions.Sequences
{
  /// <summary>
  /// Represents data used to create a sequence.
  /// </summary>
  public class DbSequence<T>
    where T : struct
  {
    /// <summary>
    /// The sequence name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The sequence schema name.
    /// </summary>
    public string Schema { get; set; }

    /// <summary>
    /// The sequence owner.
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// The sequence start value.
    /// </summary>
    public T? StartValue { get; set; }

    /// <summary>
    /// The sequence minimum value.
    /// </summary>
    public T? MinValue { get; set; }

    /// <summary>
    /// The sequence maximum value.
    /// </summary>
    public T? MaxValue { get; set; }

    /// <summary>
    /// The sequence increment by.
    /// </summary>
    public T? IncrementBy { get; set; }

    /// <summary>
    /// Determines whether this sequence is cycling.
    /// </summary>
    public bool Cycle { get; set; }

    /// <summary>
    /// The sequence cache size.
    /// </summary>
    public long? CacheSize { get; set; }
  }
}