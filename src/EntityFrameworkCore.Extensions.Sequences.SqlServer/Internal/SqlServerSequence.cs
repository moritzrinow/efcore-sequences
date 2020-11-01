namespace EntityFrameworkCore.Extensions.Sequences.SqlServer.Internal
{
  using System.ComponentModel.DataAnnotations.Schema;
  using Microsoft.EntityFrameworkCore;

  internal class SqlServerSequence<T> : IDbSequence<T>
    where T : struct
  {
    [Column("name")]
    public string Name { get; set; }

    [Column("type")]
    public string Type { get; set; }

    [Column("schema_name")]
    public string Schema { get; set; }

    [Column("owner")]
    public string Owner { get; set; }

    [Column("current_value")]
    public T CurrentValue { get; set; }

    [Column("start_value")]
    public T StartValue { get; set; }

    [Column("minimum_value")]
    public T MinValue { get; set; }

    [Column("maximum_value")]
    public T MaxValue { get; set; }

    [Column("increment")]
    public T IncrementBy { get; set; }

    [Column("is_cycling")]
    public bool Cycle { get; set; }

    [Column("cache_size")]
    public long? CacheSize { get; set; }
  }
}