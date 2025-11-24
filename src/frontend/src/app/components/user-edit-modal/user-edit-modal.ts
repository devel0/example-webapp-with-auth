import { AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogContent, MatDialogRef } from '@angular/material/dialog';
import { AuthApiService, EditUserRequestDto } from '../../../api';
import { BasicModule } from "../../modules/basic/basic-module";
import { firstValueFrom, Observable } from 'rxjs';
import { MatCard, MatCardHeader, MatCardContent } from "@angular/material/card";

@Component({
  selector: 'app-user-edit-modal',
  imports: [MatDialogContent, BasicModule, MatCard, MatCardHeader, MatCardContent],
  templateUrl: './user-edit-modal.html',
  styleUrl: './user-edit-modal.scss',
})
export class UserEditModal implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy {
  allRoles: Observable<string[]> | null = null

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private readonly authApi: AuthApiService,
    public dialogRef: MatDialogRef<UserEditModal>,
    private changeDetector: ChangeDetectorRef,
  ) {
    this.loadAllRoles()
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {

  }

  ngAfterViewChecked(): void {
  }

  ngOnDestroy(): void {

  }

  private async loadAllRoles() {
    try {
      const q = await this.authApi.apiAuthListRolesGet()

      this.allRoles = q

      this.changeDetector.detectChanges()
    }
    catch (error) {
      console.error(error)
    }
  }

  toggleRole(role: string) {
    if (this.user.editRoles == null) return

    const qIdx = this.user.editRoles.indexOf(role)
    if (qIdx === -1)
      this.user.editRoles.push(role)
    else
      this.user.editRoles.splice(qIdx, 1)
  }

  async onApply() {
    try {
      const res = await firstValueFrom(this.authApi.apiAuthEditUserPost(this.user))
      this.dialogRef.close()
    }
    catch (error) {
      console.error(error)
    }
  }

  get user(): EditUserRequestDto {
    return this.data.user
  }

}
