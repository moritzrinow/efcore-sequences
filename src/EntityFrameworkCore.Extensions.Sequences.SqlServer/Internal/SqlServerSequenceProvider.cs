namespace EntityFrameworkCore.Extensions.Sequences.SqlServer.Internal
{
  using System;
  using System.Data;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using EntityFrameworkCore.Extensions.Sequences.Internal;
  using Microsoft.Data.SqlClient;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Infrastructure;

  internal class SqlServerSequenceProvider : ISequenceProvider
  {
    private readonly DbContext context;

    public SqlServerSequenceProvider(ICurrentDbContext context)
    {
      this.context = context.Context;
    }

    public void ConfigureModel(ModelBuilder builder)
    {
      builder.Entity<SqlServerSequence<byte>>(entity => entity.HasNoKey());
      builder.Entity<SqlServerSequence<short>>(entity => entity.HasNoKey());
      builder.Entity<SqlServerSequence<int>>(entity => entity.HasNoKey());
      builder.Entity<SqlServerSequence<long>>(entity => entity.HasNoKey());
      builder.Entity<SqlServerSequence<decimal>>(entity => entity.HasNoKey());
    }

    public IQueryable<IDbSequence<T>> Query<T>() where T : struct
    {
      var set = this.context.Set<SqlServerSequence<T>>();

      var sql = this.GetSqlQuery<T>();

      return set.FromSqlRaw(sql);
    }

    public string GetSchema()
    {
      var annotation = this.context.Model
                           .GetAnnotations()
                           .FirstOrDefault(p => p.Name == "Relational:Schema");

      return annotation == null ? "dbo" : annotation.Value.ToString();
    }

    public T GetNextSequenceValue<T>(string name) where T : struct
    {
      var connection = this.context.Database.GetDbConnection();

      if (connection.State != ConnectionState.Open)
      {
        connection.Open();
      }

      SqlDbType dbType;

      if (typeof(T) == typeof(byte))
      {
        dbType = SqlDbType.TinyInt;
      }
      else if (typeof(T) == typeof(short))
      {
        dbType = SqlDbType.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        dbType = SqlDbType.Int;
      }
      else if (typeof(T) == typeof(long))
      {
        dbType = SqlDbType.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        dbType = SqlDbType.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }
      
      var result = new SqlParameter("@result", dbType)
      {
        Direction = ParameterDirection.Output
      };

      var sql =
        $"SELECT @result = (NEXT VALUE FOR [{name}])";

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

      var name = sequence.Schema != null ? $"[{sequence.Schema}].[{sequence.Name}] " : $"[{sequence.Name}]";
      var start = sequence.StartValue != null ? $"START WITH {sequence.StartValue} " : "";
      var increment = sequence.IncrementBy != null ? $"INCREMENT BY {sequence.IncrementBy} " : "";
      var min = sequence.MinValue != null ? $"MINVALUE {sequence.MinValue} " : "NO MINVALUE ";
      var max = sequence.MaxValue != null ? $"MAXVALUE {sequence.MaxValue} " : "NO MAXVALUE ";
      var cycle = sequence.Cycle ? "CYCLE " : "NO CYCLE ";
      var cache = sequence.CacheSize != null ? $"CACHE {sequence.CacheSize} " : "NO CACHE ";

      string type;
      
      if (typeof(T) == typeof(byte))
      {
        type = SqlServerSequenceDataTypes.TinyInt;
      }
      else if (typeof(T) == typeof(short))
      {
        type = SqlServerSequenceDataTypes.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        type = SqlServerSequenceDataTypes.Int;
      }
      else if (typeof(T) == typeof(long))
      {
        type = SqlServerSequenceDataTypes.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        type = SqlServerSequenceDataTypes.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }
      
      var sql =
        $"CREATE SEQUENCE {name} " +
        $"AS {type} " +
        $"{start}" +
        $"{increment}" +
        $"{min}" +
        $"{max}" +
        $"{cycle}" +
        $"{cache}";

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
        $"ALTER SEQUENCE [{name}] " +
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
        $"[{name}]";

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

      SqlDbType dbType;

      if (typeof(T) == typeof(byte))
      {
        dbType = SqlDbType.TinyInt;
      }
      else if (typeof(T) == typeof(short))
      {
        dbType = SqlDbType.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        dbType = SqlDbType.Int;
      }
      else if (typeof(T) == typeof(long))
      {
        dbType = SqlDbType.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        dbType = SqlDbType.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }
      
      var result = new SqlParameter("@result", dbType)
      {
        Direction = ParameterDirection.Output
      };

      var sql =
        $"SELECT @result = (NEXT VALUE FOR [{name}])";

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
        $"ALTER SEQUENCE [{name}] " +
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

      var name = sequence.Schema != null ? $"{sequence.Schema}.{sequence.Name} " : sequence.Name;
      var start = sequence.StartValue != null ? $"START WITH {sequence.StartValue} " : "";
      var increment = sequence.IncrementBy != null ? $"INCREMENT BY {sequence.IncrementBy} " : "";
      var min = sequence.MinValue != null ? $"MINVALUE {sequence.MinValue} " : "NO MINVALUE ";
      var max = sequence.MaxValue != null ? $"MAXVALUE {sequence.MaxValue} " : "NO MAXVALUE ";
      var cycle = sequence.Cycle ? "CYCLE " : "NO CYCLE ";
      var cache = sequence.CacheSize != null ? $"CACHE {sequence.CacheSize} " : "NO CACHE ";

      string type;
      
      if (typeof(T) == typeof(byte))
      {
        type = SqlServerSequenceDataTypes.TinyInt;
      }
      else if (typeof(T) == typeof(short))
      {
        type = SqlServerSequenceDataTypes.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        type = SqlServerSequenceDataTypes.Int;
      }
      else if (typeof(T) == typeof(long))
      {
        type = SqlServerSequenceDataTypes.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        type = SqlServerSequenceDataTypes.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }
      
      var sql =
        $"CREATE SEQUENCE {name} " +
        $"AS {type} " +
        $"{start}" +
        $"{increment}" +
        $"{min}" +
        $"{max}" +
        $"{cycle}" +
        $"{cache}";

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

      if (typeof(T) == typeof(byte))
      {
        castType = SqlServerSequenceDataTypes.TinyInt;
      }
      else if (typeof(T) == typeof(short))
      {
        castType = SqlServerSequenceDataTypes.SmallInt;
      }
      else if (typeof(T) == typeof(int))
      {
        castType = SqlServerSequenceDataTypes.Int;
      }
      else if (typeof(T) == typeof(long))
      {
        castType = SqlServerSequenceDataTypes.BigInt;
      }
      else if (typeof(T) == typeof(decimal))
      {
        castType = SqlServerSequenceDataTypes.Decimal;
      }
      else
      {
        throw new NotSupportedException($"Numeric type '{typeof(T).Name}' not supported");
      }
      
      return "SELECT name," +
             "TYPE_NAME(system_type_id) AS type," +
             "SCHEMA_NAME(schema_id) AS schema_name," +
             "USER_NAME(principal_id) AS owner," +
             $"CAST(current_value AS {castType}) AS current_value," +
             $"CAST(start_value AS {castType}) AS start_value," +
             $"CAST(minimum_value AS {castType}) AS minimum_value," +
             $"CAST(maximum_value AS {castType}) AS maximum_value," +
             $"CAST(increment AS {castType}) AS increment," +
             "is_cycling," +
             "cache_size" +
             " " +
             "FROM sys.sequences";
    }
  }
}