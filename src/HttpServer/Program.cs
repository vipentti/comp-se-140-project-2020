using System;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HttpServer
{
    public class Settings
    {
        public string OutFilePath { get; set; } = "";
    }

    class ServerMiddleware
    {
        public ServerMiddleware(RequestDelegate _)
        {
        }

        public async Task Invoke(HttpContext httpContext, ILogger<ServerMiddleware> logger, IConfiguration config)
        {
            var req = httpContext.Request;
            logger.LogInformation("Received request: {@Headers} {@Path}", req.Headers, req.Path);

            var settings = config.Get<Settings>();

            var content = await System.IO.File.ReadAllTextAsync(settings.OutFilePath, System.Text.Encoding.UTF8);

            logger.LogInformation("Read from {path}: '{content}'", settings.OutFilePath, content.Replace(Environment.NewLine, "\\n"));

            await httpContext.Response.WriteAsync(content);
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            ProgramCommon.ConfigureSerilog();

            try
            {
                var host = CreateHostBuilder(args).Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal("Uncaught exception: {Message} {Exception}", ex.Message, ex);
            }
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseKestrel()
                .ConfigureAppConfiguration(ProgramCommon.ConfigureApplication)
                .Configure(app =>
                {
                    app.UseMiddleware<ServerMiddleware>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                });
    }
}
