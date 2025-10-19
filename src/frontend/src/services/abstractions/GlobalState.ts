// DATA

import { computeIsMobile } from "../../utils/utils"

interface GlobalStateData {
    urlWanted: string | null,
    generalNetwork: boolean,
    isMobile: boolean
}

export const GlobalStateDataInitial: GlobalStateData = {
    urlWanted: null,
    generalNetwork: false,
    isMobile: computeIsMobile()
}

// ACTIONS

interface GlobalStateActions {
    setUrlWanted: (urlWanted: string | null) => void
    setGeneralNetwork: (generalNetwork: boolean) => void
    setIsMobile: (isMobile: boolean) => void
}

export interface GlobalState extends GlobalStateData, GlobalStateActions { }