namespace EntityFrameworkCore.Extensions.Sequences.SqlServer
{
  using EntityFrameworkCore.Extensions.Sequences.SqlServer.Internal;
  using Microsoft.EntityFrameworkCore.Infrastructure;

  public static class SqlServerDbContextOptionsBuilderExtensions
  {
    /// <summary>
    /// Adds SqlServer sequence providers and therefore enables operations on sequences.
    /// </summary>
    /// <param name="sqlServerOptionsBuilder"></param>
    /// <returns></returns>
    public static SqlServerDbContextOptionsBuilder UseSequences(this SqlServerDbContextOptionsBuilder sqlServerOptionsBuilder)
    {
      var infra = (IRelationalDbContextOptionsBuilderInfrastructure) sqlServerOptionsBuilder;
      var builder = (IDbContextOptionsBuilderInfrastructure) infra.OptionsBuilder;

      var extension = infra.OptionsBuilder.Options.FindExtension<SqlServerDbContextOptionsExtension>() ??
                      new SqlServerDbContextOptionsExtension();

      builder.AddOrUpdateExtension(extension);

      return sqlServerOptionsBuilder;
    }
  }
}