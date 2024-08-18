import { PayloadAction, createSlice } from "@reduxjs/toolkit";

import { GlobalInitialState } from "../states/GlobalState";
import { SnackNfo, SnackNfoType } from "../../types/SnackNfo";
import { CurrentUserNfo } from "../../types/CurrentUserNfo";
import { PaletteMode } from "@mui/material";
import { LOCAL_STORAGE_THEME } from "../../constants/general";

export const globalSlice = createSlice({
  name: "global",

  initialState: GlobalInitialState,

  reducers: {
    //-----------------------------------------------------------------------------------
    // App
    //-----------------------------------------------------------------------------------    

    setUrlWanted: (state, action: PayloadAction<string | undefined>) => {
      console.log(`setting urlWatned to ${action.payload}`);
      state.urlWanted = action.payload;
    },

    setGeneralNetwork: (state, action: PayloadAction<boolean>) => {
      state.generalNetwork = action.payload;
    },

    setSuccessfulLogin: (state, action: PayloadAction<CurrentUserNfo>) => {
      state.currentUserInitialized = true;
      state.currentUser = action.payload
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

    //-----------------------------------------------------------------------------------
    // Snacks
    //-----------------------------------------------------------------------------------

    setSnack: (state, action: PayloadAction<SnackNfo>) => {
      const nfo = action.payload;

      switch (nfo.type) {
        case SnackNfoType.success:
          state.snackSuccess.title = nfo.title;
          state.snackSuccess.msg = nfo.msg;
          state.snackSuccess.open = true;
          state.snackSuccess.duration = nfo.duration === null ? null : (nfo.duration ?? 6000);
          break;

        case SnackNfoType.info:
          state.snackInfo.title = nfo.title;
          state.snackInfo.msg = nfo.msg;
          state.snackInfo.open = true;
          state.snackInfo.duration = nfo.duration === null ? null : (nfo.duration ?? 6000);
          break;

        case SnackNfoType.warning:
          state.snackWarning.title = nfo.title;
          state.snackWarning.msg = nfo.msg;
          state.snackWarning.open = true;
          state.snackWarning.duration = nfo.duration === null ? null : (nfo.duration ?? 6000);
          break;

        case SnackNfoType.error:
          state.snackError.title = nfo.title;
          state.snackError.msg = nfo.msg;
          state.snackError.open = true;
          state.snackError.duration = nfo.duration === null ? null : (nfo.duration ?? 6000);
          break;
      }
    },

    unsetSnack: (state, action: PayloadAction<SnackNfoType>) => {
      switch (action.payload) {
        case SnackNfoType.success:
          state.snackSuccess.open = false;
          break;

        case SnackNfoType.info:
          state.snackSuccess.open = false;
          break;

        case SnackNfoType.warning:
          state.snackWarning.open = false;
          break;

        case SnackNfoType.error:
          state.snackError.open = false;
          break;
      }
    },


  },

});

export const {

  setUrlWanted,
  setGeneralNetwork,
  setSuccessfulLogin,
  setLoggedOut,
  setTheme,

  setSnack, unsetSnack

} = globalSlice.actions;

export default globalSlice.reducer;
