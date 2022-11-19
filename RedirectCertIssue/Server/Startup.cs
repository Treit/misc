using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace RedirectCertIssue
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(builder =>
            {
                var serverBinding = new WSHttpBinding(SecurityMode.None);
                serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;

                var serverBindingHttps = new WSHttpBinding(SecurityMode.Transport);
                serverBindingHttps.Security.Message.ClientCredentialType = MessageCredentialType.None;

                builder
                    .AddService<TestService>()
                    .AddServiceEndpoint<TestService, ITestService>(new BasicHttpBinding(), "/basichttp")
                    .AddServiceEndpoint<TestService, ITestService>(serverBinding, "/wsHttp.svc")
                    .AddServiceEndpoint<TestService, ITestService>(serverBindingHttps, "/wsHttp.svc");
            });
        }
    }
}
