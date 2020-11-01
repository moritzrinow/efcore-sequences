namespace EntityFrameworkCore.Extensions.Sequences
{
  using System;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using EntityFrameworkCore.Extensions.Sequences.Internal;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Infrastructure;

  public static class DbContextExtensions
  {
    /// <summary>
    /// Creates an <see cref="IQueryable{T}"/> for querying sequences from the specified <see cref="DbContext"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<IDbSequence<long>> Sequences(this DbContext context)
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      return provider.Query<long>();
    }

    /// <summary>
    /// Creates an <see cref="IQueryable{T}"/> for querying sequences of a specific type from the specified <see cref="DbContext"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IQueryable<IDbSequence<T>> Sequences<T>(this DbContext context)
      where T : struct
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      return provider.Query<T>();
    }

    /// <summary>
    /// Gets the next sequence value
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T NextSequenceValue<T>(this DbContext context, string name)
      where T : struct
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var res = provider.GetNextSequenceValue<T>(name);

      return res;
    }

    /// <summary>
    /// Gets the next sequence value asynchronous.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<T> NextSequenceValueAsync<T>(this DbContext context, string name, CancellationToken ct = default)
      where T : struct
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var res = await provider.GetNextSequenceValueAsync<T>(name, ct);

      return res;
    }

    /// <summary>
    /// Creates a sequence.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sequence"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool CreateSequence<T>(this DbContext context, DbSequence<T> sequence)
      where T : struct
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var res = provider.CreateSequence(sequence);

      return res;
    }

    /// <summary>
    /// Creates a sequence asynchronously.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sequence"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<bool> CreateSequenceAsync<T>(this DbContext context, DbSequence<T> sequence, CancellationToken ct = default)
      where T : struct
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var res = await provider.CreateSequenceAsync(sequence, ct);

      return res;
    }

    /// <summary>
    /// Updates the sequence.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <param name="update"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool UpdateSequence<T>(this DbContext context, string name, DbSequenceUpdate<T> update)
      where T : struct
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var res = provider.UpdateSequence(name, update);

      return res;
    }

    /// <summary>
    /// Updates the sequence asynchronously.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <param name="update"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<bool> UpdateSequenceAsync<T>(this DbContext context, string name, DbSequenceUpdate<T> update, CancellationToken ct = default)
      where T : struct
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var res = await provider.UpdateSequenceAsync(name, update, ct);

      return res;
    }
    
    /// <summary>
    /// Drops the sequence if it exists.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool DropSequence(this DbContext context, string name)
    {
      return context.DropSequenceInternal(name, true);
    }

    /// <summary>
    /// Drops the sequence asynchronously with the specified condition.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <param name="conditionally"></param>
    /// <returns></returns>
    public static bool DropSequence(this DbContext context, string name, bool conditionally)
    {
      return context.DropSequenceInternal(name, conditionally);
    }

    /// <summary>
    /// Drops the sequence asynchronously if it exists.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public static Task<bool> DropSequenceAsync(this DbContext context, string name, CancellationToken ct = default)
    {
      return context.DropSequenceInternalAsync(name, true, ct);
    }

    /// <summary>
    /// Drops the sequence asynchronously with the specified condition.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <param name="conditionally"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public static Task<bool> DropSequenceAsync(this DbContext context, string name, bool conditionally, CancellationToken ct = default)
    {
      return context.DropSequenceInternalAsync(name, conditionally, ct);
    }
    
    private static bool DropSequenceInternal(this DbContext context, string name, bool conditionally)
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var res = provider.DropSequence(name, conditionally);

      return res;
    }

    private static async Task<bool> DropSequenceInternalAsync(this DbContext context, string name, bool conditionally, CancellationToken ct = default)
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      var res = await provider.DropSequenceAsync(name, conditionally, ct);

      return res;
    }
  }
}