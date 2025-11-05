# AuthApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiAuthAuthOptionsGet**](#apiauthauthoptionsget) | **GET** /api/Auth/AuthOptions | Retrieve auth options.|
|[**apiAuthCurrentUserGet**](#apiauthcurrentuserget) | **GET** /api/Auth/CurrentUser | Retrieve current logged in user name, email, roles.|
|[**apiAuthDeleteUserPost**](#apiauthdeleteuserpost) | **POST** /api/Auth/DeleteUser | Delete user.|
|[**apiAuthEditUserPost**](#apiauthedituserpost) | **POST** /api/Auth/EditUser | Edit user data|
|[**apiAuthListRolesGet**](#apiauthlistrolesget) | **GET** /api/Auth/ListRoles | List all roles.|
|[**apiAuthListUsersGet**](#apiauthlistusersget) | **GET** /api/Auth/ListUsers | List all users or specific if param given.|
|[**apiAuthLoginPost**](#apiauthloginpost) | **POST** /api/Auth/Login | Login user by given username or email and auth password.|
|[**apiAuthLogoutGet**](#apiauthlogoutget) | **GET** /api/Auth/Logout | Logout current user.|
|[**apiAuthRenewAccessTokenPost**](#apiauthrenewaccesstokenpost) | **POST** /api/Auth/RenewAccessToken | |
|[**apiAuthRenewRefreshTokenPost**](#apiauthrenewrefreshtokenpost) | **POST** /api/Auth/RenewRefreshToken | Renew refresh token of current user if refresh token still valid. This is used to extends refresh token duration avoiding closing frontend session.|
|[**apiAuthResetLostPasswordGet**](#apiauthresetlostpasswordget) | **GET** /api/Auth/ResetLostPassword | Reset lost password.|

# **apiAuthAuthOptionsGet**
> AuthOptions apiAuthAuthOptionsGet()


### Example

```typescript
import {
    AuthApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

const { status, data } = await apiInstance.apiAuthAuthOptionsGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**AuthOptions**

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

# **apiAuthCurrentUserGet**
> CurrentUserResponseDto apiAuthCurrentUserGet()


### Example

```typescript
import {
    AuthApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

const { status, data } = await apiInstance.apiAuthCurrentUserGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**CurrentUserResponseDto**

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

# **apiAuthDeleteUserPost**
> DeleteUserResponseDto apiAuthDeleteUserPost()


### Example

```typescript
import {
    AuthApi,
    Configuration,
    DeleteUserRequestDto
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

let deleteUserRequestDto: DeleteUserRequestDto; // (optional)

const { status, data } = await apiInstance.apiAuthDeleteUserPost(
    deleteUserRequestDto
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **deleteUserRequestDto** | **DeleteUserRequestDto**|  | |


### Return type

**DeleteUserResponseDto**

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

# **apiAuthEditUserPost**
> EditUserResponseDto apiAuthEditUserPost()


### Example

```typescript
import {
    AuthApi,
    Configuration,
    EditUserRequestDto
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

let editUserRequestDto: EditUserRequestDto; // (optional)

const { status, data } = await apiInstance.apiAuthEditUserPost(
    editUserRequestDto
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **editUserRequestDto** | **EditUserRequestDto**|  | |


### Return type

**EditUserResponseDto**

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

# **apiAuthListRolesGet**
> Array<string> apiAuthListRolesGet()


### Example

```typescript
import {
    AuthApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

const { status, data } = await apiInstance.apiAuthListRolesGet();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**Array<string>**

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

# **apiAuthListUsersGet**
> Array<UserListItemResponseDto> apiAuthListUsersGet()


### Example

```typescript
import {
    AuthApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

let username: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiAuthListUsersGet(
    username
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **username** | [**string**] |  | (optional) defaults to undefined|


### Return type

**Array<UserListItemResponseDto>**

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

# **apiAuthLoginPost**
> LoginResponseDto apiAuthLoginPost()


### Example

```typescript
import {
    AuthApi,
    Configuration,
    LoginRequestDto
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

let loginRequestDto: LoginRequestDto; // (optional)

const { status, data } = await apiInstance.apiAuthLoginPost(
    loginRequestDto
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **loginRequestDto** | **LoginRequestDto**|  | |


### Return type

**LoginResponseDto**

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

# **apiAuthLogoutGet**
> apiAuthLogoutGet()


### Example

```typescript
import {
    AuthApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

const { status, data } = await apiInstance.apiAuthLogoutGet();
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

# **apiAuthRenewAccessTokenPost**
> RenewAccessTokenResponse apiAuthRenewAccessTokenPost()


### Example

```typescript
import {
    AuthApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

const { status, data } = await apiInstance.apiAuthRenewAccessTokenPost();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**RenewAccessTokenResponse**

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

# **apiAuthRenewRefreshTokenPost**
> RenewRefreshTokenResponse apiAuthRenewRefreshTokenPost()


### Example

```typescript
import {
    AuthApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

const { status, data } = await apiInstance.apiAuthRenewRefreshTokenPost();
```

### Parameters
This endpoint does not have any parameters.


### Return type

**RenewRefreshTokenResponse**

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

# **apiAuthResetLostPasswordGet**
> apiAuthResetLostPasswordGet()


### Example

```typescript
import {
    AuthApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new AuthApi(configuration);

let email: string; // (optional) (default to undefined)
let token: string; // (optional) (default to undefined)
let resetPassword: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiAuthResetLostPasswordGet(
    email,
    token,
    resetPassword
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **email** | [**string**] |  | (optional) defaults to undefined|
| **token** | [**string**] |  | (optional) defaults to undefined|
| **resetPassword** | [**string**] |  | (optional) defaults to undefined|


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

