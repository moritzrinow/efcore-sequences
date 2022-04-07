namespace EntityFrameworkCore.Extensions.Sequences.SqlServer.Internal
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
    using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
    using Microsoft.EntityFrameworkCore.Storage;
    using DbFunctionsExtensions = EntityFrameworkCore.Extensions.Sequences.DbFunctionsExtensions;

    internal class SqlServerSequenceTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo MethodByte = typeof(DbFunctionsExtensions)
          .GetMethod(nameof(DbFunctionsExtensions.NextValByte), new[] { typeof(DbFunctions), typeof(string) });

        private static readonly MethodInfo MethodShort = typeof(DbFunctionsExtensions)
          .GetMethod(nameof(DbFunctionsExtensions.NextValShort), new[] { typeof(DbFunctions), typeof(string) });

        private static readonly MethodInfo MethodInt = typeof(DbFunctionsExtensions)
          .GetMethod(nameof(DbFunctionsExtensions.NextValInt), new[] { typeof(DbFunctions), typeof(string) });

        private static readonly MethodInfo MethodLong = typeof(DbFunctionsExtensions)
          .GetMethod(nameof(DbFunctionsExtensions.NextValLong), new[] { typeof(DbFunctions), typeof(string) });

        private static readonly MethodInfo MethodDec = typeof(DbFunctionsExtensions)
          .GetMethod(nameof(DbFunctionsExtensions.NextValDec), new[] { typeof(DbFunctions), typeof(string) });

        private readonly ISqlExpressionFactory factory;

        public SqlServerSequenceTranslator([NotNull] ISqlExpressionFactory factory)
        {
            this.factory = factory;
        }

        public virtual SqlExpression Translate(SqlExpression instance,
          MethodInfo method,
          IReadOnlyList<SqlExpression> arguments)
        {
            if (method.Equals(SqlServerSequenceTranslator.MethodByte) ||
                method.Equals(SqlServerSequenceTranslator.MethodShort) ||
                method.Equals(SqlServerSequenceTranslator.MethodInt) ||
                method.Equals(SqlServerSequenceTranslator.MethodLong) ||
                method.Equals(SqlServerSequenceTranslator.MethodDec))
            {
                var typedArguments = new List<SqlExpression>();

                foreach (var arg in arguments.Skip(1))
                {
                    typedArguments.Add(this.factory.ApplyDefaultTypeMapping(arg));
                }

                if (method.ReturnType == typeof(byte))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new SqlServerByteTypeMapping(SqlServerSequenceDataTypes.TinyInt));
                }

                if (method.ReturnType == typeof(short))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new SqlServerShortTypeMapping(SqlServerSequenceDataTypes.SmallInt));
                }

                if (method.ReturnType == typeof(int))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new IntTypeMapping(SqlServerSequenceDataTypes.Int));
                }

                if (method.ReturnType == typeof(long))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new IntTypeMapping(SqlServerSequenceDataTypes.BigInt));
                }

                if (method.ReturnType == typeof(decimal))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new SqlServerDecimalTypeMapping(SqlServerSequenceDataTypes.Decimal));
                }
            }

            return null;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (method.Equals(SqlServerSequenceTranslator.MethodByte) ||
                method.Equals(SqlServerSequenceTranslator.MethodShort) ||
                method.Equals(SqlServerSequenceTranslator.MethodInt) ||
                method.Equals(SqlServerSequenceTranslator.MethodLong) ||
                method.Equals(SqlServerSequenceTranslator.MethodDec))
            {
                var typedArguments = new List<SqlExpression>();

                foreach (var arg in arguments.Skip(1))
                {
                    typedArguments.Add(this.factory.ApplyDefaultTypeMapping(arg));
                }

                if (method.ReturnType == typeof(byte))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new SqlServerByteTypeMapping(SqlServerSequenceDataTypes.TinyInt));
                }

                if (method.ReturnType == typeof(short))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new SqlServerShortTypeMapping(SqlServerSequenceDataTypes.SmallInt));
                }

                if (method.ReturnType == typeof(int))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new IntTypeMapping(SqlServerSequenceDataTypes.Int));
                }

                if (method.ReturnType == typeof(long))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new IntTypeMapping(SqlServerSequenceDataTypes.BigInt));
                }

                if (method.ReturnType == typeof(decimal))
                {
                    return new NextValueExpression(
                      this.factory.ApplyDefaultTypeMapping(instance),
                      typedArguments,
                      method.ReturnType,
                      new SqlServerDecimalTypeMapping(SqlServerSequenceDataTypes.Decimal));
                }
            }

            return null;
        }
    }
}