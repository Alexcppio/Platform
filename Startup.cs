using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Routing;

namespace Platform
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(opts =>
            {
                opts.ConstraintMap.Add("countryName",
                    typeof(CountryRouteConstraint));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseMiddleware<Population>();
            //app.UseMiddleware<Capital>();
            app.Use(async (context, next) =>
            {
                if (context.Request.Method == HttpMethods.Get)
                {
                    await context.Response.WriteAsync("Custom Middleware \n");
                    await context.Response.WriteAsync(context.Response.ContentLength.ToString() + "\n");
                }
                await next();
            });
            
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                Endpoint end = context.GetEndpoint();
                if (end != null)
                {
                    await context.Response
                    .WriteAsync($"{end.DisplayName} Selected \n");
                }
                else
                {
                    await context.Response.WriteAsync("No Endpoint Selected \n");
                }
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("{number:int}", async context =>
                {
                    await context.Response.WriteAsync("Routed to the int endpoint\n");
                })
                .WithDisplayName("Int Endpoint")
                .Add(b => ((RouteEndpointBuilder)b).Order = 1);
                endpoints.Map("{number:double}", async context =>
                {
                    await context.Response.WriteAsync("Routed to the double endpoint");
                })
                .WithDisplayName("Double Endpoint")
                .Add(b => ((RouteEndpointBuilder)b).Order = 2);
                endpoints.MapGet("capital/{country:countryName}", Capital.Endpoint);
                endpoints.MapGet("size/{city?}", Population.Endpoint)
                    .WithMetadata(new RouteNameMetadata("population"));
                endpoints.MapFallback(async context =>
                {
                    await context.Response.WriteAsync("Routed to fallback endpoint");
                });
            });
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Terminal Middleware Reached");
            });
        }
    }
}
