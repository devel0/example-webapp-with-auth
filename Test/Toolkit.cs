namespace Test;

public static partial class Toolkit
{

    public static JwtSecurityToken? DecodeToJwtSecurityToken(string jwtEncoded)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadToken(jwtEncoded) as JwtSecurityToken;
    }

    public static SymmetricSecurityKey RandomJwtEncryptionKey()
    {
        var rsaKey = RSA.Create();
        var privateKey = rsaKey.ExportRSAPrivateKey();
        return new SymmetricSecurityKey(privateKey);
    }

    /// <summary>
    /// create jwt token from given template with everything equal in the content but with different jwt key.
    /// </summary>
    public static string GenerateFakeAccessToken(IConfiguration config, string accessTokenTemplate)
    {
        var template = DecodeToJwtSecurityToken(accessTokenTemplate);
        if (template is null) throw new InternalError("can't decode jwt token");

        var key = RandomJwtEncryptionKey();

        var accessTokenLifetime = TimeSpan.FromSeconds(
            config.GetConfigVar<double>(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(template.Claims),
            Expires = DateTime.UtcNow.Add(accessTokenLifetime),
            Issuer = template.Issuer,
            Audience = template.Audiences.First(),
            SigningCredentials = new SigningCredentials(key, JWT_SecurityAlghoritm)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

}