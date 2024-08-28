namespace ExampleWebApp.Backend.Data.Types;

[Index(nameof(UserName), [nameof(RefreshToken)])]
[Index(nameof(RefreshToken))]
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
    /// rotated utc timestamp ; refresh tokens with non null Rotated that are great to
    /// (utc now + RefreshTokenRotationSkewSeconds) will be deleted on next refresh token validity checks.
    /// </summary>    
    public DateTimeOffset? Rotated { get; set; }

    /// <summary>
    /// expires utc timestamp
    /// </summary>    
    [Required]
    public required DateTimeOffset Expires { get; set; }

}
