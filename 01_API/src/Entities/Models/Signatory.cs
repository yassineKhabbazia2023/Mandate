namespace KPMG.Pulse.Back.Accounting.Mandate;
public class Signatory
{
    public Signatory(string? title, string? firstName, string? lastName, string? email)
    {
        Title = title;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public string? Title { get; }

    public string? FirstName { get; }

    public string? LastName { get; }

    public string? Email { get; }

    public string FullName => string.Join(" ", new[]{Title, FirstName, LastName}.Where(s => !string.IsNullOrEmpty(s)));
}
