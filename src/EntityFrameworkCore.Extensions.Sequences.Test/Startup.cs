namespace EntityFrameworkCore.Extensions.Sequences.Test
{
  using EntityFrameworkCore.Extensions.Sequences.Test.Database;
  using EntityFrameworkCore.Extensions.Sequences.Test.Options;
  using EntityFrameworkCore.Extensions.Sequences.Test.Services;
  using EntityFrameworkCore.Extensions.Sequences.Test.Services.Tests;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      services.Configure<TestOptions>(this.Configuration.GetSection("TestOptions"));

      services.AddDbContext<SequenceContext>();

      if (this.Configuration.GetValue<bool>("TestOptions:RunTestServices"))
      {
        services.AddTransient<ITestService, SqlServerTest>();
        //services.AddTransient<ITestService, NpgsqlTest>();
        services.AddHostedService<TestSuiteService>();
      }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      
      app.UseRouting();

      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}