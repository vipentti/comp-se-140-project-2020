using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace HttpServer
{
    public class ServerMiddleware : IMiddleware
    {
        private readonly IFileSystem fileSystem;
        private readonly ILogger<ServerMiddleware> logger;
        private readonly IConfiguration config;

        public ServerMiddleware(IFileSystem fileSystem, ILogger<ServerMiddleware> logger, IConfiguration config)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
            this.config = config;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate _)
        {
            var req = httpContext.Request;
            logger.LogInformation("Received request: {@Headers} {@Path}", req.Headers, req.Path);

            var settings = config.Get<Settings>();

            string content = "";

            try
            {
                if (fileSystem.File.Exists(settings.OutFilePath))
                {
                    content = await fileSystem.File.ReadAllTextAsync(settings.OutFilePath, System.Text.Encoding.UTF8);
                    logger.LogInformation("Read from {path}: '{content}'", settings.OutFilePath, content.Replace(Environment.NewLine, "\\n"));
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                logger.LogWarning("File {path} does not exist.\n{@Exception}", settings.OutFilePath, ex);
            }

            await httpContext.Response.WriteAsync(content);
        }
    }
}
