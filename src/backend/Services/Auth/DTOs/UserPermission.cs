namespace ExampleWebApp.Backend.WebApi.Services.Auth.DTOs;

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

    /// <summary>
    /// Generate an email with a reset password token.
    /// </summary>
    ResetLostPassword

}
