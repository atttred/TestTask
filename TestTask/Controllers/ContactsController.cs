using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.DTO;
using TestTask.Models;
namespace TestTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContactsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("{accountName}")]
    public async Task<IActionResult> CreateOrUpdateContact(string accountName, [FromBody] ContactDto contactDto)
    {
        if (string.IsNullOrEmpty(accountName) || contactDto == null || string.IsNullOrEmpty(contactDto.Email) || !contactDto.Email.Contains('@'))
        {
            return BadRequest("Invalid data.");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Name == accountName);
        if (account == null)
        {
            return NotFound("Account not found.");
        }

        var contact = await _context.Contacts.Include(c => c.Account).FirstOrDefaultAsync(c => c.Email == contactDto.Email);

        if (contact != null)
        {
            contact.FirstName = contactDto.FirstName;
            contact.LastName = contactDto.LastName;

            if (contact.AccountId != account.Id)
            {
                contact.AccountId = account.Id;
                contact.Account = account;
            }

            _context.Contacts.Update(contact);
        }
        else
        {
            contact = new Contact
            {
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Email = contactDto.Email,
                AccountId = account.Id,
                Account = account,
            };

            await _context.Contacts.AddAsync(contact);
        }

        await _context.SaveChangesAsync();
        return Ok(contact);
    }
}