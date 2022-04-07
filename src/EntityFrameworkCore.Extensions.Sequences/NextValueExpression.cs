namespace EntityFrameworkCore.Extensions.Sequences
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using Microsoft.EntityFrameworkCore.Query;
  using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
  using Microsoft.EntityFrameworkCore.Storage;

  public class NextValueExpression : SqlExpression
  {
    public NextValueExpression(
      Expression instance,
      IEnumerable<SqlExpression> arguments,
      Type type,
      RelationalTypeMapping typeMapping)
      : base(type, typeMapping)
    {
      this.Instance = instance;

      this.Arguments = (arguments ?? Enumerable.Empty<SqlExpression>()).ToList();
    }

    public virtual string Function => "NEXT VALUE FOR ";
    public virtual Expression Instance { get; }
    public virtual IReadOnlyList<SqlExpression> Arguments { get; }

    protected override Expression Accept(ExpressionVisitor visitor)
    {
      visitor.Visit(new SqlFragmentExpression(this.Function));

      foreach (var arg in this.Arguments)
      {
        visitor.Visit(arg);
      }

      return this;
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
      var changed = false;
      var instance = (SqlExpression)visitor.Visit(this.Instance);
      changed |= instance != this.Instance;
      var arguments = new SqlExpression[this.Arguments.Count];
      for (var i = 0; i < arguments.Length; i++)
      {
        arguments[i] = (SqlExpression)visitor.Visit(Arguments[i]);
        changed |= arguments[i] != Arguments[i];
      }

      return changed ? new NextValueExpression(instance, arguments, this.Type, this.TypeMapping) : this;
    }
    
    public virtual NextValueExpression ApplyTypeMapping(RelationalTypeMapping typeMapping)
      => new NextValueExpression(
        Instance,
        Arguments,
        Type,
        typeMapping ?? TypeMapping);
    
    public virtual NextValueExpression Update(SqlExpression instance, IReadOnlyList<SqlExpression> arguments)
      => instance != Instance || !arguments.SequenceEqual(Arguments)
           ? new NextValueExpression(instance, arguments, Type, TypeMapping)
           : this;

    protected override void Print(ExpressionPrinter expressionPrinter)
    {
      expressionPrinter.Append(this.Function);
      expressionPrinter.VisitCollection(this.Arguments);
    }
    
    public override bool Equals(object obj)
      => obj != null
         && (ReferenceEquals(this, obj)
             || obj is NextValueExpression nextValueExpression
             && Equals(nextValueExpression));
    
    private bool Equals(NextValueExpression nextValueExpression)
      => base.Equals(nextValueExpression)
         && string.Equals(this.Function, nextValueExpression.Function)
         && ((Instance == null && nextValueExpression.Instance == null)
             || (Instance != null && Instance.Equals(nextValueExpression.Instance)))
         && Arguments.SequenceEqual(nextValueExpression.Arguments);
    
    public override int GetHashCode()
    {
      var hash = new HashCode();
      hash.Add(base.GetHashCode());
      hash.Add(Function);
      hash.Add(Instance);
      for (var i = 0; i < Arguments.Count; i++)
      {
        hash.Add(Arguments[i]);
      }

      return hash.ToHashCode();
    }
  }
}