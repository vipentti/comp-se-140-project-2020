using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enumerations
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

            return InputFormatterResult.Success(result);
        }

        protected override bool CanReadType(Type type) => type.ImplementsOrDerives(typeof(Enumeration));
    }
}
