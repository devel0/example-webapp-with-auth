import { CurrentUserNfo } from "../../types/CurrentUserNfo";
import { GlobalInitialState } from "../states/GlobalState";
import { LOCAL_STORAGE_CURRENT_USER_NFO, LOCAL_STORAGE_THEME } from "../../constants/general";
import { PaletteMode } from "@mui/material";
import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { UserPermission } from "../../../api";

export const globalSlice = createSlice({
  name: "global",

  initialState: GlobalInitialState,

  reducers: {
    //-----------------------------------------------------------------------------------
    // App
    //-----------------------------------------------------------------------------------    

    setUrlWanted: (state, action: PayloadAction<string | undefined>) => {
      // console.log(`setting urlWatned to ${action.payload}`);
      state.urlWanted = action.payload;
    },

    setGeneralNetwork: (state, action: PayloadAction<boolean>) => {
      state.generalNetwork = action.payload;
    },

    setSuccessfulLogin: (state, action: PayloadAction<CurrentUserNfo>) => {
      state.currentUserInitialized = true;

      const currentUser = action.payload
      state.currentUser = currentUser
      state.currentUserCanManageUsers = currentUser.permissions.indexOf(UserPermission.CreateNormalUser) !== -1      
    },

    setLoggedOut: (state) => {
      state.urlWanted = undefined
      state.currentUser = undefined
      localStorage.removeItem(LOCAL_STORAGE_CURRENT_USER_NFO)
    },

    setTheme: (state, action: PayloadAction<PaletteMode>) => {
      const theme = action.payload;
      state.theme = theme;

      localStorage.setItem(LOCAL_STORAGE_THEME, theme);

      document.body.style.colorScheme = theme
    },     

    setIsMobile: (state, action: PayloadAction<boolean>) => {
      state.isMobile = action.payload
    },

  },

});

export const {

  setUrlWanted,
  setGeneralNetwork,
  setSuccessfulLogin,
  setLoggedOut,
  setTheme,    
  setIsMobile

} = globalSlice.actions;

export default globalSlice.reducer;
