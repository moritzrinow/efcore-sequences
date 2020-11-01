namespace EntityFrameworkCore.Extensions.Sequences
{
  using System;

  public static class DbSequenceExtensions
  {
    public static byte NextVal(this IDbSequence<byte> _)
    {
      throw new InvalidOperationException($"Function '{nameof(DbSequenceExtensions.NextVal)}' cannot be evaluated on client");
    }
    
    public static short NextVal(this IDbSequence<short> _)
    {
      throw new InvalidOperationException($"Function '{nameof(DbSequenceExtensions.NextVal)}' cannot be evaluated on client");
    }
    
    public static int NextVal(this IDbSequence<int> _)
    {
      throw new InvalidOperationException($"Function '{nameof(DbSequenceExtensions.NextVal)}' cannot be evaluated on client");
    }
    
    public static long NextVal(this IDbSequence<long> _)
    {
      throw new InvalidOperationException($"Function '{nameof(DbSequenceExtensions.NextVal)}' cannot be evaluated on client");
    }

    public static decimal NextVal(this IDbSequence<decimal> _)
    {
      throw new InvalidOperationException($"Function '{nameof(DbSequenceExtensions.NextVal)}' cannot be evaluated on client");
    }
  }
}