import { useEffect, useRef, useState } from 'react';
import styles from './DataGridColumnWidthHandler.module.scss';

export function DataGridColumnWidthHandler<T>(props: {
    onColumnWidthChange?: (deltaX: number) => void
}) {
    const [startPosX, setStartPosX] = useState<number | null>(null)
    const [posX, setPosX] = useState<number | null>(null)
    const divRef = useRef<HTMLDivElement>(null)

    useEffect(() => {
        if (divRef.current != null && startPosX != null && posX != null) {
            const deltaX = startPosX - posX

            // console.log(`delta x = ${deltaX}`)

            // props.onColumnWidthChange?.(-deltaX)

            divRef.current.style['right'] = `${-5 + deltaX}px`
        }
    }, [
        startPosX, posX, divRef
    ])

    return <div
        ref={divRef}
        onPointerDown={e => {
            e.currentTarget.setPointerCapture(e.pointerId)

            const right = parseFloat(
                e.currentTarget.computedStyleMap().get('right')?.toString().replace(/px$/, '') ?? '0')

            setStartPosX(5 + right + e.clientX)
        }}
        onPointerMove={e => setPosX(e.clientX)}
        onPointerUp={e => {
            e.currentTarget.releasePointerCapture(e.pointerId)
            setStartPosX(null)

            if (divRef.current != null) {

                const right = parseFloat(
                    e.currentTarget.computedStyleMap().get('right')?.toString().replace(/px$/, '') ?? '0')

                console.log(`setting right => ${-right}`)

                props.onColumnWidthChange?.(-right - 5)

                divRef.current.style['right'] = `-5px`
            }
        }}
        className={styles['handler']}
    >

    </div>
}