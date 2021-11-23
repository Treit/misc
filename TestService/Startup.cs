using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Test
{
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestService", Version = "v1" });
            });

            AddHttpClients(services);
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
