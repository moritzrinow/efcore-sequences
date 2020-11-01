namespace EntityFrameworkCore.Extensions.Sequences.Test.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  public class TestSuiteService : IHostedService
  {
    private readonly IReadOnlyList<ITestService> tests;
    private readonly ILogger<TestSuiteService> logger;
    private readonly CancellationTokenSource cancellationTokenSource;
    
    public TestSuiteService(
      ILogger<TestSuiteService> logger,
      IEnumerable<ITestService> tests)
    {
      this.tests = tests.ToList();
      this.logger = logger;
      this.cancellationTokenSource = new CancellationTokenSource();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
      this.logger.LogInformation("Starting test suite...");
      
      var syncTests = this.tests.Where(test => !test.IsAsync);
      var asyncTests = this.tests.Where(test => test.IsAsync);

      await Task.Run(() =>
      {
        foreach (var test in syncTests)
        {
          test.Run();
        }
      }, this.cancellationTokenSource.Token);
      
      await Task.WhenAll(asyncTests.Select(test => test.RunAsync(this.cancellationTokenSource.Token)));

      await this.StopAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      this.logger.LogInformation("Stopping test suite...");
      
      this.cancellationTokenSource.Cancel();

      return Task.CompletedTask;
    }
  }
}