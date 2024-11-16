import "@fontsource/roboto/400.css";
import {
  CSS_VAR_APP_BUTTON_BG, CSS_VAR_APP_BUTTON_FG, CSS_VAR_APP_CONTROL_BG,
  CSS_VAR_TOOLBAR_TEXT_COLOR, light_bg_paper
} from "../constants/general";

export const setCssVars = (isLightTheme: boolean) => {
  const root = document.documentElement;

  root?.style.setProperty(
    CSS_VAR_TOOLBAR_TEXT_COLOR,
    isLightTheme ? "white" : "#1f7bd5"
  );

  root?.style.setProperty(
    CSS_VAR_APP_BUTTON_FG,
    isLightTheme ? "#2f2f2f" : "white"
  );

  root?.style.setProperty(
    CSS_VAR_APP_BUTTON_BG,
    isLightTheme ? light_bg_paper : "#2b2b2b"
  );

  root?.style.setProperty(
    CSS_VAR_APP_CONTROL_BG,
    isLightTheme ? light_bg_paper : "#212121"
  );

};
