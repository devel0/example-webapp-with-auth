import { useEventListener } from "usehooks-ts"
import { useAppSelector, useAppDispatch } from "../redux/hooks/hooks"
import { setIsMobile } from "../redux/slices/globalSlice"
import { GlobalState } from "../redux/states/GlobalState"
import { computeIsMobile } from "../utils/utils"

export const useMobileDetect = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    useEventListener('resize', () => dispatch(setIsMobile(computeIsMobile())))
}