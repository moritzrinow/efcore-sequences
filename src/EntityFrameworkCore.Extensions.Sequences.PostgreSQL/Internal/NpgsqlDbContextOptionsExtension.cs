namespace EntityFrameworkCore.Extensions.Sequences.PostgreSQL.Internal
{
  using System.Collections.Generic;
  using EntityFrameworkCore.Extensions.Sequences.Internal;
  using Microsoft.EntityFrameworkCore.Infrastructure;
  using Microsoft.EntityFrameworkCore.Query;
  using Microsoft.Extensions.DependencyInjection;

  internal class NpgsqlDbContextOptionsExtension : IDbContextOptionsExtension
  {
    public void ApplyServices(IServiceCollection services)
    {
      services.AddTransient<ISequenceProvider, NpgsqlSequenceProvider>();
      services.AddSingleton<IMethodCallTranslatorProvider, NpgsqlSequenceMethodCallTranslatorProvider>();
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

      public override string LogFragment => "'NpgsqlSequenceSupport'=true";

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