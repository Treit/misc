using Benchmark;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public class BackGroundWorker : BackgroundService
    {
        ILogger _logger;
        public BackGroundWorker(ILogger<BackGroundWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Task cancelled.");
                }
            }

            Console.WriteLine("Cancellation requested.");
        }
    }

    public class LifetimeMonitor : BackgroundService
    {
        ILogger _logger;
        IHostApplicationLifetime _lifetime;

        public LifetimeMonitor(ILogger<LifetimeMonitor> logger, IHostApplicationLifetime lifetime)
        {
            _logger = logger;
            _lifetime = lifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true && !stoppingToken.IsCancellationRequested)
            {
                if (File.Exists(@"C:\temp\STOP"))
                {
                    _lifetime.StopApplication();
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ServicePointManager.DefaultConnectionLimit = 100;

            services.AddDbContextFactory<AdventureWorks2019Context>(options =>
            {
                options.UseSqlServer("Server=.; Integrated Security=sspi; Initial Catalog=AdventureWorks2019;Encrypt=false");
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestService", Version = "v1" });
            });

            AddHttpClients(services);
            services.AddSingleton(services);
            services.AddHostedService<BackGroundWorker>();
            services.AddHostedService<LifetimeMonitor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment _)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestService v1"));

            var defaultFile = new DefaultFilesOptions();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ThreadPool.SetMinThreads(64, 64);
        }

        private static IServiceCollection AddHttpClients(IServiceCollection services)
        {
            services.AddHttpClient("TestClient", config =>
            {

            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                HttpClientHandler handler = new()
                {
                    AutomaticDecompression = DecompressionMethods.All
                };

                // Avoid any SSL validation issues.
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

                return handler;
            });

            return services;
        }
    }
}
