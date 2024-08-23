namespace ExampleWebApp.Backend.WebApi.Types;

public enum UserPermission
{

    /// <summary>
    /// Change another user roles.
    /// </summary>
    ChangeUserRoles,

    /// <summary>
    /// Can create another user with "admin" role.
    /// </summary>
    CreateAdminUser,

    /// <summary>
    /// Can create another user with "advanced" role.
    /// </summary>
    CreateAdvancedUser,

    /// <summary>
    /// Can create another user with "normal" role.
    /// </summary>
    CreateNormalUser,

    /// <summary>
    /// Change its own email (without need to validate).
    /// </summary>
    ChangeOwnEmail,

    /// <summary>
    /// Change its own password (by reset).
    /// </summary>
    ChangeOwnPassword,

    /// <summary>
    /// Change email of another user which max role is "normal".
    /// </summary>
    ChangeNormalUserEmail,

    /// <summary>
    /// Change email of another user which max role is "advanced".
    /// </summary>
    ChangeAdvancedUserEmail,

    /// <summary>
    /// Change email of another user which max role is "admin".
    /// </summary>
    ChangeAdminUserEmail,

    /// <summary>
    /// Reset password of another user which max role is "normal".
    /// </summary>
    ResetNormalUserPassword,

    /// <summary>
    /// Reset password of another user which max role is "advanced".
    /// </summary>
    ResetAdvancedUserPassword,

    /// <summary>
    /// Reset password of another user which max role is "normal".
    /// </summary>
    ResetAdminUserPassword,

    /// <summary>
    /// Edit lockout of another user which max role is "admin".
    /// </summary>
    LockoutAdminUser,

    /// <summary>
    /// Edit lockout of another user which max role is "advanced".
    /// </summary>
    LockoutAdvancedUser,

    /// <summary>
    /// Edit lockout of another user which max role is "normal".
    /// </summary>
    LockoutNormalUser,

    /// <summary>
    /// Delete user which max role is "admin".
    /// </summary>
    DeleteAdminUser,

    /// <summary>
    /// Delete user which max role is "advanced".
    /// </summary>
    DeleteAdvancedUser,

    /// <summary>
    /// Delete user which max role is "normal".
    /// </summary>
    DeleteNormalUser,

    /// <summary>
    /// Disable user which max role is "admin".
    /// </summary>
    DisableAdminUser,

    /// <summary>
    /// Disable user which max role is "advanced".
    /// </summary>
    DisableAdvancedUser,

    /// <summary>
    /// Disable user which max role is "normal".
    /// </summary>
    DisableNormalUser,

}

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
        }

        if (roles.Contains(ROLE_normal))
        {
            permHs.Add(UserPermission.ChangeOwnEmail);
            permHs.Add(UserPermission.ChangeOwnPassword);
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