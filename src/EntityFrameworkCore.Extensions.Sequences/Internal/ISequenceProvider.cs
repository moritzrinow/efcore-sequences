namespace EntityFrameworkCore.Extensions.Sequences.Internal
{
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;

  public interface ISequenceProvider
  {
    /// <summary>
    /// Configures the specified <see cref="ModelBuilder"/> to be aware of the keyless type <see cref="IDbSequence"/>.
    /// </summary>
    /// <param name="builder"></param>
    void ConfigureModel(ModelBuilder builder);

    /// <summary>
    /// Creates an <see cref="IQueryable"/> for querying sequences from the specified <see cref="DbContext"/>.
    /// </summary>
    /// <returns></returns>
    IQueryable<IDbSequence<T>> Query<T>() where T : struct;

    /// <summary>
    /// Gets the default schema of the specified <see cref="DbContext"/>.
    /// </summary>
    /// <returns></returns>
    string GetSchema();

    /// <summary>
    /// Gets the next sequence value.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetNextSequenceValue<T>(string name) where T : struct;

    /// <summary>
    /// Create a sequence.
    /// </summary>
    /// <param name="sequence"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    bool CreateSequence<T>(DbSequence<T> sequence) where T : struct;

    /// <summary>
    /// Updates the sequence.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="update"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    bool UpdateSequence<T>(string name, DbSequenceUpdate<T> update) where T : struct;
    
    /// <summary>
    /// Drops the sequence.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="conditionally"></param>
    /// <returns></returns>
    bool DropSequence(string name, bool conditionally);
    
    /// <summary>
    /// Gets the next sequence value asynchronously.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<T> GetNextSequenceValueAsync<T>(string name, CancellationToken ct = default) where T : struct;

    /// <summary>
    /// Creates a sequence asynchronously.
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<bool> CreateSequenceAsync<T>(DbSequence<T> sequence, CancellationToken ct = default) where T : struct;

    /// <summary>
    /// Updates the sequence asynchronously.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="update"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<bool> UpdateSequenceAsync<T>(string name, DbSequenceUpdate<T> update, CancellationToken ct = default) where T : struct;

    /// <summary>
    /// Drops the sequence asynchronously.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="conditionally"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<bool> DropSequenceAsync(string name, bool conditionally, CancellationToken ct = default);
  }
}