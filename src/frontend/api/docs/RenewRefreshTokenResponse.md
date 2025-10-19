# RenewRefreshTokenResponse

M:ExampleWebApp.Backend.WebApi.AuthController.RenewRefreshToken api response data.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**status** | [**RenewRefreshTokenStatus**](RenewRefreshTokenStatus.md) |  | [default to undefined]
**refreshTokenNfo** | [**RefreshTokenNfo**](RefreshTokenNfo.md) |  | [optional] [default to undefined]

## Example

```typescript
import { RenewRefreshTokenResponse } from './api';

const instance: RenewRefreshTokenResponse = {
    status,
    refreshTokenNfo,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
