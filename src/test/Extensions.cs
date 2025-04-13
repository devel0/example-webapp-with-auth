namespace ExampleWebApp.Backend.Test;

public static class Extensions
{


    public static UserConfig GetAdminUserConfig(this IConfiguration configuration)
    {
        var qAdminSeed = configuration.GetAppConfig().Database.Seed.Users.FirstOrDefault(w => w.Roles.Any(x => x == ROLE_admin));

        if (qAdminSeed is null) throw new Exception($"can't find an admin role user in configuration");

        return qAdminSeed;
    }

    /// <summary>
    /// Extracts X data from Set-Cookie cookie in given response headers.
    /// </summary>
    public static JwtCookies GetJwtCookiesFromResponse(this HttpResponseHeaders headers)
    {
        string? accessToken = null;
        string? userName = null;
        string? refreshToken = null;

        foreach (var hdr in headers)
        {
            switch (hdr.Key)
            {
                case WEB_HeadersCollection_SetCookie:
                    {
                        var cookies = SetCookieHeaderValue.ParseList(hdr.Value.ToList());

                        foreach (var cookie in cookies)
                        {
                            switch (cookie.Name.Value)
                            {
                                case WEB_CookieName_XAccessToken: accessToken = cookie.Value.Value; break;
                                case WEB_CookieName_XRefreshToken: refreshToken = cookie.Value.Value; break;
                            }
                        }
                    }
                    break;
            }
        }

        return new JwtCookies
        {
            AccessToken = accessToken,
            UserName = userName,
            RefreshToken = refreshToken
        };
    }

    /// <summary>
    /// Sets given cookie in the http client default request headers.
    /// </summary>
    public static void SetCookie(this HttpClient client, string cookieName, string cookieValue)
    {
        var cookieReplaced = false;

        if (client.DefaultRequestHeaders.Contains(WEB_Cookie))
        {
            var cookies = client.DefaultRequestHeaders.GetValues(WEB_Cookie);
            client.DefaultRequestHeaders.Remove(WEB_Cookie);

            foreach (var cookie in cookies)
            {
                if (cookie.StartsWith($"{cookieName}="))
                {
                    client.DefaultRequestHeaders.Add(WEB_Cookie, $"{cookieName}={cookieValue}");
                    cookieReplaced = true;
                }
                else
                    client.DefaultRequestHeaders.Add(WEB_Cookie, cookie);
            }
        }

        if (!cookieReplaced)
        {
            client.DefaultRequestHeaders.Add(WEB_Cookie, $"{cookieName}={cookieValue}");
        }
    }

    /// <summary>
    /// Simulate browser behavior applying Set-Cookie from given response to the given client default request headers.
    /// Note that this doesn't take in account the secure and same site policy into account for testing purpose.
    /// </summary>
    public static HttpResponseMessage ApplySetCookies(this HttpResponseMessage res, HttpClient client)
    {
        // on the client default request headers no more than one "Cookie" headers can exists
        // as for https://www.rfc-editor.org/rfc/rfc6265#section-5.4

        // while Set-Cookie header can appears multiple times in the response        

        foreach (var hdr in res.Headers)
        {
            if (hdr.Key == WEB_HeadersCollection_SetCookie)
            {
                var cookies = SetCookieHeaderValue.ParseList(hdr.Value.ToList());

                foreach (var cookie in cookies)
                {
                    if (cookie.Name.Value is null) continue;

                    client.SetCookie(cookie.Name.Value, cookie.Value.Value ?? "");
                }
            }
        }

        return res;
    }

    /// <summary>
    /// Deserialize reponse content into given object template.
    /// </summary>
    public static async Task<T?> DeserializeAsync<T>(this HttpResponseMessage httpResponseMessage, IUtilService util)
    {
        return await httpResponseMessage.Content.ReadFromJsonAsync<T>(util.JavaSerializerSettings);
    }

}