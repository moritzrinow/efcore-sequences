namespace EntityFrameworkCore.Extensions.Sequences.PostgreSQL.Internal
{
  using Microsoft.EntityFrameworkCore.Query;
  using Npgsql.EntityFrameworkCore.PostgreSQL.Query;
  using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

  internal sealed class NpgsqlSequenceMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
  {
    public NpgsqlSequenceMethodCallTranslatorProvider(RelationalMethodCallTranslatorProviderDependencies dependencies)
      : base(dependencies)
    {
      this.AddTranslators(new[]
      {
        new NpgsqlSequenceTranslator((NpgsqlSqlExpressionFactory) dependencies.SqlExpressionFactory),
      });
    }
  }
}