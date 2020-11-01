namespace EntityFrameworkCore.Extensions.Sequences
{
  /// <summary>
  /// Represents a database sequence of type <see cref="T"/>.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IDbSequence<out T>
    where T : struct
  {
    /// <summary>
    /// The sequence name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The sequence data type name.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// The sequence schema name.
    /// </summary>
    string Schema { get; }

    /// <summary>
    /// The sequence owner name.
    /// </summary>
    string Owner { get; }

    /// <summary>
    /// The current value of the sequence.
    /// </summary>
    T CurrentValue { get; }

    /// <summary>
    /// The sequence start value.
    /// </summary>
    T StartValue { get; }

    /// <summary>
    /// The sequence minimum value.
    /// </summary>
    T MinValue { get; }

    /// <summary>
    /// The sequence maximum value.
    /// </summary>
    T MaxValue { get; }

    /// <summary>
    /// The sequence increment by.
    /// </summary>
    T IncrementBy { get; }

    /// <summary>
    /// Determines whether this sequence is cycling.
    /// </summary>
    bool Cycle { get; }

    /// <summary>
    /// The sequence cache size.
    /// </summary>
    long? CacheSize { get; }
  }
}