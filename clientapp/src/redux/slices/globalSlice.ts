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

      switch (action.payload.type) {
        case SnackNfoType.success:
          state.snackSuccessMsg = action.payload.msg;
          state.snackSuccessOpen = true;
          state.snackSuccessDuration =
            action.payload.duration === null
              ? null
              : action.payload.duration ?? 6000;
          break;

        case SnackNfoType.info:
          state.snackInfoMsg = action.payload.msg;
          state.snackInfoOpen = true;
          state.snackInfoDuration =
            action.payload.duration === null
              ? null
              : action.payload.duration ?? 6000;
          break;

        case SnackNfoType.warning:
          state.snackWarningMsg = action.payload.msg;
          state.snackWarningOpen = true;
          state.snackWarningDuration =
            action.payload.duration === null
              ? null
              : action.payload.duration ?? 6000;
          break;

        case SnackNfoType.error:
          state.snackErrorMsg = action.payload.msg;
          state.snackErrorOpen = true;
          state.snackErrorDuration =
            action.payload.duration === null
              ? null
              : action.payload.duration ?? 6000;
          break;
      }
    },

    unsetSnack: (state, action: PayloadAction<SnackNfoType>) => {
      switch (action.payload) {
        case SnackNfoType.success:
          state.snackSuccessOpen = false;
          break;

        case SnackNfoType.info:
          state.snackInfoOpen = false;
          break;

        case SnackNfoType.warning:
          state.snackWarningOpen = false;
          break;

        case SnackNfoType.error:
          state.snackErrorOpen = false;
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
