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
 * M:ExampleWebApp.Backend.WebApi.AuthController.Login(ExampleWebApp.Backend.WebApi.LoginRequestDto) api request data.
 * @export
 * @interface LoginRequestDto
 */
export interface LoginRequestDto {
    /**
     * Username. If null but ExampleWebApp.Backend.WebApi.LoginRequestDto.Email was given login will be tried within email as user identifier, instead of the username.
     * @type {string}
     * @memberof LoginRequestDto
     */
    userName?: string | null;
    /**
     * Email. Can be null if non null ExampleWebApp.Backend.WebApi.LoginRequestDto.UserName given.
     * @type {string}
     * @memberof LoginRequestDto
     */
    email?: string | null;
    /**
     * Password.
     * @type {string}
     * @memberof LoginRequestDto
     */
    password: string | null;
}

/**
 * Check if a given object implements the LoginRequestDto interface.
 */
export function instanceOfLoginRequestDto(value: object): value is LoginRequestDto {
    if (!('password' in value) || value['password'] === undefined) return false;
    return true;
}

export function LoginRequestDtoFromJSON(json: any): LoginRequestDto {
    return LoginRequestDtoFromJSONTyped(json, false);
}

export function LoginRequestDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): LoginRequestDto {
    if (json == null) {
        return json;
    }
    return {
        
        'userName': json['userName'] == null ? undefined : json['userName'],
        'email': json['email'] == null ? undefined : json['email'],
        'password': json['password'],
    };
}

export function LoginRequestDtoToJSON(value?: LoginRequestDto | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'userName': value['userName'],
        'email': value['email'],
        'password': value['password'],
    };
}

