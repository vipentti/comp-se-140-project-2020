using System;
using Xunit;
using Moq;
using FluentAssertions;
using Common;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;

namespace HttpServer.Tests
{
    public class ServerMiddlewareTests
    {
        private readonly Mock<IFileSystem> _fileSystemMock;
        private readonly Mock<IFile> _fileMock;
        private readonly Mock<ILogger<ServerMiddleware>> _loggerMock;
        private readonly IHostBuilder _hostBuilder;

        public ServerMiddlewareTests()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _fileMock = new Mock<IFile>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<ServerMiddleware>>();

            _fileMock.Setup(it => it.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<System.Text.Encoding>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("test content");

            _fileSystemMock.Setup(it => it.File).Returns(_fileMock.Object);

            _hostBuilder = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .Configure(app =>
                        {
                            app.UseMiddleware<ServerMiddleware>();
                        })
                        .ConfigureAppConfiguration(ProgramCommon.ConfigureApplication)
                        .ConfigureServices(services =>
                        {
                            services.AddTransient(_ => _fileSystemMock.Object);
                            services.AddTransient(_ => _loggerMock.Object);
                        })
                        ;
                });
        }

        [Fact]
        public async Task ServerMiddleWare_Reads_From_FileSystem()
        {
            IHost host = await _hostBuilder.StartAsync();

            var response = await host.GetTestClient().GetAsync("/");

            // assert

            response.Should().NotBeNull();
            response.IsSuccessStatusCode.Should().BeTrue();

            _fileMock.Verify(it => it.ReadAllTextAsync(It.IsAny<string>(),It.IsAny<System.Text.Encoding>(), It.IsAny<CancellationToken>()), Times.Once);

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Be("test content");
        }
    }
}
