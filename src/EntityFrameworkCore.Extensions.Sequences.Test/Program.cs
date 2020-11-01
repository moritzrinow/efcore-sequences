namespace EntityFrameworkCore.Extensions.Sequences.Test
{
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  public class Program
  {
    public static void Main(string[] args)
    {
      Program.CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration(config => { config.AddJsonFile("appsettings.local.json", true, true); })
                 .ConfigureLogging((host, logging) => logging.AddConsole())
                 .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
  }
}