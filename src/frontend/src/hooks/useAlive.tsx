import { useRef, useState } from "react"
import { API_URL, WS_PING_MS, WS_PONG_EXPECTED_MS, WSS_URL } from "../constants/general"
import useWebSocket from "react-use-websocket"
import { useGlobalService } from "../services/global/Service"
import { AliveWSProtocol, WSPing, WSPong } from "../../api"
import { useInterval } from "usehooks-ts"

export const useAlive = () => {
    const pingMsg = useRef('ping')
    const pongMsg = useRef('pong')
    const setWsConnected = useGlobalService(x => x.setWsConnected)

    useInterval(() => {
        pingMsg.current = `${new Date().valueOf()}`

        ws.sendMessage(JSON.stringify({
            messageType: 'Ping',
            msg: pingMsg.current
        } as WSPing))

        // console.log(`sent ping ${pingMsg.current}`)

        setTimeout(() => {
            // console.log(`check ping ${pingMsg.current} equals ${pongMsg.current}`)
            setWsConnected(pingMsg.current === pongMsg.current
            )
        }, WS_PONG_EXPECTED_MS)
    },
        WS_PING_MS
    )

    const ws = useWebSocket(WSS_URL(), {
        share: true,
        retryOnError: true,
        shouldReconnect: (x) => true,

        onOpen: (e) => {
            setWsConnected(true)
        },

        onMessage: (e) => {
            // console.log(`ws mex ${e.data}`)

            const proto: AliveWSProtocol = JSON.parse(e.data)
            if (proto.messageType === 'Pong') {
                const pong: WSPong = JSON.parse(e.data)

                pongMsg.current = pong.msg ?? ''

                // console.log(`RX pong ${pongMsg.current}`)

                setWsConnected(pingMsg.current === pongMsg.current)
            }
        },

        onError: (e) => {
            console.error("ws err")
        },

        onClose: (e) => {
            console.log("ws closed")
            setWsConnected(false)
        },

    })
}