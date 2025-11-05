namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Fake;

public class FakeData
{

    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public int GroupNumber { get; set; }

    public DateTimeOffset DateOfBirth { get; set; }

}
