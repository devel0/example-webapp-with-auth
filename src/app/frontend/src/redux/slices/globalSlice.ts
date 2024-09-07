import { PayloadAction, createSlice, current } from "@reduxjs/toolkit";

import { GlobalInitialState } from "../states/GlobalState";
import { CurrentUserNfo } from "../../types/CurrentUserNfo";
import { PaletteMode } from "@mui/material";
import { LOCAL_STORAGE_THEME } from "../../constants/general";
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
      state.currentUser = undefined
    },

    setTheme: (state, action: PayloadAction<PaletteMode>) => {
      const theme = action.payload;
      state.theme = theme;

      localStorage.setItem(LOCAL_STORAGE_THEME, theme);

      document.body.style.colorScheme = theme
    },     

  },

});

export const {

  setUrlWanted,
  setGeneralNetwork,
  setSuccessfulLogin,
  setLoggedOut,
  setTheme,    

} = globalSlice.actions;

export default globalSlice.reducer;
