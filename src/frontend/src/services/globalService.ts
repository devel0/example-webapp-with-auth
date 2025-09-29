import { create } from "zustand"
import { GlobalState, GlobalStateDataInitial } from "./abstractions/GlobalState"

export const useGlobalService = create<GlobalState>(set => {

    // const increment = () => set(state => ({ cnt: state.cnt + 1 }))

    return ({
        ...GlobalStateDataInitial,

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