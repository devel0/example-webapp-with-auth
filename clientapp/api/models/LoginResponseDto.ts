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
import type { LoginStatus } from './LoginStatus';
import {
    LoginStatusFromJSON,
    LoginStatusFromJSONTyped,
    LoginStatusToJSON,
} from './LoginStatus';

/**
 * M:ExampleWebApp.Backend.WebApi.AuthController.Login(ExampleWebApp.Backend.WebApi.LoginRequestDto) api response data.
 * @export
 * @interface LoginResponseDto
 */
export interface LoginResponseDto {
    /**
     * 
     * @type {LoginStatus}
     * @memberof LoginResponseDto
     */
    status: LoginStatus;
    /**
     * Username.
     * @type {string}
     * @memberof LoginResponseDto
     */
    userName?: string | null;
    /**
     * Email.
     * @type {string}
     * @memberof LoginResponseDto
     */
    email?: string | null;
    /**
     * User roles.
     * @type {Array<string>}
     * @memberof LoginResponseDto
     */
    roles?: Array<string> | null;
}

/**
 * Check if a given object implements the LoginResponseDto interface.
 */
export function instanceOfLoginResponseDto(value: object): value is LoginResponseDto {
    if (!('status' in value) || value['status'] === undefined) return false;
    return true;
}

export function LoginResponseDtoFromJSON(json: any): LoginResponseDto {
    return LoginResponseDtoFromJSONTyped(json, false);
}

export function LoginResponseDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): LoginResponseDto {
    if (json == null) {
        return json;
    }
    return {
        
        'status': LoginStatusFromJSON(json['status']),
        'userName': json['userName'] == null ? undefined : json['userName'],
        'email': json['email'] == null ? undefined : json['email'],
        'roles': json['roles'] == null ? undefined : json['roles'],
    };
}

export function LoginResponseDtoToJSON(value?: LoginResponseDto | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'status': LoginStatusToJSON(value['status']),
        'userName': value['userName'],
        'email': value['email'],
        'roles': value['roles'],
    };
}
