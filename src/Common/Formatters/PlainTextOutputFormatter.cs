using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Formatters
{
    public class PlainTextOutputFormatter<TOutput> : TextOutputFormatter
    {
        public PlainTextOutputFormatter() : base()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected virtual string SerializeToString(TOutput data) => data?.ToString() ?? "";

        protected virtual string SerializeToString(IEnumerable<TOutput> data)
        {
            return string.Join(Environment.NewLine, data.Select(SerializeToString));
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            string response;

            if (context.Object is TOutput single)
            {
                response = SerializeToString(single);
            }
            else if (context.Object is IEnumerable<TOutput> multiple)
            {
                response = SerializeToString(multiple);
            }
            else
            {
                throw new InvalidOperationException($"Invalid type {context.ObjectType}");
            }

            await context.HttpContext.Response.WriteAsync(response);
        }

        protected override bool CanWriteType(Type type)
        {
            var sequenceType = typeof(IEnumerable<>).MakeGenericType(typeof(TOutput));

            return typeof(TOutput).IsAssignableFrom(type) || sequenceType.IsAssignableFrom(type);
        }
    }
}
