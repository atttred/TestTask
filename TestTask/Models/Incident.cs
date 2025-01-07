namespace TestTask.Models;

public class Incident
{
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<Account> Accounts { get; set; }
}
