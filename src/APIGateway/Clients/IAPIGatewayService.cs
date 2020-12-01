using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Common.Messages;
using Common.States;
using Refit;

namespace APIGateway.Clients
{
    public interface IMessageApiService
    {
        [Get("/messages")]
        Task<IEnumerable<TopicMessage>> GetMessages();
    }

    public interface IStateApiService
    {
        [Get("/state")]
        Task<ApplicationState> GetCurrentState();

        [Put("/state")]
        Task<ApplicationState> SetCurrentState(ApplicationState state);
    }

    public interface IRunLogApiService
    {
        [Get("/run-log")]
        Task<IEnumerable<RunLogEntry>> GetRunLog();

        [Put("/reinit-log")]
        Task<IEnumerable<RunLogEntry>> ReinitRunLog(ApplicationState state);
    }

    public interface IAPIGatewayService : IMessageApiService, IStateApiService, IRunLogApiService
    {
    }

    public class ApiContentSerializer : IContentSerializer
    {
        public async Task<T> DeserializeAsync<T>(HttpContent content)
        {
            string contentString = await content.ReadAsStringAsync();

            if (typeof(T).ImplementsOrDerives(typeof(Enumeration)))
            {
                // NOTE: Having to use the non-generic version since the IContentSerializer is not
                // restricted to only Enumerations
                return (T)Enumeration.FromName(typeof(T), contentString);
            }

            if (typeof(RunLogEntry).IsAssignableFrom(typeof(T)))
            {
                object logEntry = RunLogEntry.FromString(contentString);
                return (T)logEntry;
            }

            if (typeof(TopicMessage).IsAssignableFrom(typeof(T)))
            {
                object logEntry = TopicMessage.FromString(contentString);
                return (T)logEntry;
            }

            var runLogGenericType = typeof(IEnumerable<>).MakeGenericType(typeof(RunLogEntry));

            if (runLogGenericType.IsAssignableFrom(typeof(T)))
            {
                // Run-log
                object logEntries = contentString.RunLogEntriesFromString();

                return (T)logEntries;
            }

            var messageGenericType = typeof(IEnumerable<>).MakeGenericType(typeof(TopicMessage));

            if (messageGenericType.IsAssignableFrom(typeof(T)))
            {
                // messages
                object messages = contentString.TopicMessagesFromString();

                return (T)messages;
            }

            throw new NotSupportedException($"Deserialization of {typeof(T)} is not supported. Value: {content}");
        }

        public Task<HttpContent> SerializeAsync<T>(T item)
        {
            if (item is Enumeration @enum)
            {
                return Task.FromResult<HttpContent>(new StringContent(@enum.Name));
            }

            if (item is IEnumerable<RunLogEntry> logs)
            {
                return Task.FromResult<HttpContent>(new StringContent(logs.RunLogEntriesToString()));
            }

            if (item is RunLogEntry entry)
            {
                return Task.FromResult<HttpContent>(new StringContent(entry.ToString()));
            }

            if (item is IEnumerable<TopicMessage> messages)
            {
                return Task.FromResult<HttpContent>(new StringContent(messages.TopicMessagesToString()));
            }

            if (item is TopicMessage message)
            {
                return Task.FromResult<HttpContent>(new StringContent(message.ToString()));
            }

            throw new NotSupportedException($"Serialization of {typeof(T)} is not supported. Value: {item}");
        }
    }
}
