using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.SiteManagement.CariHesaplari.Queries;

namespace SiteYonetimi.API.Controllers;

/// <summary>
/// Ödeme Makbuzu / Gelir Tahsilat Makbuzu formlarında paylaşılan Cari Hesap picker.
/// Kendi başına bir sayfa değil, ilgili sayfaların yetkisi altında kullanılır —
/// bu yüzden [RequirePage] eklenmez (yalnızca site-token Authorize yeterli).
/// </summary>
[Route("api/cari-hesaplar")]
public class CariHesaplariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetCariHesaplarForPickerQuery(CurrentSiteId, search)));
}
