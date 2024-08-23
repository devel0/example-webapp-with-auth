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
import type { EditUserStatus } from './EditUserStatus';
import {
    EditUserStatusFromJSON,
    EditUserStatusFromJSONTyped,
    EditUserStatusToJSON,
} from './EditUserStatus';

/**
 * 
 * @export
 * @interface EditUserResponseDto
 */
export interface EditUserResponseDto {
    /**
     * 
     * @type {EditUserStatus}
     * @memberof EditUserResponseDto
     */
    status: EditUserStatus;
    /**
     * Roles added.
     * @type {Array<string>}
     * @memberof EditUserResponseDto
     */
    rolesAdded?: Array<string> | null;
    /**
     * Roles removed.
     * @type {Array<string>}
     * @memberof EditUserResponseDto
     */
    rolesRemoved?: Array<string> | null;
    /**
     * List of edit user errors if any.
     * @type {Array<string>}
     * @memberof EditUserResponseDto
     */
    errors?: Array<string> | null;
}

/**
 * Check if a given object implements the EditUserResponseDto interface.
 */
export function instanceOfEditUserResponseDto(value: object): value is EditUserResponseDto {
    if (!('status' in value) || value['status'] === undefined) return false;
    return true;
}

export function EditUserResponseDtoFromJSON(json: any): EditUserResponseDto {
    return EditUserResponseDtoFromJSONTyped(json, false);
}

export function EditUserResponseDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): EditUserResponseDto {
    if (json == null) {
        return json;
    }
    return {
        
        'status': EditUserStatusFromJSON(json['status']),
        'rolesAdded': json['rolesAdded'] == null ? undefined : json['rolesAdded'],
        'rolesRemoved': json['rolesRemoved'] == null ? undefined : json['rolesRemoved'],
        'errors': json['errors'] == null ? undefined : json['errors'],
    };
}

export function EditUserResponseDtoToJSON(value?: EditUserResponseDto | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'status': EditUserStatusToJSON(value['status']),
        'rolesAdded': value['rolesAdded'],
        'rolesRemoved': value['rolesRemoved'],
        'errors': value['errors'],
    };
}

