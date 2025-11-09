export interface IGlobalActions {    
    setGeneralNetwork: (generalNetwork: boolean) => void
    setIsMobile: (isMobile: boolean) => void
    setAppBarHeight: (appBarHeight: number) => void
    setWsConnected: (connected: boolean) => void
}