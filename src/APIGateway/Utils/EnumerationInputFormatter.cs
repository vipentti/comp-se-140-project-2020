using APIGateway.Features.States;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APIGateway.Utils
{
    public class EnumerationInputFormatter : TextInputFormatter
    {
        public EnumerationInputFormatter() : base()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            string data = "";
            using (var streamReader = context.ReaderFactory(context.HttpContext.Request.Body, encoding))
            {
                data = await streamReader.ReadToEndAsync();
            }

            var type = context.Metadata.ModelType;

            var result = Enumeration.FromName(type, data);

            //var readerType = typeof(EnumerationReader<>).MakeGenericType(type);

            //var readerInstance = Activator.CreateInstance(
            //    typeof(EnumerationReader<>).MakeGenericType(
            //        new Type[] { type }),
            //    BindingFlags.Instance | BindingFlags.Public,
            //    binder: null,
            //    args: Array.Empty<object>(),
            //    culture: null) as EnumerationReader;

            //var value = readerInstance?.Read(data);

            //if (value is null)
            //{
            //    throw new InvalidOperationException($"Unable to read '{data}' as {type}");
            //}

            return InputFormatterResult.Success(result);
        }

        protected override bool CanReadType(Type type) => type.IsImplementationOf(typeof(Enumeration));

        private abstract class EnumerationReader
        {
            public abstract object Read(string data);
        }

        private class EnumerationReader<T> : EnumerationReader
            where T : Enumeration
        {
            public override object Read(string data)
            {
                return Enumeration.FromName<T>(data);
            }
        }

        //public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)

        //{
        //    if (context.Object is not ApplicationState state)
        //    {
        //        throw new InvalidOperationException($"Invalid type {context.ObjectType}");
        //    }

        //    await context.HttpContext.Response.WriteAsync(state.ToString());
        //}
    }
}
