import { computeIsMobile } from "../utils/utils"
import { useEventListener } from "usehooks-ts"
import { useGlobalService } from "../services/global/Service"

export const useMobileDetect = () => {
    const setIsMobile = useGlobalService(x => x.setIsMobile)
    
    useEventListener('resize', () => setIsMobile(computeIsMobile()))
}