# FakeDataApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiFakeDataCountFakeDatasPost**](#apifakedatacountfakedataspost) | **POST** /api/FakeData/CountFakeDatas | count items with optional filtering|
|[**apiFakeDataDeleteFakeDataDelete**](#apifakedatadeletefakedatadelete) | **DELETE** /api/FakeData/DeleteFakeData | delete record by id|
|[**apiFakeDataGetFakeDataByIdPost**](#apifakedatagetfakedatabyidpost) | **POST** /api/FakeData/GetFakeDataById | get record by id|
|[**apiFakeDataGetFakeDatasPost**](#apifakedatagetfakedataspost) | **POST** /api/FakeData/GetFakeDatas | get items with optional filtering, sorting|
|[**apiFakeDataUpdateFakeDataPost**](#apifakedataupdatefakedatapost) | **POST** /api/FakeData/UpdateFakeData | update given record|

# **apiFakeDataCountFakeDatasPost**
> number apiFakeDataCountFakeDatasPost()


### Example

```typescript
import {
    FakeDataApi,
    Configuration,
    CountGenericRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new FakeDataApi(configuration);

let countGenericRequest: CountGenericRequest; // (optional)

const { status, data } = await apiInstance.apiFakeDataCountFakeDatasPost(
    countGenericRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **countGenericRequest** | **CountGenericRequest**|  | |


### Return type

**number**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiFakeDataDeleteFakeDataDelete**
> apiFakeDataDeleteFakeDataDelete()


### Example

```typescript
import {
    FakeDataApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new FakeDataApi(configuration);

let id: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiFakeDataDeleteFakeDataDelete(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | (optional) defaults to undefined|


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

# **apiFakeDataGetFakeDataByIdPost**
> FakeData apiFakeDataGetFakeDataByIdPost()


### Example

```typescript
import {
    FakeDataApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new FakeDataApi(configuration);

let id: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiFakeDataGetFakeDataByIdPost(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | (optional) defaults to undefined|


### Return type

**FakeData**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiFakeDataGetFakeDatasPost**
> Array<FakeData> apiFakeDataGetFakeDatasPost()


### Example

```typescript
import {
    FakeDataApi,
    Configuration,
    GetGenericRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new FakeDataApi(configuration);

let getGenericRequest: GetGenericRequest; // (optional)

const { status, data } = await apiInstance.apiFakeDataGetFakeDatasPost(
    getGenericRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **getGenericRequest** | **GetGenericRequest**|  | |


### Return type

**Array<FakeData>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiFakeDataUpdateFakeDataPost**
> FakeData apiFakeDataUpdateFakeDataPost()


### Example

```typescript
import {
    FakeDataApi,
    Configuration,
    FakeDataGenericItemWithOrig
} from './api';

const configuration = new Configuration();
const apiInstance = new FakeDataApi(configuration);

let fakeDataGenericItemWithOrig: FakeDataGenericItemWithOrig; // (optional)

const { status, data } = await apiInstance.apiFakeDataUpdateFakeDataPost(
    fakeDataGenericItemWithOrig
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **fakeDataGenericItemWithOrig** | **FakeDataGenericItemWithOrig**|  | |


### Return type

**FakeData**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

