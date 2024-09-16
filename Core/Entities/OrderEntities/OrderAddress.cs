namespace Core.Entities.OrderEntities;
public class OrderAddress
{
    public OrderAddress() { /* we create this constructor because EF need it while migration to make instance from this class */ }
    public OrderAddress(string firstName, string lastName, string street, string city, string country)
    {
        FirstName = firstName;
        LastName = lastName;
        Street = street;
        City = city;
        Country = country;
    }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
}