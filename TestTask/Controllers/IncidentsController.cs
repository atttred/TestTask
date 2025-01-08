using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.DTO;
using TestTask.Models;
namespace TestTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public IncidentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateIncident([FromBody] IncidentDto incidentDto)
    {
        if (!CheckIncidentData(incidentDto))
        {
            return BadRequest("Invalid data.");
        }

        var account = await _context.Accounts
            .Include(a => a.Contacts)
            .FirstOrDefaultAsync(a => a.Name == incidentDto.AccountName);

        if (account == null)
        {
            return NotFound("Account not found.");
        }
        else if (!string.IsNullOrEmpty(account.IncidentName))
        {
            return Conflict("An incident already exists for this account.");
        }

        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Email == incidentDto.ContactEmail);

        if (contact == null)
        {
            contact = new Contact
            {
                FirstName = incidentDto.ContactFirstName,
                LastName = incidentDto.ContactLastName,
                Email = incidentDto.ContactEmail,
                AccountId = account.Id,
                Account = account,
            };

            await _context.Contacts.AddAsync(contact);
        }
        else if (contact.FirstName != incidentDto.ContactFirstName || contact.LastName != incidentDto.ContactLastName)
        {
            return Conflict("Contact details do not match.");
        }
        else if (contact.AccountId != account.Id)
        {
            return Conflict("Contact is linked to a different account.");
        }

        var incident = new Incident
        {
            Name = Guid.NewGuid().ToString(),
            Description = incidentDto.Description,
            Accounts = new List<Account> { account }
        };

        await _context.Incidents.AddAsync(incident);
        await _context.SaveChangesAsync();

        return Ok(new { IncidentId = incident.Name });
    }

    [HttpPut("{incidentId}")]
    public async Task<IActionResult> UpdateIncident(string incidentId, [FromBody] UpdateIncidentDto updateDto)
    {
        if (string.IsNullOrEmpty(incidentId) || string.IsNullOrEmpty(updateDto.AccountName) || string.IsNullOrEmpty(updateDto.Description))
        {
            return BadRequest("Invalid data.");
        }

        var incident = await _context.Incidents
            .Include(i => i.Accounts)
            .FirstOrDefaultAsync(i => i.Name == incidentId);

        if (incident == null)
        {
            return NotFound("Incident not found.");
        }

        var account = await _context.Accounts
            .Include(a => a.Contacts)
            .FirstOrDefaultAsync(a => a.Name == updateDto.AccountName);

        if (account == null)
        {
            return NotFound("Account not found.");
        }

        if (!incident.Accounts.Contains(account))
        {
            incident.Accounts.Add(account);
            incident.Description += $" " + updateDto.Description;
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Incident updated successfully." });
    }

    private bool CheckIncidentData(IncidentDto incidentDto)
    {
        return !string.IsNullOrEmpty(incidentDto.AccountName) &&
               !string.IsNullOrEmpty(incidentDto.ContactEmail) &&
               !string.IsNullOrEmpty(incidentDto.Description) &&
               !string.IsNullOrEmpty(incidentDto.ContactFirstName) &&
               !string.IsNullOrEmpty(incidentDto.ContactLastName) &&
               incidentDto.ContactEmail.Contains('@');
    }
}