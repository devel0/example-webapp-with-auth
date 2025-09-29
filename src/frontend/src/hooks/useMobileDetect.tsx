import { useEventListener } from "usehooks-ts"
import { computeIsMobile } from "../utils/utils"
import { useGlobalPersistService } from "../services/globalPersistService"
import { useGlobalService } from "../services/globalService"

export const useMobileDetect = () => {
    const globalState = useGlobalService()
    const globalPersistState = useGlobalPersistService()

    useEventListener('resize', () => globalState.setIsMobile(computeIsMobile()))
}