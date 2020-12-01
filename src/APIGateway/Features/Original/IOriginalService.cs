using Common;
using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIGateway.Features.Original
{
    public interface IOriginalService
    {
        [Put("/start")]
        Task<ApplicationState> Start();

        [Put("/stop")]
        Task<ApplicationState> Stop();

        [Put("/reset")]
        Task<ApplicationState> Reset();

        [Put("/state")]
        Task<ApplicationState> SetState(ApplicationState state);
    }

    public class EnumerationSerializer : IContentSerializer
    {
        public async Task<T> DeserializeAsync<T>(HttpContent content)
        {
            if (!typeof(T).ImplementsOrDerives(typeof(Enumeration)))
            {
                throw new InvalidOperationException($"Unsupported content type {typeof(T)} {content}");
            }

            string contentString = await content.ReadAsStringAsync();

            // NOTE: Having to use the non-generic version since the IContentSerializer is not
            // restricted to only Enumerations
            return (T)Enumeration.FromName(typeof(T), contentString);
        }

        public Task<HttpContent> SerializeAsync<T>(T item)
        {
            if (item is not Enumeration enumeration)
            {
                throw new InvalidOperationException($"Item is not enumeration {item} {typeof(T)}");
            }

            return Task.FromResult<HttpContent>(new StringContent(enumeration.Name));
        }
    }
}
