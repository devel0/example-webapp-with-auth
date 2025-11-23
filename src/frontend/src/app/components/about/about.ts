import { Component, isDevMode } from '@angular/core';
import { environment } from '../../../environments/environment';
import { BasicModule } from '../../modules/basic/basic-module';
import { ConstantsService } from '../../services/constants-service';

@Component({
  selector: 'app-about',
  imports: [BasicModule],
  templateUrl: './about.html',
  styleUrl: './about.scss'
})
export class About {


  constructor(
    private readonly constantsService: ConstantsService,
  ) {
  }

  get appName() { return this.constantsService.APP_NAME }

  get devMode() { return isDevMode() }

  get gitCommitSha() { return environment.commit }
  get gitCommitDate() { return environment.commitDate }

}
