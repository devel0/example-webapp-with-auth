namespace ExampleWebApp.Backend.WebApi.Services.Auth.Data;

[@Index(nameof(UserName), [nameof(RefreshToken)])]
[@Index(nameof(RefreshToken))]
public class UserRefreshToken
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [Required]
    public required string UserName { get; set; }

    [Required]
    public required string RefreshToken { get; set; }

    /// <summary>
    /// issued utc timestamp
    /// </summary>
    [Required]
    public required DateTimeOffset Issued { get; set; }

    /// <summary>
    /// expires utc timestamp
    /// </summary>    
    [Required]
    public required DateTimeOffset Expires { get; set; }

}
