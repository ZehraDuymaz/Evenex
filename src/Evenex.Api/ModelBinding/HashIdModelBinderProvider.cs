using Evenex.Common.Hashing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Evenex.Api.ModelBinding;

/// <summary>
/// ASP.NET her parametre için "bunu hangi binder dolduracak?" diye provider'lara sorar.
/// Bu provider sadece [HashId] etiketli int parametreler için "ben hallederim" der ve HashIdModelBinder'ı verir;
/// diğer tüm parametreler için null döner, onları ASP.NET'in hazır binder'ları doldurmaya devam eder.
/// Program.cs'te listenin EN BAŞINA eklenir ki hazır int binder'ından önce sorulsun.
/// </summary>
public class HashIdModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var isHashIdParameter =
            context.Metadata.ModelType == typeof(int)
            && context.Metadata is DefaultModelMetadata metadata
            && metadata.Attributes.ParameterAttributes?.OfType<HashIdAttribute>().Any() == true;

        return isHashIdParameter ? new BinderTypeModelBinder(typeof(HashIdModelBinder)) : null;
    }
}
