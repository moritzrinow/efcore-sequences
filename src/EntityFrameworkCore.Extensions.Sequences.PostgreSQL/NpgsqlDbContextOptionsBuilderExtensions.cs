namespace EntityFrameworkCore.Extensions.Sequences.PostgreSQL
{
  using EntityFrameworkCore.Extensions.Sequences.PostgreSQL.Internal;
  using Microsoft.EntityFrameworkCore.Infrastructure;
  using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

  public static class NpgsqlDbContextOptionsBuilderExtensions
  {
    /// <summary>
    /// Adds PostgreSQL sequence providers and therefore enables operations on sequences.
    /// </summary>
    /// <param name="npgsqlOptionsBuilder"></param>
    /// <returns></returns>
    public static NpgsqlDbContextOptionsBuilder UseSequences(this NpgsqlDbContextOptionsBuilder npgsqlOptionsBuilder)
    {
      var infra = (IRelationalDbContextOptionsBuilderInfrastructure) npgsqlOptionsBuilder;
      var builder = (IDbContextOptionsBuilderInfrastructure) infra.OptionsBuilder;

      var extension = infra.OptionsBuilder.Options.FindExtension<NpgsqlDbContextOptionsExtension>() ??
                      new NpgsqlDbContextOptionsExtension();

      builder.AddOrUpdateExtension(extension);

      return npgsqlOptionsBuilder;
    }
  }
}