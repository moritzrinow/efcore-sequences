namespace EntityFrameworkCore.Extensions.Sequences
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using Microsoft.EntityFrameworkCore;

  public static class DbFunctionsExtensions
  {
    /// <summary>
    /// Gets the next sequence value.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static byte NextValByte(this DbFunctions _, string name)
    {
      throw new InvalidOperationException($"Function '{nameof(DbFunctionsExtensions.NextValByte)}' cannot be evaluated on client");
    }
    
    /// <summary>
    /// Gets the next sequence value.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static short NextValShort(this DbFunctions _, string name)
    {
      throw new InvalidOperationException($"Function '{nameof(DbFunctionsExtensions.NextValShort)}' cannot be evaluated on client");
    }
    
    /// <summary>
    /// Gets the next sequence value.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static int NextValInt(this DbFunctions _, string name)
    {
      throw new InvalidOperationException($"Function '{nameof(DbFunctionsExtensions.NextValInt)}' cannot be evaluated on client");
    }
    
    /// <summary>
    /// Gets the next sequence value.
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public static long NextValLong(this DbFunctions _, string sequence)
    {
      throw new InvalidOperationException($"Function '{nameof(DbFunctionsExtensions.NextValLong)}' cannot be evaluated on client");
    }

    /// <summary>
    /// Gets the next sequence value.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static decimal NextValDec(this DbFunctions _, string name)
    {
      throw new InvalidOperationException($"Function '{nameof(DbFunctionsExtensions.NextValDec)}' cannot be evaluated on client");
    }
  }
}