import { API_URL, APP_URL_Login } from "./constants/general"
import { AuthApi, Configuration, MainApi } from "../api"
import { setSnack } from "./utils/utils";
import { useEffect } from "react";
import { useGlobalPersistService } from "./services/globalPersistService";
import { useGlobalService } from "./services/globalService";
import axios, { AxiosError, HttpStatusCode } from "axios";

export const useAxiosConfig = () => {
  const setGeneralNetwork = useGlobalService(x => x.setGeneralNetwork)
  
  const globalPersistState = useGlobalPersistService()

  useEffect(() => {
    axios.defaults.withCredentials = true

    axios.interceptors.request.use(
      async (config) => {
        setGeneralNetwork(true)

        return config
      },

      (error) => {
        Promise.reject(error)
      }
    )

    axios.interceptors.response.use(
      (response) => {
        setGeneralNetwork(false)

        return response
      },

      (error: AxiosError) => {
        setGeneralNetwork(false)

        if (error?.response?.status === HttpStatusCode.Unauthorized) {
          if (document.location.pathname !== APP_URL_Login()) {
            globalPersistState.setLogout()
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

  }, [])

}

export const authApi = new AuthApi(new Configuration({
  basePath: API_URL()
}))

export const mainApi = new MainApi(new Configuration({
  basePath: API_URL()
}))
