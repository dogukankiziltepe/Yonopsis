using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.SiteManagement.Bankalar.Commands;
using SiteYonetimi.SiteManagement.Bankalar.DTOs;
using SiteYonetimi.SiteManagement.Bankalar.Queries;

namespace SiteYonetimi.API.Controllers;

/// <summary>
/// Global banka/şube referans verisi. Okuma tüm site kullanıcılarına açık
/// (Personel Banka Bilgileri picker'ı için); yazma SuperAdmin'e özeldir.
/// </summary>
[Route("api/bankalar")]
public class BankalarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Handle(await Mediator.Send(new GetBankalarQuery()));

    [HttpGet("subeler")]
    public async Task<IActionResult> GetSubeler([FromQuery] Guid? bankaId = null, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetBankaSubeleriQuery(bankaId, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBankaDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        var result = await Mediator.Send(new CreateBankaCommand(dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBankaDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new UpdateBankaCommand(id, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new DeleteBankaCommand(id)));
    }

    [HttpPost("{bankaId:guid}/subeler")]
    public async Task<IActionResult> CreateSube(Guid bankaId, [FromBody] CreateBankaSubesiDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        var result = await Mediator.Send(new CreateBankaSubesiCommand(bankaId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("subeler/{id:guid}")]
    public async Task<IActionResult> UpdateSube(Guid id, [FromBody] UpdateBankaSubesiDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new UpdateBankaSubesiCommand(id, dto)));
    }

    [HttpDelete("subeler/{id:guid}")]
    public async Task<IActionResult> DeleteSube(Guid id)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new DeleteBankaSubesiCommand(id)));
    }
}
