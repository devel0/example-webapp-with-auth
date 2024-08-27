import { AuthApi, Configuration, MainApi } from "../api"
import { API_URL } from "./constants/general"
import { setGeneralNetwork } from "./redux/slices/globalSlice";
import axios, { AxiosError, HttpStatusCode } from "axios";
import { store } from "./redux/stores/store";
import { setSnack } from "./utils/utils";

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

        if (error?.response?.status === HttpStatusCode.BadGateway) {
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