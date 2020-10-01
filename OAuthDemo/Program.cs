using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OAuthDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .Configure(app =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

                    if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }

                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/login", async context =>
                        {
                            if (context.Request.Query.TryGetValue("code", out var codeValue))
                            {
                                await context.Response.WriteAsync("Got the following code:\n");
                                await context.Response.WriteAsync(codeValue);
                            }
                        });

                        endpoints.MapGet("/logoff", async context =>
                        {
                            await context.Response.WriteAsync("Logoff called.");
                        });

                    });
                });
            });

            return builder;
        }
    }
}
