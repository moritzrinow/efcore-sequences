namespace EntityFrameworkCore.Extensions.Sequences
{
  using System;
  using EntityFrameworkCore.Extensions.Sequences.Internal;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Infrastructure;

  public static class ModelBuilderExtensions
  {
    /// <summary>
    /// Configures the specified <see cref="ModelBuilder"/> to be aware of sequence types.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="context"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void UseSequences(this ModelBuilder builder, DbContext context)
    {
      var provider = context.GetService<ISequenceProvider>();

      if (provider == null)
      {
        throw new InvalidOperationException("No SequenceProvider found");
      }

      provider.ConfigureModel(builder);
    }
  }
}