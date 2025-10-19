# EditUserResponseDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**status** | [**EditUserStatus**](EditUserStatus.md) |  | [default to undefined]
**rolesAdded** | **Array&lt;string&gt;** | Roles added. | [optional] [default to undefined]
**rolesRemoved** | **Array&lt;string&gt;** | Roles removed. | [optional] [default to undefined]
**errors** | **Array&lt;string&gt;** | List of edit user errors if any. | [optional] [default to undefined]

## Example

```typescript
import { EditUserResponseDto } from './api';

const instance: EditUserResponseDto = {
    status,
    rolesAdded,
    rolesRemoved,
    errors,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
