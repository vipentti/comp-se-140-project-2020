using APIGateway.Features.States;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIGateway.Tests
{
    public static class Extensions
    {
        public static WebApplicationFactory<Startup> WithTestServices(this WebApplicationFactory<Startup> factory, Action<IServiceCollection> configuration)
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(configuration);
            });
        }

        public static async Task<HttpResponseMessage> PutStringContent(this HttpClient httpClient, string endpoint, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Method = HttpMethod.Put,
                Content = new StringContent(value.ToString()),
            };

            return await httpClient.SendAsync(request);
        }

        public static async Task<IEnumerable<RunLogEntry>> GetRunLogEntries(this HttpClient client, string endpoint = "/run-log")
        {
            var response = await client.GetAsync(endpoint);

            // Assert

            response.Should().NotBeNull();
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return content
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(RunLogEntry.FromString)
                .ToList();
        }
    }
}
