using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.DTO;
using TestTask.Models;
namespace TestTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] AccountDto accountDto)
    {
        if (accountDto == null || string.IsNullOrEmpty(accountDto.Name))
        {
            return BadRequest("Invalid data.");
        }

        var account = await _context.Accounts
            .Include(a => a.Contacts)
            .FirstOrDefaultAsync(a => a.Name == accountDto.Name);

        if (account != null)
        {
            return Conflict("Account already exists.");
        }

        account = new Account
        {
            Name = accountDto.Name,
            Contacts = new List<Contact> { }
        };

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return Ok(account);
    }
}