# UserListItemResponseDto

M:ExampleWebApp.Backend.WebApi.AuthController.ListUsers(System.String) api response data.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**userName** | **string** | User name. | [default to undefined]
**email** | **string** | User email. | [default to undefined]
**roles** | **Array&lt;string&gt;** | User roles. | [default to undefined]
**disabled** | **boolean** |  | [default to undefined]
**accessFailedCount** | **number** | Access failed count. | [default to undefined]
**emailConfirmed** | **boolean** | Email is confirmed. | [default to undefined]
**lockoutEnd** | **string** | Lockout end (UTC). ExampleWebApp.Backend.WebApi.Services.Auth.DTOs.UserListItemResponseDto.LockoutEnabled. | [default to undefined]
**lockoutEnabled** | **boolean** | If true the user is lockout until ExampleWebApp.Backend.WebApi.Services.Auth.DTOs.UserListItemResponseDto.LockoutEnd. | [default to undefined]
**phoneNumber** | **string** | User phone number. | [default to undefined]
**phoneNumberConfirmed** | **boolean** | User phone number confirmed. | [default to undefined]
**twoFactorEnabled** | **boolean** | Two factor enabled. | [default to undefined]

## Example

```typescript
import { UserListItemResponseDto } from './api';

const instance: UserListItemResponseDto = {
    userName,
    email,
    roles,
    disabled,
    accessFailedCount,
    emailConfirmed,
    lockoutEnd,
    lockoutEnabled,
    phoneNumber,
    phoneNumberConfirmed,
    twoFactorEnabled,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
