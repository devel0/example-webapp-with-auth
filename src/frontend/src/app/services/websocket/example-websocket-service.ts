import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ConstantsService } from '../constants-service';
import { BaseWSProtocol, ExampleWSProto1, ExampleWSProto2, ExampleWSProtocol, ExampleWSProtoServerMem } from '../../../api';

@Injectable({
  providedIn: 'root',
})
export class ExampleWebsocketService {

  private websocket!: WebSocket

  private connected = new BehaviorSubject<boolean>(false)
  connected$ = this.connected.asObservable()

  private srvMemUsed = new BehaviorSubject<number | null>(null)
  srvMemUsed$ = this.srvMemUsed.asObservable()

  private pingMsg: string = 'ping'
  private pongMsg: string = 'pong'

  constructor(
    private readonly constantsService: ConstantsService
  ) {
    this.connect()

    setInterval(() => this.ping(), constantsService.WEBSOCKET_PING_MS)
  }

  sendMyProto1(msg: string) {
    this.sendMessage({
      protocolType: 'MyProto1',
      someMsg: msg
    } as ExampleWSProto1)
  }

  sendMyProto2(value: number) {
    this.sendMessage({
      protocolType: 'MyProto2',
      someLongValue: value
    } as ExampleWSProto2)
  }

  private connect() {
    this.websocket = new WebSocket(this.constantsService.ALIVE_WEBSOCKET_URL)

    this.websocket.onopen = (event) => {
      this.connected.next(true)
    }

    this.websocket.onmessage = (event) => {
      const proto: ExampleWSProtocol = JSON.parse(event.data)

      if (proto.baseProtocolType === 'Pong') {
        const pong: BaseWSProtocol = JSON.parse(event.data)
        this.pongMsg = pong.baseProtocolMsg ?? ''
        this.connected.next(this.pingMsg === this.pongMsg)
      }
      else switch (proto.protocolType) {
        case 'SrvMem':
          {
            const srvMem: ExampleWSProtoServerMem = JSON.parse(event.data)
            this.srvMemUsed.next(srvMem.memoryUsed ?? null)
          }
          break;
      }

    }

    this.websocket.onerror = (error) => {
      console.error(`ws err ${error}`)
    }

    this.websocket.onclose = () => {
      this.connected.next(false)
    }

  }

  private sendMessage(msg: any) {
    if (this.websocket.readyState === WebSocket.OPEN) {
      this.websocket.send(JSON.stringify(msg))
    }
    else
      this.connect()
  }

  private ping() {
    this.pingMsg = new Date().toISOString()

    this.sendMessage({
      baseProtocolType: 'Ping',
      baseProtocolMsg: this.pingMsg
    } as ExampleWSProtocol)

    setTimeout(() => {
      this.connected.next(this.pingMsg === this.pongMsg)
    }, this.constantsService.WEBSOCKET_PONG_MS)
  }

}
