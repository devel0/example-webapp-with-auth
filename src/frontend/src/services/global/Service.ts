import { create } from "zustand"
import { computeIsMobile } from "../../utils/utils"
import { IGlobalActions } from "./IActions"
import { IGlobalData } from "./IData"

export type IGlobalService = IGlobalData & IGlobalActions

const InitialData: IGlobalData = {
    urlWanted: null,
    generalNetwork: false,
    isMobile: computeIsMobile()
}

export const useGlobalService = create<IGlobalService>(set => {
    
    return ({
        ...InitialData,

        setUrlWanted: (urlWanted) => {
            set(state => ({ urlWanted }))
        },

        setGeneralNetwork: (generalNetwork) => {
            set(state => ({ generalNetwork }))
        },

        setIsMobile: (isMobile) => {
            set(state => ({ isMobile }))
        },

    })
})
