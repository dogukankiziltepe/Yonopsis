using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SiteYonetimi.Auth.Commands;
using SiteYonetimi.Auth.DTOs;
using SiteYonetimi.Auth.Queries;

namespace SiteYonetimi.API.Controllers;

[AllowAnonymous]
public class AuthController : BaseController
{
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await Mediator.Send(new LoginCommand(request.Email, request.Password));
        if (!result.IsSuccess) return Unauthorized(new { message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await Mediator.Send(new RegisterCommand(
            request.FirstName, request.LastName,
            request.Email, request.Password, request.PhoneNumber));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request,
        [FromHeader(Name = "Authorization")] string? authorization)
    {
        var accessToken = authorization?.Replace("Bearer ", "") ?? string.Empty;
        var result = await Mediator.Send(new RefreshTokenCommand(accessToken, request.RefreshToken));
        if (!result.IsSuccess) return Unauthorized(new { message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        => Handle(await Mediator.Send(new LogoutCommand(request.RefreshToken)));

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok(new
    {
        id = CurrentUserId,
        email = CurrentUserEmail,
        userType = CurrentUserType
    });

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
        => Handle(await Mediator.Send(new GetProfileQuery(CurrentUserId)));

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        => Handle(await Mediator.Send(new UpdateProfileCommand(CurrentUserId, dto)));

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        => Handle(await Mediator.Send(new ChangePasswordCommand(CurrentUserId, request.CurrentPassword, request.NewPassword)));

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await Mediator.Send(new ForgotPasswordCommand(request.Email));
        return Ok(new { message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        => Handle(await Mediator.Send(new ResetPasswordCommand(request.Token, request.NewPassword)));
}
