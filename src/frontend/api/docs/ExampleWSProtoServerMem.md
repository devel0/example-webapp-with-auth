# ExampleWSProtoServerMem


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**baseProtocolType** | [**BaseWSProtocolType**](BaseWSProtocolType.md) |  | [optional] [default to undefined]
**baseProtocolMsg** | **string** | (don\&#39;t care) : for internal protocol purpose | [optional] [default to undefined]
**protocolType** | [**ExampleWSProtocolType**](ExampleWSProtocolType.md) |  | [optional] [default to undefined]
**memoryUsed** | **number** | srv memory used in bytes | [optional] [default to undefined]

## Example

```typescript
import { ExampleWSProtoServerMem } from './api';

const instance: ExampleWSProtoServerMem = {
    baseProtocolType,
    baseProtocolMsg,
    protocolType,
    memoryUsed,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
