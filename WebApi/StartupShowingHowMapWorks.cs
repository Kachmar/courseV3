using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Fundamentals
{

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class StartupShowingHowMapWorks
    {
        public void ConfigureServices(IServiceCollection services)
        { }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();

            app.MapWhen(
                context =>
                {
                    var userAgent = context.Request.Headers["user-agent"][0];
                    var IsIe = userAgent.Contains("MSIE") || userAgent.Contains("Trident");
                    return IsIe;
                },
                builder =>
                    {
                        builder.Run(
                            context =>
                                {
                                    return context.Response.WriteAsync("Hey IE user!");

                                });
                    });

            app.MapWhen(
                context =>
                {
                    var isChrome = context.Request.Headers["user-agent"][0].Contains("Chrome");
                    return isChrome;
                },
                builder =>
                {
                    builder.Run(
                        context =>
                        {
                            return context.Response.WriteAsync("Hey Chrome user!");
                        });
                });
            app.Map(
                "/SpecialRoute",
                appBuilder =>
                    {
                        appBuilder.Run(context => context.Response.WriteAsync("Special Route reached."));
                    });

            //This adds middleware
            app.UseMiddleware<MyRequestHandler>();
            app.Use(async (context, next) => { next(); });
            app.Use(
                (Httpcontext, func) =>
                {
                    foreach (var keyValuePair in Httpcontext.Request.Headers)
                    {
                        Console.WriteLine(keyValuePair.Value);
                    }

                    Httpcontext.Response.WriteAsync("This is response from middleware 1");
                    return func();
                });
            app.Use(
                (Httpcontext, func) =>
                    {
                        return Httpcontext.Response.WriteAsync("This is response from middleware 2");
                        // return func();
                    });
            //This is the last item in a chain, and Run means no further items in the pipeline
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
