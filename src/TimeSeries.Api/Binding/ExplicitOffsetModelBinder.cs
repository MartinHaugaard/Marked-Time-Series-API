using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TimeSeries.Api.Binding;

/// <summary>
/// Rejects DateTimeOffset values in the query string that don't state their offset.
/// </summary>
internal sealed class ExplicitOffsetModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (result == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, result);

        if (ExplicitOffset.TryParse(result.FirstValue, out var value))
            bindingContext.Result = ModelBindingResult.Success(value);
        else
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ExplicitOffset.ErrorMessage);

        return Task.CompletedTask;
    }
}

internal sealed class ExplicitOffsetModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var type = Nullable.GetUnderlyingType(context.Metadata.ModelType) ?? context.Metadata.ModelType;
        return type == typeof(DateTimeOffset) ? new ExplicitOffsetModelBinder() : null;
    }
}
