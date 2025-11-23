import { AuthApiService, EditUserRequestDto, GenericSort, SortModelItem, UserListItemResponseDto, UserPermission } from '../../../api';
import { buildGenericDynFilter, ColumnFilterNfo } from '../../components/data-grid/utils/DataGridDynFilter';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ComputeDynFilerNfo, DataGridColumn, NeedCountNfo, NeedLoadNfo } from '../../components/data-grid/types/data-grid-types';
import { DataGrid } from "../../components/data-grid/data-grid";
import { FullHeightDiv } from "../../components/full-height-div/full-height-div";
import { HttpErrorResponse } from '@angular/common/http';
import { MainLayout } from "../../components/main-layout/main-layout";
import { pathBuilder } from '../../utils/utils';
import { from } from 'linq-to-typescript';
import { firstValueFrom, Subscription } from 'rxjs';
import { MatIcon } from "@angular/material/icon";
import { DataGridPager } from "../../components/data-grid/data-grid-pager/data-grid-pager";
import { MatDialog } from '@angular/material/dialog';
import { UserEditModal } from '../../components/user-edit-modal/user-edit-modal'

type TDATA = UserListItemResponseDto
const fnTDATA = pathBuilder<TDATA>()

@Component({
  selector: 'app-users-manager',
  imports: [MainLayout, FullHeightDiv, DataGrid, MatIcon, DataGridPager],
  templateUrl: './users-manager.html',
  styleUrl: './users-manager.scss',
})
export class UsersManager implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('dgRef') dgRef!: DataGrid<TDATA>

  dataGrid!: DataGrid<TDATA>

  private dialogSub: Subscription | null = null

  constructor(
    public readonly dialog: MatDialog,
    private readonly authApiService: AuthApiService
  ) {
  }

  ngOnInit() {
    // prevent "Expression has changed after it was checked."
    Promise.resolve().then(() => {
      this.dataGrid = this.dgRef
    })
  }

  ngOnDestroy() {
    if (this.dialogSub != null) this.dialogSub.unsubscribe()
  }

  ngAfterViewInit() {
  }

  addUser() {

  }

  onRowDoubleClicked(row: TDATA) {
    const edit: EditUserRequestDto = {
      existingUsername: row.userName,
      editEmail: row.email,
      editRoles: [...row.roles],
      editDisabled: row.disabled
    }

    const dialogRef = this.dialog.open(UserEditModal,
      {
        data: {
          user: edit
        }
      })

    this.dialogSub = dialogRef.afterClosed().subscribe(x => {
      this.dataGrid.reload()
    })
  }

  computeDynFilter(nfo: ComputeDynFilerNfo) {
    let dynFilterTmp = ''

    const qFilter = from(nfo.columnsState)
      .select((colState, colIdx) => { return { colIdx, col: this.columns[colIdx], colState, } })
      .where(r => r.colState.filter != null)
      .toArray()
    if (qFilter.length > 0) {
      dynFilterTmp = buildGenericDynFilter({
        columnFilters: qFilter.map(x => {
          return {
            key: x.col.fieldName,
            filterNfo: x.colState.filter,
            filterCaseSensitive: x.col.filterCaseSensitive,
            kind: x.col.fieldKind,
            preProcessField: x.col.dbFunFilterPreprocess
          } as ColumnFilterNfo<TDATA>
        })
      })
    }

    return dynFilterTmp
  }

  getRowId(row: TDATA) { return row.userName ?? '' }

  async countData(nfo: NeedCountNfo) {
    let res: number | null = null

    try {
      res = await firstValueFrom(this.authApiService.apiAuthCountUsersPost({
        dynFilter: nfo.dynFilter
      }))
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.error(error)
      }
    }

    return res
  }

  async loadData(nfo: NeedLoadNfo) {
    let res: TDATA[] | null = null

    try {
      let sort: GenericSort | undefined = undefined

      const qSort = from(nfo.columnsState)
        .select((colState, colIdx) => { return { colIdx, col: this.columns[colIdx], colState, } })
        .where(r => r.col.fieldName != null && r.colState.sortDirection != null)
        .orderBy(w => w.colState.sortTimestamp)
        .toArray()
      if (qSort.length > 0) {
        sort = {
          columns: from(qSort).select(w => {
            const res: SortModelItem = {
              columnName: w.col.fieldName,
              direction: w.colState.sortDirection ?? undefined,
            }
            return res
          }).toArray()
        }
      }

      const dynFilter = nfo.precomputedDynFilter === undefined ?
        this.computeDynFilter({ columnsState: nfo.columnsState }) :
        nfo.precomputedDynFilter

      res = await firstValueFrom(this.authApiService.apiAuthGetUsersPost({
        count: nfo.pageSize,
        offset: nfo.page * nfo.pageSize,
        sort,
        dynFilter
      }))
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.error(error)
      }
    }

    return res
  }

  columns: DataGridColumn<TDATA>[] = [
    {
      header: 'Username',
      fieldName: fnTDATA('userName'),
      getData: x => x.userName,
    },
    {
      header: 'Email',
      fieldName: fnTDATA('email'),
      getData: x => x.email
    },
    {
      header: 'Roles',
      fieldName: fnTDATA('roles'),
      getData: x => x.roles
    },
    {
      header: 'Disabled',
      fieldName: fnTDATA('disabled'),
      getData: x => x.disabled
    },
  ]

}
