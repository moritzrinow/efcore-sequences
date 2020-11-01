namespace EntityFrameworkCore.Extensions.Sequences.SqlServer.Internal
{
  using Microsoft.EntityFrameworkCore.Query;
  using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

  internal sealed class SqlServerSequenceMethodCallTranslatorProvider : SqlServerMethodCallTranslatorProvider
  {
    public SqlServerSequenceMethodCallTranslatorProvider(RelationalMethodCallTranslatorProviderDependencies dependencies)
      : base(dependencies)
    {
      this.AddTranslators(new[]
      {
        new SqlServerSequenceTranslator(dependencies.SqlExpressionFactory),
      });
    }
  }
}