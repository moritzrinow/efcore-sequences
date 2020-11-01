namespace EntityFrameworkCore.Extensions.Sequences.Test.Services
{
  using System.Threading;
  using System.Threading.Tasks;

  public interface ITestService
  {
    string Id { get; }
    
    bool IsAsync { get; }
    
    void Run();

    Task RunAsync(CancellationToken cancellationToken);
  }
}