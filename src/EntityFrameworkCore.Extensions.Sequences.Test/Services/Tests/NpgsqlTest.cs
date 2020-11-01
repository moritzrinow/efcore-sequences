namespace EntityFrameworkCore.Extensions.Sequences.Test.Services.Tests
{
  using System;
  using System.Diagnostics;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using EntityFrameworkCore.Extensions.Sequences.PostgreSQL;
  using EntityFrameworkCore.Extensions.Sequences.Test.Database;
  using EntityFrameworkCore.Extensions.Sequences.Test.Options;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;

  public class NpgsqlTest : ITestService
  {
    private readonly TestOptions options;
    private readonly ILogger<NpgsqlTest> logger;
    private readonly ILoggerFactory loggerFactory;
    
    public NpgsqlTest(
      ILogger<NpgsqlTest> logger,
      IOptions<TestOptions> options,
      ILoggerFactory loggerFactory)
    {
      this.logger = logger;
      this.options = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
      this.loggerFactory = loggerFactory;
    }
    
    public string Id => "npgsql-test";
    public bool IsAsync => false;
    public void Run()
    {
      this.logger.LogInformation($"Running synchronous test '{this.Id}'");
      
      if (!this.options.Databases.TryGetValue("npgsql-testdb", out var dbOptions))
      {
        throw new InvalidOperationException("No database options found for 'npgsql-testdb'");
      }

      var builder = new DbContextOptionsBuilder<SequenceContext>()
                    .EnableSensitiveDataLogging()
                    .UseLoggerFactory(this.loggerFactory)
                    .UseNpgsql(dbOptions.ConnectionString, server => server.UseSequences());
      
      using var context = new SequenceContext(builder.Options);
      
      this.Test<long>(context, 1, 1);
      this.Test<decimal>(context, 1, 1);

      this.logger.LogInformation($"Finished running synchronous test '{this.Id}'");
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
      throw new System.NotImplementedException();
    }
    
    private void Test<T>(DbContext context, T start, T increment)
      where T : struct
    {
      var exists = context.Sequences<T>().Any(s => s.Name == "testsequence");

      if (exists)
      {
        var success = context.DropSequence("testsequence");
        Debug.Assert(success);
      }
      
      Debug.Assert(!context.Sequences<T>().Any(p => p.Name == "testsequence"));

      var created = context.CreateSequence(new DbSequence<T>
      {
        Name = "testsequence",
        StartValue = start,
        IncrementBy = increment
      });
      
      Debug.Assert(created);

      var sequence = context.Sequences<T>().SingleOrDefault(p => p.Name == "testsequence");
      Debug.Assert(sequence != null);
      Debug.Assert(sequence.StartValue.Equals(start));
      Debug.Assert(sequence.IncrementBy.Equals(increment));

      var nextValue = context.NextSequenceValue<T>("testsequence");
      Debug.Assert(nextValue.Equals(start));

      var updated = context.UpdateSequence("testsequence", new DbSequenceUpdate<T>
      {
        RestartWith = start
      });
      
      Debug.Assert(updated);

      nextValue = context.NextSequenceValue<T>("testsequence");
      Debug.Assert(nextValue.Equals(start));

      var dropped = context.DropSequence("testsequence");
      Debug.Assert(dropped);

      exists = context.Sequences<T>().Any(p => p.Name == "testsequence");
      Debug.Assert(!exists);
    }
  }
}