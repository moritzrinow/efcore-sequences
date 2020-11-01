namespace EntityFrameworkCore.Extensions.Sequences.SqlServer.Internal
{
  using System.Collections.Generic;
  using EntityFrameworkCore.Extensions.Sequences.Internal;
  using Microsoft.EntityFrameworkCore.Infrastructure;
  using Microsoft.EntityFrameworkCore.Query;
  using Microsoft.Extensions.DependencyInjection;

  internal class SqlServerDbContextOptionsExtension : IDbContextOptionsExtension
  {
    public void ApplyServices(IServiceCollection services)
    {
      services.AddTransient<ISequenceProvider, SqlServerSequenceProvider>();
      services.AddSingleton<IMethodCallTranslatorProvider, SqlServerSequenceMethodCallTranslatorProvider>();
    }

    public void Validate(IDbContextOptions options)
    {
    }

    public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

    private class ExtensionInfo : DbContextOptionsExtensionInfo
    {
      public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
      {
      }

      public override bool IsDatabaseProvider => false;

      public override string LogFragment => "'SqlServerSequenceSupport'=true";

      public override long GetServiceProviderHashCode()
      {
        return 0L;
      }

      public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
      {
      }
    }
  }
}