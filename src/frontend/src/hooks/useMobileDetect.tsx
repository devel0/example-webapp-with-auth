import { useEventListener } from "usehooks-ts"
import { computeIsMobile } from "../utils/utils"
import { useGlobalPersistService } from "../services/globalPersistService"
import { useGlobalService } from "../services/globalService"

export const useMobileDetect = () => {
    const setIsMobile = useGlobalService(x => x.setIsMobile)
    

    useEventListener('resize', () => setIsMobile(computeIsMobile()))
}