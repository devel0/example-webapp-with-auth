import { API_URL, APP_URL_Login, LOCAL_STORAGE_CURRENT_USER_NFO } from "./constants/general"
import { AuthApi, Configuration, MainApi } from "../api"
import { setGeneralNetwork } from "./redux/slices/globalSlice";
import { setSnack } from "./utils/utils";
import { store } from "./redux/stores/store";
import axios, { AxiosError, HttpStatusCode } from "axios";

export const ConfigAxios = () => {

  axios.defaults.withCredentials = true

  axios.interceptors.request.use(
    async (config) => {
      store.dispatch(setGeneralNetwork(true))

      return config
    },

    (error) => {
      Promise.reject(error)
    }
  )

  axios.interceptors.response.use(
    (response) => {
      store.dispatch(setGeneralNetwork(false))

      return response
    },

    (error: AxiosError) => {
      store.dispatch(setGeneralNetwork(false))

      if (error?.response?.status === HttpStatusCode.Unauthorized) {
        if (document.location.pathname !== APP_URL_Login()) {
          localStorage.removeItem(LOCAL_STORAGE_CURRENT_USER_NFO)
          document.location = APP_URL_Login()
        }

        return
      }
      else if (error?.response?.status === HttpStatusCode.BadGateway) {
        setSnack({
          title: "Network error",
          msg: ["Backend server unreachable"],
          type: 'error'
        })

        return
      }
      // else handleApiException(error)

      return Promise.reject(error);
    }
  )

}


export const authApi = new AuthApi(new Configuration({
  basePath: API_URL()
}))

export const mainApi = new MainApi(new Configuration({
  basePath: API_URL()
}))
