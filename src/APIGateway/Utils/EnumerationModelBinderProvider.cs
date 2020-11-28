using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Common;

namespace APIGateway.Utils
{
    /// <summary>
    /// Provides types inheriting from <see cref="Enumeration"/> with valid modelbinder
    /// </summary>
    public class EnumerationModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var type = context.Metadata.ModelType;

            if (type.IsImplementationOf(typeof(Enumeration)))
            {
                var binderType = typeof(EnumerationModelBinder<>).MakeGenericType(type);

                return new BinderTypeModelBinder(binderType);
            }

            return null;
        }

        /// <summary>
        /// Provides support for modelbinding <see cref="Enumeration"/>. This allows Enumerations to
        /// be used as parameters to controller methods
        /// </summary>
        /// <typeparam name="T">Specific enumeration subclass</typeparam>
        private class EnumerationModelBinder<T> : IModelBinder
            where T : Enumeration
        {
            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var modelName = bindingContext.ModelName;

                // Try to fetch the value of the argument by name
                var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

                if (valueProviderResult == ValueProviderResult.None)
                {
                    return Task.CompletedTask;
                }

                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

                var value = valueProviderResult.FirstValue;

                // Check if the argument value is null or empty
                if (string.IsNullOrEmpty(value))
                {
                    return Task.CompletedTask;
                }

                if (!Enumeration.TryFromName<T>(value, out var code))
                {
                    // invalid name
                    bindingContext.ModelState.TryAddModelError(
                        modelName, $"'{value}' is not valid '{typeof(T).Name}'");

                    return Task.CompletedTask;
                }

                bindingContext.Result = ModelBindingResult.Success(code);

                return Task.CompletedTask;
            }
        }
    }
}
