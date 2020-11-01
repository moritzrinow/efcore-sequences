namespace EntityFrameworkCore.Extensions.Sequences
{
  using System;
  using System.Linq;
  using System.Reflection;
  using EntityFrameworkCore.Extensions.Sequences.Internal;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
  using Microsoft.EntityFrameworkCore.Infrastructure;
  using Microsoft.EntityFrameworkCore.Query;
  using Microsoft.EntityFrameworkCore.Query.Internal;

  public static class QueryableExtensions
  {
    /// <summary>
    /// Applies a filter for the default schema to the query.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<IDbSequence<T>> DefaultSchema<T>(this IQueryable<IDbSequence<T>> query)
      where T : struct
    {
      var context = query.GetDbContext();

      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var schema = provider.GetSchema();

      query = query.Where(p => p.Schema == schema);

      return query;
    }

    internal static DbContext GetDbContext(this IQueryable query)
    {
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags).GetValue(query.Provider);
      var queryContextFactory = queryCompiler.GetType().GetField("_queryContextFactory", bindingFlags).GetValue(queryCompiler);

      var dependencies = typeof(RelationalQueryContextFactory).GetField("_dependencies", bindingFlags).GetValue(queryContextFactory);
      var queryContextDependencies = typeof(DbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName);
      var stateManagerProperty = queryContextDependencies.GetProperty("StateManager", bindingFlags | BindingFlags.Public).GetValue(dependencies);
      var stateManager = (IStateManager) stateManagerProperty;

      return stateManager.Context;
    }
  }
}