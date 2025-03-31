namespace ExampleWebApp.Backend.WebApi;

public static class Toolkit
{

    /// <summary>
    /// Compile-time (static) permissions related to roles
    /// </summary>
    public static HashSet<UserPermission> PermissionsFromRoles(HashSet<string> roles)
    {
        var permHs = new HashSet<UserPermission>();

        if (roles.Contains(ROLE_admin))
        {
            foreach (var perm in Enum.GetValues<UserPermission>())
                permHs.Add(perm);
        }

        if (roles.Contains(ROLE_advanced))
        {
            permHs.Add(UserPermission.CreateNormalUser);

            permHs.Add(UserPermission.ChangeOwnEmail);
            permHs.Add(UserPermission.ChangeOwnPassword);

            permHs.Add(UserPermission.ChangeNormalUserEmail);
            permHs.Add(UserPermission.ResetNormalUserPassword);

            permHs.Add(UserPermission.LockoutNormalUser);
            permHs.Add(UserPermission.DeleteNormalUser);
            permHs.Add(UserPermission.DisableNormalUser);

            permHs.Add(UserPermission.ResetLostPassword);
        }

        if (roles.Contains(ROLE_normal))
        {
            permHs.Add(UserPermission.ChangeOwnEmail);
            permHs.Add(UserPermission.ChangeOwnPassword);

            permHs.Add(UserPermission.ResetLostPassword);
        }

        return permHs;
    }

    /// <summary>
    /// Retrieve max role from given list of roles.
    /// </summary>
    public static string? MaxRole(IEnumerable<string> roles)
    {
        if (roles.Contains(ROLE_admin)) return ROLE_admin;
        if (roles.Contains(ROLE_advanced)) return ROLE_advanced;
        if (roles.Contains(ROLE_normal)) return ROLE_normal;

        return null;
    }

}