import { Component, isDevMode } from '@angular/core';
import { APP_NAME } from '../../constants/general';
import { environment } from '../../../environments/environment';
import { BasicModule } from '../../modules/basic/basic-module';

@Component({
  selector: 'app-about',
  imports: [BasicModule],
  templateUrl: './about.html',
  styleUrl: './about.scss'
})
export class About {


  constructor() {
  }

  get appName() { return APP_NAME }

  get devMode() { return isDevMode() }

  get gitCommitSha() { return environment.commit }
  get gitCommitDate() { return environment.commitDate }

}
