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

import { mapValues } from '../runtime';
/**
 * Edit user data
 * @export
 * @interface EditUserRequestDto
 */
export interface EditUserRequestDto {
    /**
     * True for new user, false to change existing one.
     * @type {boolean}
     * @memberof EditUserRequestDto
     */
    createNew: boolean;
    /**
     * User name.
     * @type {string}
     * @memberof EditUserRequestDto
     */
    userName: string | null;
    /**
     * User email.
     * @type {string}
     * @memberof EditUserRequestDto
     */
    email: string | null;
    /**
     * Non null password to change the password.
     * @type {string}
     * @memberof EditUserRequestDto
     */
    changePassword?: string | null;
    /**
     * Roles to set to the user.
     * @type {Array<string>}
     * @memberof EditUserRequestDto
     */
    roles: Array<string> | null;
}

/**
 * Check if a given object implements the EditUserRequestDto interface.
 */
export function instanceOfEditUserRequestDto(value: object): value is EditUserRequestDto {
    if (!('createNew' in value) || value['createNew'] === undefined) return false;
    if (!('userName' in value) || value['userName'] === undefined) return false;
    if (!('email' in value) || value['email'] === undefined) return false;
    if (!('roles' in value) || value['roles'] === undefined) return false;
    return true;
}

export function EditUserRequestDtoFromJSON(json: any): EditUserRequestDto {
    return EditUserRequestDtoFromJSONTyped(json, false);
}

export function EditUserRequestDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): EditUserRequestDto {
    if (json == null) {
        return json;
    }
    return {
        
        'createNew': json['createNew'],
        'userName': json['userName'],
        'email': json['email'],
        'changePassword': json['changePassword'] == null ? undefined : json['changePassword'],
        'roles': json['roles'],
    };
}

export function EditUserRequestDtoToJSON(value?: EditUserRequestDto | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'createNew': value['createNew'],
        'userName': value['userName'],
        'email': value['email'],
        'changePassword': value['changePassword'],
        'roles': value['roles'],
    };
}

