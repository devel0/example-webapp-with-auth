# LoginRequestDto

M:ExampleWebApp.Backend.WebApi.AuthController.Login(ExampleWebApp.Backend.WebApi.Services.Auth.DTOs.LoginRequestDto) api request data.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**usernameOrEmail** | **string** | Username or email. | [optional] [default to undefined]
**password** | **string** | Password. | [default to undefined]
**passwordResetToken** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { LoginRequestDto } from './api';

const instance: LoginRequestDto = {
    usernameOrEmail,
    password,
    passwordResetToken,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
