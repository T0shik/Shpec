using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Shpec.AspNetCore;

public static class ServiceRegistration
{
    public static IServiceCollection AddShpecValidator(this IServiceCollection service)
    {
        service.AddSingleton<IObjectModelValidator, ShepcObjectModelValidator>();
        return service;
    }
}