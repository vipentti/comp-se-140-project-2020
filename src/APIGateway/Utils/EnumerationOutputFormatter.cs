using APIGateway.Features.States;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace APIGateway.Utils
{
    public class EnumerationOutputFormatter : TextOutputFormatter
    {
        public EnumerationOutputFormatter() : base()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context.Object is not ApplicationState state)
            {
                throw new InvalidOperationException($"Invalid type {context.ObjectType}");
            }

            await context.HttpContext.Response.WriteAsync(state.ToString());
        }

        protected override bool CanWriteType(Type type) => type.IsImplementationOf(typeof(Enumeration));
    }
}
