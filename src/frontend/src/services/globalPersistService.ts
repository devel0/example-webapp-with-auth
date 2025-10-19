import { create } from "zustand"
import { GlobalState } from "./abstractions/GlobalState"
import { GlobalPersistState, GlobalPersistStateInitial } from "./abstractions/GlobalPersistState"
import { createJSONStorage, persist } from "zustand/middleware"
import { UserPermission } from "../../api"
import { useGlobalService } from "./globalService"
import { LOCAL_STORAGE_DATA } from "../constants/general"

export const useGlobalPersistService = create<GlobalPersistState>()(
    persist(
        (set, get) => ({
            ...GlobalPersistStateInitial,

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