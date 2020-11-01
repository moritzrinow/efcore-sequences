namespace EntityFrameworkCore.Extensions.Sequences.Test.Database
{
  using System;
  using EntityFrameworkCore.Extensions.Sequences.PostgreSQL;
  using EntityFrameworkCore.Extensions.Sequences.SqlServer;
  using EntityFrameworkCore.Extensions.Sequences.Test.Options;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Options;

  public class SequenceContext : DbContext
  {
    private readonly TestOptions testOptions;

    internal SequenceContext(DbContextOptions<SequenceContext> options)
      : base(options)
    {
    }
    
    public SequenceContext(DbContextOptions<SequenceContext> options, IOptionsSnapshot<TestOptions> testOptions)
      : base(options)
    {
      this.testOptions = testOptions.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.EnableSensitiveDataLogging();

        if (!this.testOptions.Databases.ContainsKey(this.testOptions.CurrentDatabase))
        {
          throw new InvalidOperationException($"DatabaseOptions not found for database '{this.testOptions.CurrentDatabase}'");
        }

        var database = this.testOptions.Databases[this.testOptions.CurrentDatabase];

        switch (database.Provider)
        {
          case "sqlserver":
            optionsBuilder.UseSqlServer(database.ConnectionString, options => options.UseSequences());
            break;

          case "npgsql":
            optionsBuilder.UseNpgsql(database.ConnectionString, options => options.UseSequences());
            break;

          default: throw new InvalidOperationException("Database provider not supported");
        }
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.UseSequences(this);
    }
  }
}