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
import type { RegisterUserStatus } from './RegisterUserStatus';
import {
    RegisterUserStatusFromJSON,
    RegisterUserStatusFromJSONTyped,
    RegisterUserStatusToJSON,
} from './RegisterUserStatus';
import type { IdentityError } from './IdentityError';
import {
    IdentityErrorFromJSON,
    IdentityErrorFromJSONTyped,
    IdentityErrorToJSON,
} from './IdentityError';

/**
 * M:ExampleWebApp.Backend.WebApi.AuthController.RegisterUser(ExampleWebApp.Backend.WebApi.RegisterUserRequestDto) api response data.
 * @export
 * @interface RegisterUserResponseDto
 */
export interface RegisterUserResponseDto {
    /**
     * 
     * @type {RegisterUserStatus}
     * @memberof RegisterUserResponseDto
     */
    status: RegisterUserStatus;
    /**
     * List of register errors if any.
     * @type {Array<IdentityError>}
     * @memberof RegisterUserResponseDto
     */
    errors: Array<IdentityError> | null;
}

/**
 * Check if a given object implements the RegisterUserResponseDto interface.
 */
export function instanceOfRegisterUserResponseDto(value: object): value is RegisterUserResponseDto {
    if (!('status' in value) || value['status'] === undefined) return false;
    if (!('errors' in value) || value['errors'] === undefined) return false;
    return true;
}

export function RegisterUserResponseDtoFromJSON(json: any): RegisterUserResponseDto {
    return RegisterUserResponseDtoFromJSONTyped(json, false);
}

export function RegisterUserResponseDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): RegisterUserResponseDto {
    if (json == null) {
        return json;
    }
    return {
        
        'status': RegisterUserStatusFromJSON(json['status']),
        'errors': (json['errors'] == null ? null : (json['errors'] as Array<any>).map(IdentityErrorFromJSON)),
    };
}

export function RegisterUserResponseDtoToJSON(value?: RegisterUserResponseDto | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'status': RegisterUserStatusToJSON(value['status']),
        'errors': (value['errors'] == null ? null : (value['errors'] as Array<any>).map(IdentityErrorToJSON)),
    };
}

