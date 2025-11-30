import { Component, isDevMode, OnDestroy } from '@angular/core';
import { environment } from '../../../environments/environment';
import { BasicModule } from '../../modules/basic/basic-module';
import { ConstantsService } from '../../services/constants-service';
import { ExampleWebsocketService } from '../../services/websocket/example-websocket-service';
import { sizeHumanizer } from '../../utils/utils';
import { BehaviorSubject, firstValueFrom, Subscription } from 'rxjs';

@Component({
  selector: 'app-about',
  imports: [BasicModule],
  templateUrl: './about.html',
  styleUrl: './about.scss'
})
export class About implements OnDestroy {

  private srvMem = new BehaviorSubject<string>("")
  srvMem$ = this.srvMem.asObservable()
  srvMemSub: Subscription

  constructor(
    private readonly constantsService: ConstantsService,
    readonly ws: ExampleWebsocketService
  ) {
    this.srvMemSub = ws.srvMemUsed$.subscribe(x => {
      if (x != null) this.srvMem.next(sizeHumanizer(x))
    })
  }

  ngOnDestroy(): void {
    if (this.srvMemSub != null) this.srvMemSub.unsubscribe()
  }

  get appName() { return this.constantsService.APP_NAME }

  get devMode() { return isDevMode() }

  get gitCommitSha() { return environment.commit }
  get gitCommitDate() { return environment.commitDate }

}
