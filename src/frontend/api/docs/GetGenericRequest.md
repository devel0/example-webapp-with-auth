# GetGenericRequest

get requeste

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**offset** | **number** | paged from | [optional] [default to undefined]
**count** | **number** | paged size (-1 disabled) | [optional] [default to undefined]
**dynFilter** | **string** | ef core dynamic filter | [optional] [default to undefined]
**sort** | [**GenericSort**](GenericSort.md) |  | [optional] [default to undefined]

## Example

```typescript
import { GetGenericRequest } from './api';

const instance: GetGenericRequest = {
    offset,
    count,
    dynFilter,
    sort,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
