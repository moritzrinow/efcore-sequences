namespace EntityFrameworkCore.Extensions.Sequences.PostgreSQL.Internal
{
  using System;
  using System.Data;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using EntityFrameworkCore.Extensions.Sequences.Internal;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Infrastructure;
  using Npgsql;

  internal class NpgsqlSequenceProvider : ISequenceProvider
  {
    private readonly DbContext context;

    public NpgsqlSequenceProvider(ICurrentDbContext context)
    {
      this.context = context.Context;
    }

    public void ConfigureModel(ModelBuilder builder)
    {
      builder.Entity<NpgsqlSequence<short>>(entity => entity.HasNoKey());
      builder.Entity<NpgsqlSequence<int>>(entity => entity.HasNoKey());
      builder.Entity<NpgsqlSequence<long>>(entity => entity.HasNoKey());
      builder.Entity<NpgsqlSequence<decimal>>(entity => entity.HasNoKey());
    }

    public IQueryable<IDbSequence<T>> Query<T>() where T : struct
    {
      var set = this.context.Set<NpgsqlSequence<T>>();

      var sql = this.GetSqlQuery<T>();

      return set.FromSqlRaw(sql);
    }

    public string GetSchema()
    {
      var annotation = this.context.Model
                           .GetAnnotations()
                           .FirstOrDefault(p => p.Name == "Relational:Schema");

      return annotation == null ? "public" : annotation.Value.ToString();
    }

    public T GetNextSequenceValue<T>(string name) where T : struct
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        connection.Open();
      }

      string dbTypeString = "bigint";
      SqlDbType dbType;
      
      if (typeof(T) == typeof(short))
      {
        dbType = SqlDbType.SmallInt;
        dbTypeString = NpgsqlSequenceDataTypes.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        dbType = SqlDbType.Int;
        dbTypeString = NpgsqlSequenceDataTypes.Integer;
      }
      else if (typeof(T) == typeof(long))
      {
        dbType = SqlDbType.BigInt;
        dbTypeString = NpgsqlSequenceDataTypes.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        dbType = SqlDbType.Decimal;
        dbTypeString = NpgsqlSequenceDataTypes.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }

      var result = new NpgsqlParameter("result", dbType)
      {
        Direction = ParameterDirection.Output
      };

      var sql =
        $"SELECT CAST(nextval('{name}') AS {dbTypeString}) AS result";

      var res = this.context.Database.ExecuteSqlRaw(sql, new object[] {result});

      return (T) result.Value;
    }

    public bool CreateSequence<T>(DbSequence<T> sequence) where T : struct
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        connection.Open();
      }

      var name = sequence.Schema != null ? $"{sequence.Schema}.{sequence.Name}" : sequence.Name;
      var start = sequence.StartValue != null ? $"START WITH {sequence.StartValue} " : "";
      var increment = sequence.IncrementBy != null ? $"INCREMENT BY {sequence.IncrementBy} " : "";
      var min = sequence.MinValue != null ? $"MINVALUE {sequence.MinValue} " : "NO MINVALUE ";
      var max = sequence.MaxValue != null ? $"MAXVALUE {sequence.MaxValue} " : "NO MAXVALUE ";
      var cycle = sequence.Cycle ? "CYCLE " : "NO CYCLE ";
      var cache = sequence.CacheSize != null ? $"CACHE {sequence.CacheSize} " : null;
      var owner = sequence.Owner != null ? $"OWNED BY {sequence.Owner} " : "OWNED BY NONE";

      string type;
      
      if (typeof(T) == typeof(short))
      {
        type = NpgsqlSequenceDataTypes.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        type = NpgsqlSequenceDataTypes.Integer;
      }
      else if (typeof(T) == typeof(long))
      {
        type = NpgsqlSequenceDataTypes.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        type = NpgsqlSequenceDataTypes.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }
      
      var sql =
        $"CREATE SEQUENCE {name} " +
        $"{start}" +
        $"{increment}" +
        $"{min}" +
        $"{max}" +
        $"{cycle}" +
        $"{cache}" +
        $"{owner}";

      var result = this.context.Database.ExecuteSqlRaw(sql);

      return result == -1;
    }

    public bool UpdateSequence<T>(string name, DbSequenceUpdate<T> update) where T : struct
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        connection.Open();
      }

      var restart = update.RestartWith != null ? $"RESTART WITH {update.RestartWith}" : "";
      var increment = update.IncrementBy != null ? $"INCREMENT BY {update.IncrementBy}" : "";
      var min = update.MinValue != null ? $"MINVALUE {update.MinValue}" : "";
      var max = update.MaxValue != null ? $"MAXVALUE {update.MaxValue}" : "";
      var cycle = update.Cycle != null ? (update.Cycle.Value ? "CYCLE" : "NO CYCLE") : "";
      var cache = update.CacheSize != null ? (update.CacheSize.Value > 0 ? $"CACHE {update.CacheSize}" : "NO CACHE") : "";

      var sql =
        $"ALTER SEQUENCE {name} " +
        $"{restart} " +
        $"{increment}" +
        $"{min} " +
        $"{max}" +
        $"{cycle}" +
        $"{cache}";

      var result = this.context.Database.ExecuteSqlRaw(sql);

      return result == -1;
    }

    public bool DropSequence(string name, bool conditionally)
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        connection.Open();
      }

      var sql =
        $"DROP SEQUENCE " +
        (conditionally ? $"IF EXISTS " : "") +
        $"{name}";

      var result = this.context.Database.ExecuteSqlRaw(sql);

      return result == -1;
    }

    public async Task<T> GetNextSequenceValueAsync<T>(string name, CancellationToken ct = default)
      where T : struct
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        await connection.OpenAsync(ct);
      }

      string dbTypeString = "bigint";
      SqlDbType dbType;
      
      if (typeof(T) == typeof(short))
      {
        dbType = SqlDbType.SmallInt;
        dbTypeString = NpgsqlSequenceDataTypes.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        dbType = SqlDbType.Int;
        dbTypeString = NpgsqlSequenceDataTypes.Integer;
      }
      else if (typeof(T) == typeof(long))
      {
        dbType = SqlDbType.BigInt;
        dbTypeString = NpgsqlSequenceDataTypes.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        dbType = SqlDbType.Decimal;
        dbTypeString = NpgsqlSequenceDataTypes.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }

      var result = new NpgsqlParameter("result", dbType)
      {
        Direction = ParameterDirection.Output
      };

      var sql =
        $"SELECT CAST(nextval('{name}') AS {dbTypeString}) AS result";

      var res = await this.context.Database.ExecuteSqlRawAsync(sql, new object[] {result}, ct);

      return (T) result.Value;
    }

    public async Task<bool> UpdateSequenceAsync<T>(string name, DbSequenceUpdate<T> update, CancellationToken ct = default)
      where T : struct
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        await connection.OpenAsync(ct);
      }

      var restart = update.RestartWith != null ? $"RESTART WITH {update.RestartWith}" : "";
      var increment = update.IncrementBy != null ? $"INCREMENT BY {update.IncrementBy}" : "";
      var min = update.MinValue != null ? $"MINVALUE {update.MinValue}" : "";
      var max = update.MaxValue != null ? $"MAXVALUE {update.MaxValue}" : "";
      var cycle = update.Cycle != null ? (update.Cycle.Value ? "CYCLE" : "NO CYCLE") : "";
      var cache = update.CacheSize != null ? (update.CacheSize.Value > 0 ? $"CACHE {update.CacheSize}" : "NO CACHE") : "";

      var sql =
        $"ALTER SEQUENCE {name} " +
        $"{restart} " +
        $"{increment}" +
        $"{min} " +
        $"{max}" +
        $"{cycle}" +
        $"{cache}";

      var result = await this.context.Database.ExecuteSqlRawAsync(sql, ct);

      return result == -1;
    }

    public async Task<bool> CreateSequenceAsync<T>(DbSequence<T> sequence, CancellationToken ct = default)
      where T : struct
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        await connection.OpenAsync(ct);
      }

      var name = sequence.Schema != null ? $"{sequence.Schema}.{sequence.Name}" : sequence.Name;
      var start = sequence.StartValue != null ? $"START WITH {sequence.StartValue} " : "";
      var increment = sequence.IncrementBy != null ? $"INCREMENT BY {sequence.IncrementBy} " : "";
      var min = sequence.MinValue != null ? $"MINVALUE {sequence.MinValue} " : "NO MINVALUE ";
      var max = sequence.MaxValue != null ? $"MAXVALUE {sequence.MaxValue} " : "NO MAXVALUE ";
      var cycle = sequence.Cycle ? "CYCLE " : "NO CYCLE ";
      var cache = sequence.CacheSize != null ? $"CACHE {sequence.CacheSize} " : null;
      var owner = sequence.Owner != null ? $"OWNED BY {sequence.Owner} " : "OWNED BY NONE";

      string type;
      
      if (typeof(T) == typeof(short))
      {
        type = NpgsqlSequenceDataTypes.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        type = NpgsqlSequenceDataTypes.Integer;
      }
      else if (typeof(T) == typeof(long))
      {
        type = NpgsqlSequenceDataTypes.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        type = NpgsqlSequenceDataTypes.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }
      
      var sql =
        $"CREATE SEQUENCE {name} " +
        $"{start}" +
        $"{increment}" +
        $"{min}" +
        $"{max}" +
        $"{cycle}" +
        $"{cache}" +
        $"{owner}";

      var result = await this.context.Database.ExecuteSqlRawAsync(sql, ct);

      return result == -1;
    }

    public async Task<bool> DropSequenceAsync(string name, bool conditionally, CancellationToken ct = default)
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        await connection.OpenAsync(ct);
      }

      var sql =
        $"DROP SEQUENCE " +
        (conditionally ? $"IF EXISTS " : "") +
        $"{name}";

      var result = await this.context.Database.ExecuteSqlRawAsync(sql, ct);

      return result == -1;
    }
    
    private string GetSqlQuery<T>()
      where T : struct
    {
      string castType;

      if (typeof(T) == typeof(short))
      {
        castType = NpgsqlSequenceDataTypes.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        castType = NpgsqlSequenceDataTypes.Integer;
      }
      else if (typeof(T) == typeof(long))
      {
        castType = NpgsqlSequenceDataTypes.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        castType = NpgsqlSequenceDataTypes.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }

      return "SELECT sequencename AS name," +
             "schemaname AS schema," +
             "sequenceowner AS owner," +
             "'bigint' AS type," +
             $"CAST(start_value AS {castType}) AS start_value," +
             $"CAST(min_value AS {castType}) AS min_value," +
             $"CAST(max_value AS {castType}) AS max_value," +
             $"CAST(increment_by AS {castType}) AS increment_by," +
             "cycle AS cycle," +
             "cache_size AS cache_size," +
             $"CASE WHEN last_value IS NULL THEN CAST(start_value AS {castType}) ELSE CAST(last_value AS {castType}) END AS current_value " +
             "FROM pg_sequences";
    }
  }
}