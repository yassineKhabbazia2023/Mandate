namespace KPMG.Pulse.Back.Accounting.Mandate;
public class Address
{
    public Address(string? street, string? complements, string? zipCode, string? city, string? country)
    {
        Street = street;
        Complements = complements;
        ZipCode = zipCode;
        City = city;
        Country = country;
    }

    public string? Street { get; }

    public string? Complements { get; }

    public string? ZipCode { get; }

    public string? City { get; }

    public string? Country { get; }

    public string FullAddress => string.Join(" - ", new[] { Street, Complements, ZipCode, City, Country }.Where(s => !string.IsNullOrEmpty(s)));
}
