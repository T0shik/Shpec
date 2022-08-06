using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shpec.Validation;

namespace Shpec.AspNetCore;

internal class ShepcObjectModelValidator : IObjectModelValidator
{
    public void Validate(ActionContext actionContext, ValidationStateDictionary? validationState, string prefix, object? model)
    {
        if (model is not IValidatable validatable)
        {
            return;
        }

        var result = validatable.Valid();
        if (result)
        {
            return;
        }

        foreach (var validationError in result.Errors)
        {
            actionContext.ModelState.AddModelError(validationError.Key, validationError.Error);
        }
    }
}