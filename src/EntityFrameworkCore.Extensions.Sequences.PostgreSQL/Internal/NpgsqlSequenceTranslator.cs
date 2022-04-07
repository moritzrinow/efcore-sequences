namespace EntityFrameworkCore.Extensions.Sequences.PostgreSQL.Internal
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Query;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;
    using DbFunctionsExtensions = EntityFrameworkCore.Extensions.Sequences.DbFunctionsExtensions;

    internal class NpgsqlSequenceTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo MethodLong = typeof(DbFunctionsExtensions)
          .GetMethod(nameof(DbFunctionsExtensions.NextValLong), new[] { typeof(DbFunctions), typeof(string) });

        private readonly NpgsqlSqlExpressionFactory factory;

        public NpgsqlSequenceTranslator([NotNull] NpgsqlSqlExpressionFactory factory)
        {
            this.factory = factory;
        }

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {

            if (method.Equals(NpgsqlSequenceTranslator.MethodLong))
            {
                return this.factory.Function("nextval",
                                             arguments.Skip(1),
                                             method.ReturnType);
            }

            return null;
        }

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (method.Equals(NpgsqlSequenceTranslator.MethodLong))
            {
                return this.factory.Function("nextval",
                                             arguments.Skip(1),
                                             method.ReturnType);
            }

            return null;
        }
    }
}