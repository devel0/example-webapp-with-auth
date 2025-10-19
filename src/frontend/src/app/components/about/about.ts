import { Component, isDevMode } from '@angular/core';
import { APP_NAME } from '../../constants/general';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-about',
  imports: [],
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
