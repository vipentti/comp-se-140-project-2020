using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Formatters
{
    public class PlainTextInputFormatter<TInput> : TextInputFormatter
    {
        private readonly Func<string, TInput> readSingle;
        private readonly Func<string, IEnumerable<TInput>> readMultiple;

        public PlainTextInputFormatter(Func<string, TInput> readSingle, Func<string, IEnumerable<TInput>> readMultiple) : base()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            this.readSingle = readSingle;
            this.readMultiple = readMultiple;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            string data = "";
            using (var streamReader = context.ReaderFactory(context.HttpContext.Request.Body, encoding))
            {
                data = await streamReader.ReadToEndAsync();
            }

            var type = context.Metadata.ModelType;

            var sequenceType = typeof(IEnumerable<>).MakeGenericType(typeof(TInput));

            object result;

            if (sequenceType.IsAssignableFrom(type))
            {
                result = readMultiple(data);
            }
            else if (typeof(TInput).IsAssignableFrom(type))
            {
                result = readSingle(data) ?? throw new InvalidOperationException("ReadSingle returned null");
            }
            else
            {
                throw new InvalidOperationException($"Invalid type {type}. Value: {data}");
            }

            return InputFormatterResult.Success(result);
        }

        protected override bool CanReadType(Type type)
        {
            var sequenceType = typeof(IEnumerable<>).MakeGenericType(typeof(TInput));

            return typeof(TInput).IsAssignableFrom(type) || sequenceType.IsAssignableFrom(type);
        }
    }
}
