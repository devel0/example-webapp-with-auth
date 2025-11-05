# RenewAccessTokenResponse

M:ExampleWebApp.Backend.WebApi.AuthController.RenewAccessToken api response data.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**status** | [**RenewAccessTokenStatus**](RenewAccessTokenStatus.md) |  | [default to undefined]
**accessTokenNfo** | [**AccessTokenNfo**](AccessTokenNfo.md) |  | [optional] [default to undefined]

## Example

```typescript
import { RenewAccessTokenResponse } from './api';

const instance: RenewAccessTokenResponse = {
    status,
    accessTokenNfo,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
