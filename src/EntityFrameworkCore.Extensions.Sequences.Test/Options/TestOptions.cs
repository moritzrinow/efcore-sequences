namespace EntityFrameworkCore.Extensions.Sequences.Test.Options
{
  using System.Collections.Generic;

  public class TestOptions
  {
    public string CurrentDatabase { get; set; }

    public IDictionary<string, DatabaseOptions> Databases { get; set; }
  }
}