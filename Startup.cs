using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Platform.Services;

namespace Platform
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IResponseFormatter, GuidService>();
        }
        public void Configure(IApplicationBuilder app, 
                              IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();            
            app.UseRouting();
            app.UseMiddleware<WeatherMiddleware>();
            
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/middleware/function")
                {
                    IResponseFormatter formatter
                    = app.ApplicationServices.GetService<IResponseFormatter>();
                    await formatter.Format(context, 
                        "Middleware Function: It is snowing in Chicago");
                }
                else
                {
                    await next();
                }
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapEndpoint<WeatherEndpoint>("/endpoint/class");
                endpoints.MapGet("/endpoint/function", async context =>
                {
                    IResponseFormatter formatter
                    = app.ApplicationServices.GetService<IResponseFormatter>();
                    await formatter.Format(context,
                    "Endpoint Function: It is sunny in LA");
                });
            });
        }
    }
}
