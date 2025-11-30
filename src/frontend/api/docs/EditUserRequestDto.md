# EditUserRequestDto

Edit user data

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**existingUsername** | **string** | Existing User name or null to create a new one using ExampleWebApp.Backend.WebApi.Services.Auth.DTOs.EditUserRequestDto.EditUsername. | [optional] [default to undefined]
**editEmail** | **string** | New email or null to leave unchanged. | [optional] [default to undefined]
**editUsername** | **string** | New username or null to leave unchanged. | [optional] [default to undefined]
**editPassword** | **string** | New password or null to leave unchanged. | [optional] [default to undefined]
**editRoles** | **Array&lt;string&gt;** | Roles to set to the user or null to leave unchanged. | [optional] [default to undefined]
**editDisabled** | **boolean** | If true the user can\&#39;t login after previous release access token expires. | [optional] [default to undefined]
**editLockoutEnd** | **string** | Set the end date of lockout. The user will be unable to login until ExampleWebApp.Backend.WebApi.Services.Auth.DTOs.EditUserRequestDto.EditLockoutEnd. If ExampleWebApp.Backend.WebApi.Services.Auth.DTOs.EditUserRequestDto.EditLockoutEnd is set in the past the user will be re-enabled immediately. | [optional] [default to undefined]

## Example

```typescript
import { EditUserRequestDto } from './api';

const instance: EditUserRequestDto = {
    existingUsername,
    editEmail,
    editUsername,
    editPassword,
    editRoles,
    editDisabled,
    editLockoutEnd,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
