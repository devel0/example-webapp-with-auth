/* tslint:disable */
/* eslint-disable */
/**
 * ExampleWebApp API
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


/**
 * 
 * 
 * OK (Delete user ok.)
 * 
 * IdentityError (asp net core IdentityError, see Errors for details.)
 * 
 * UserNotFound (User not found.)
 * 
 * CannotDeleteLastActiveAdmin (Cannot delete last non disabled admin user.)
 * 
 * PermissionsError
 * @export
 */
export const DeleteUserStatus = {
    OK: 'OK',
    IdentityError: 'IdentityError',
    UserNotFound: 'UserNotFound',
    CannotDeleteLastActiveAdmin: 'CannotDeleteLastActiveAdmin',
    PermissionsError: 'PermissionsError'
} as const;
export type DeleteUserStatus = typeof DeleteUserStatus[keyof typeof DeleteUserStatus];


export function instanceOfDeleteUserStatus(value: any): boolean {
    for (const key in DeleteUserStatus) {
        if (Object.prototype.hasOwnProperty.call(DeleteUserStatus, key)) {
            if ((DeleteUserStatus as Record<string, DeleteUserStatus>)[key] === value) {
                return true;
            }
        }
    }
    return false;
}

export function DeleteUserStatusFromJSON(json: any): DeleteUserStatus {
    return DeleteUserStatusFromJSONTyped(json, false);
}

export function DeleteUserStatusFromJSONTyped(json: any, ignoreDiscriminator: boolean): DeleteUserStatus {
    return json as DeleteUserStatus;
}

export function DeleteUserStatusToJSON(value?: DeleteUserStatus | null): any {
    return value as any;
}

