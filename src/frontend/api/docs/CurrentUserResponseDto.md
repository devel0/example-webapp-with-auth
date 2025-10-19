# CurrentUserResponseDto

M:ExampleWebApp.Backend.WebApi.AuthController.CurrentUser api response data.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**status** | [**CurrentUserStatus**](CurrentUserStatus.md) |  | [default to undefined]
**userName** | **string** | Login username. | [optional] [default to undefined]
**email** | **string** | Email address. | [optional] [default to undefined]
**roles** | **Set&lt;string&gt;** | List of roles associated to this user. | [optional] [default to undefined]
**permissions** | [**Set&lt;UserPermission&gt;**](UserPermission.md) | Permissions related to this user roles. | [optional] [default to undefined]

## Example

```typescript
import { CurrentUserResponseDto } from './api';

const instance: CurrentUserResponseDto = {
    status,
    userName,
    email,
    roles,
    permissions,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
