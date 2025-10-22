import { create } from "zustand"
import { createJSONStorage, persist } from "zustand/middleware"
import { IGlobalPersistActions } from "./IActions"
import { IGlobalPersistData } from "./IData"
import { LOCAL_STORAGE_DATA, THEME_INITIAL } from "../../constants/general"
import { UserPermission } from "../../../api"

export type IGlobalPersistService = IGlobalPersistData & IGlobalPersistActions

const InitialData: IGlobalPersistData = {
    hydrated: false,

    currentUser: null,
    currentUserInitialized: false,

    themePalette: THEME_INITIAL,

    refreshTokenExpiration: null
}

export const useGlobalPersistService = create<IGlobalPersistService>()(
    persist(
        (set, get) => ({
            ...InitialData,

            setHydrated() {
                set(state => ({ hydrated: true }))
            },

            setCurrentUser(currentUser) {
                set(state => ({
                    currentUser,
                    currentUserInitialized: true
                }))
            },

            setRefreshTokenExpiration(refreshTokenExpiration) {
                set(state => ({ refreshTokenExpiration }))
            },

            setLogout() {
                set(state => ({ currentUser: null }))
            },

            setThemePalette(themePalette) {
                set(state => ({ themePalette }))
            },

            currentUserCanManageUsers() {
                return get().currentUser?.permissions.indexOf(UserPermission.CreateNormalUser) !== -1
            }

        }),

        {
            name: LOCAL_STORAGE_DATA,
            storage: createJSONStorage(() => localStorage),
            onRehydrateStorage(state) {
                return () => state.setHydrated()
            },
        }
    )
)