# LoginResponseDto

M:ExampleWebApp.Backend.WebApi.AuthController.Login(ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.DTOs.LoginRequestDto) api response data.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**status** | [**LoginStatus**](LoginStatus.md) |  | [default to undefined]
**userName** | **string** | Username. | [optional] [default to undefined]
**email** | **string** | Email. | [optional] [default to undefined]
**roles** | **Array&lt;string&gt;** | User roles. | [optional] [default to undefined]
**errors** | **Array&lt;string&gt;** |  | [optional] [default to undefined]
**permissions** | [**Set&lt;UserPermission&gt;**](UserPermission.md) | Permissions related to this user roles. | [optional] [default to undefined]
**refreshTokenExpiration** | **string** | Expiration timestamp for the refresh token. To keep alive auth issue M:ExampleWebApp.Backend.WebApi.AuthController.RenewRefreshToken before   token expire. | [optional] [default to undefined]

## Example

```typescript
import { LoginResponseDto } from './api';

const instance: LoginResponseDto = {
    status,
    userName,
    email,
    roles,
    errors,
    permissions,
    refreshTokenExpiration,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
