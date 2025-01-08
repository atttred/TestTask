using System.Text.Json.Serialization;
namespace TestTask.Models;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? IncidentName { get; set; }

    public Incident? Incident { get; set; }

    [JsonIgnore]
    public ICollection<Contact> Contacts { get; set; }
}