using NpgsqlTypes;

namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Fake;

[@Index(nameof(FirstName))]
[@Index(nameof(LastName))]
[@Index(nameof(Email))]
[@Index(nameof(Phone))]
[@Index(nameof(GroupNumber))]
[@Index(nameof(DateOfBirth))]
public class FakeData
{

    [Key]
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public int GroupNumber { get; set; }

    public DateTimeOffset DateOfBirth { get; set; }    

}
