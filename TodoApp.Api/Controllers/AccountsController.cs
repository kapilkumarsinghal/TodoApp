using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;
using TodoApp.Api.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accounts;
    public AccountsController(IAccountService accounts) => _accounts = accounts;

    [HttpPost("register")]
    public async Task<ActionResult<AccountResponse>> Register(AccountRequest request)
    {
        if (!new EmailAddressAttribute().IsValid(request.Email)) return BadRequest("Enter a valid email address.");
        if (request.Email.Trim().Length > 256) return BadRequest("Email must be 256 characters or fewer.");
        if (request.Password.Length < 8) return BadRequest("Password must contain at least 8 characters.");
        if (request.Password.Length > 128) return BadRequest("Password must be 128 characters or fewer.");
        var account = await _accounts.RegisterAsync(request);
        return account is null ? Conflict("An account with that email already exists.") : Ok(account);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccountResponse>> Login(AccountRequest request)
    {
        var account = await _accounts.LoginAsync(request);
        return account is null ? Unauthorized("Email or password is incorrect.") : Ok(account);
    }
}
