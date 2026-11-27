using Evenex.Common.Hashing;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Evenex.Api.ModelBinding;

/// <summary>
/// URL'deki hash'li id'yi ("k1JppaeX") controller parametresine gerçek int olarak (7) koyan model binder.
///
/// Model binder = ASP.NET'in, controller metodu çalışmadan hemen önce isteğin içindeki ham metinleri
/// (URL, query, gövde) alıp metodun parametrelerine yerleştiren parçası. Hazır olanlar hash'i bilmez;
/// bu bizim yazdığımız, hash'i çözmeyi bilen resepsiyon görevlisi.
/// </summary>
public class HashIdModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // ModelName = parametrenin adı ("id", "venueId"...). ValueProvider URL'den/query'den o adla değeri bulur.
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

        // Değer hiç yoksa bir şey yapmıyoruz; ASP.NET "zorunlu parametre eksik" hatasını kendisi verir
        if (string.IsNullOrEmpty(value))
            return Task.CompletedTask;

        // Uydurma/bozuk hash: kullanıcı açısından "böyle bir kayıt yok" ile aynı -> 404
        // (eskiden servislerdeki DecodeId de 404 veriyordu, davranış aynı kaldı).
        // Mesajda gelen hash'i yazıyoruz; gerçek id zaten yok, sızacak bir şey yok.
        if (!HashidsHelper.TryDecode(value, out var id))
            throw new KeyNotFoundException($"Kayıt bulunamadı: {value}");

        bindingContext.Result = ModelBindingResult.Success(id);
        return Task.CompletedTask;
    }
}
