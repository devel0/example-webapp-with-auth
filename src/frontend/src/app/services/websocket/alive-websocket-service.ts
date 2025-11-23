import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ConstantsService } from '../constants-service';
import { AliveWSProtocol, WSPing, WSPong } from '../../../api';

@Injectable({
  providedIn: 'root',
})
export class AliveWebsocketService {

  private websocket!: WebSocket

  private connected = new BehaviorSubject<boolean>(false)
  connected$ = this.connected.asObservable()

  private pingMsg: string = 'ping'
  private pongMsg: string = 'pong'

  constructor(
    private readonly constantsService: ConstantsService
  ) {
    this.connect()

    setInterval(() => this.ping(), constantsService.WEBSOCKET_PING_MS)
  }

  private connect() {
    this.websocket = new WebSocket(this.constantsService.ALIVE_WEBSOCKET_URL)

    this.websocket.onopen = (event) => {
      this.connected.next(true)
    }

    this.websocket.onmessage = (event) => {
      const proto: AliveWSProtocol = JSON.parse(event.data)
      if (proto.messageType === 'Pong') {
        const pong: WSPong = JSON.parse(event.data)
        this.pongMsg = pong.msg ?? ''
        this.connected.next(this.pingMsg === this.pongMsg)
      }
    }

    this.websocket.onerror = (error) => {
      console.error(`ws err ${error}`)
    }

    this.websocket.onclose = () => {
      this.connected.next(false)
    }

  }

  sendMessage(msg: any) {
    if (this.websocket.readyState === WebSocket.OPEN) {
      this.websocket.send(JSON.stringify(msg))
    }
    else
      this.connect()
  }

  private ping() {
    this.pingMsg = new Date().toISOString()

    this.sendMessage({
      messageType: 'Ping',
      msg: this.pingMsg
    } as WSPing)

    setTimeout(() => {
      this.connected.next(this.pingMsg === this.pongMsg)
    }, this.constantsService.WEBSOCKET_PONG_MS)
  }

}
