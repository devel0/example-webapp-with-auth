import { ThemeOptions, createTheme } from "@mui/material";
import { GlobalState } from "../redux/states/GlobalState";
import { TypographyOptions } from "@mui/material/styles/createTypography";

// https://fontsource.org/fonts/roboto
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";
import { THEME_DARK, THEME_LIGHT, light_bg_paper } from "../constants/general";
import { setCssVars } from "./CssVars";

// palette { primary, secondary, error, warning, info, success }

export const Colors = {
  primary: "primary.main",
  secondary: "secondary.main",
  error: "error.main",
  warning: "warning.main",
  info: "info.main",
  success: "success.main",
};

export const evalThemeChanged = (global: GlobalState) => {
  const commonThemeOptions: ThemeOptions = {
    typography: {
      fontFamily: "Roboto",
      // fontWeightRegular: '300',

      h4: {
        fontWeight: 500
      },

      button: {
        textTransform: "none",
      },
    } as TypographyOptions,
  };

  //
  // DARK THEME
  //
  const darkThemeOptions: ThemeOptions = {
    ...commonThemeOptions,

    palette: {
      mode: THEME_DARK,
    },
  };

  //
  // LIGHT THEME
  //
  const lightThemeOptions: ThemeOptions = {
    ...commonThemeOptions,

    palette: {
      mode: THEME_LIGHT,

      background: {        
        paper: light_bg_paper,
      },
    },
  };

  const isLightTheme = global.theme === THEME_LIGHT;

  const theme = createTheme(
    isLightTheme ? lightThemeOptions : darkThemeOptions
  );

  setCssVars(isLightTheme);

  return theme;
};
