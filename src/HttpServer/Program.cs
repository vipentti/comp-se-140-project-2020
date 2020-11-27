using Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace HttpServer
{
    public class Settings
    {
        public string OutFilePath { get; set; } = "";
    }

    public class ServerMiddleware
    {
        public ServerMiddleware(RequestDelegate _)
        {
        }

        public async Task Invoke(HttpContext httpContext, IFileSystem fileSystem, ILogger<ServerMiddleware> logger, IConfiguration config)
        {
            var req = httpContext.Request;
            logger.LogInformation("Received request: {@Headers} {@Path}", req.Headers, req.Path);

            var settings = config.Get<Settings>();

            string content = "";

            try {
                content = await fileSystem.File.ReadAllTextAsync(settings.OutFilePath, System.Text.Encoding.UTF8);
                logger.LogInformation("Read from {path}: '{content}'", settings.OutFilePath, content.Replace(Environment.NewLine, "\\n"));
            } catch (System.IO.FileNotFoundException ex) {
                logger.LogWarning("File {path} does not exist.\n{@Exception}", settings.OutFilePath, ex);
            }

            await httpContext.Response.WriteAsync(content);
        }
    }

    public class Program
    {
        private static async Task Main(string[] args)
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
                    services.AddSingleton<IFileSystem, FileSystem>();
                });
    }
}
