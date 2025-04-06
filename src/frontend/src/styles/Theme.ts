import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { GlobalState } from "../redux/states/GlobalState";
import { orange } from "@mui/material/colors";
import { setCssVars } from "./CssVars";
import { THEME_DARK, THEME_LIGHT, light_bg_paper } from "../constants/general";
import { ThemeOptions, createTheme } from "@mui/material";
import { TypographyOptions } from "@mui/material/styles/createTypography";

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

    // https://github.com/mui/material-ui/issues/26607#issuecomment-856595593
    components: {
      MuiInputLabel: {
        defaultProps: { shrink: true }
      },
      MuiOutlinedInput: {
        defaultProps: {
          notched: true
        }
      }
    }
  };

  //
  // DARK THEME
  //
  const darkThemeOptions: ThemeOptions = {
    ...commonThemeOptions,

    palette: {
      mode: THEME_DARK,
      error: {
        main: orange[400]
      }
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
