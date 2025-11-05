# MainApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiMainLongRunningGet**](#apimainlongrunningget) | **GET** /api/Main/LongRunning | Long running api test. ( admin and users allowed )|
|[**apiMainTestExceptionGet**](#apimaintestexceptionget) | **GET** /api/Main/TestException | Generate test exception. ( only admin )|

# **apiMainLongRunningGet**
> apiMainLongRunningGet()


### Example

```typescript
import {
    MainApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new MainApi(configuration);

const { status, data } = await apiInstance.apiMainLongRunningGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiMainTestExceptionGet**
> apiMainTestExceptionGet()


### Example

```typescript
import {
    MainApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new MainApi(configuration);

const { status, data } = await apiInstance.apiMainTestExceptionGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

