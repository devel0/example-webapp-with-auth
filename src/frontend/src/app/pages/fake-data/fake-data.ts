import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject, firstValueFrom, map, pairwise, Subscription } from 'rxjs';
import { buildGenericDynFilter, ColumnFilterNfo } from '../../components/data-grid/utils/DataGridDynFilter';
import { DataGrid } from "../../components/data-grid/data-grid";
import { ComputeDynFilerNfo, DataGridColumn, DataGridColumnState, FieldKind, NeedCountNfo, NeedLoadNfo } from '../../components/data-grid/types/data-grid-types';
import { FakeData as FakeDataType } from '../../../api/model/fakeData'
import { FakeDataApiService, GenericSort, SortModelItem } from '../../../api';
import { from } from 'linq-to-typescript';
import { FullHeightDiv } from "../../components/full-height-div/full-height-div";
import { HttpErrorResponse } from '@angular/common/http';
import { MainLayout } from "../../components/main-layout/main-layout";
import { pathBuilder } from '../../utils/utils';
import { DataGridPager } from "../../components/data-grid/data-grid-pager/data-grid-pager";
import { MatIcon } from "@angular/material/icon";

type TDATA = FakeDataType
const fnTDATA = pathBuilder<TDATA>()

@Component({
  selector: 'app-fake-data',
  imports: [MainLayout, DataGrid, FullHeightDiv, DataGridPager, MatIcon],
  templateUrl: './fake-data.html',
  styleUrl: './fake-data.scss'
})
export class FakeData implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('dgRef') dgRef!: DataGrid<TDATA>

  dataGrid!: DataGrid<TDATA>

  constructor(
    private readonly fakerApi: FakeDataApiService
  ) {
  }

  ngOnInit() {
    // prevent "Expression has changed after it was checked."
    Promise.resolve().then(() => {
      this.dataGrid = this.dgRef
    })
  }

  ngOnDestroy() {
  }

  ngAfterViewInit() {    
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
          } as ColumnFilterNfo<FakeData>
        })
      })
    }

    return dynFilterTmp
  }

  async countData(nfo: NeedCountNfo) {
    let res: number | null = null

    try {
      res = await firstValueFrom(this.fakerApi.apiFakeDataCountFakeDatasPost({
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

      res = await firstValueFrom(this.fakerApi.apiFakeDataGetFakeDatasPost({
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
      header: 'Id',
      fieldName: fnTDATA('id'),
      flexWidth: 2,
      // width: 350,
      getData: x => x.id,
      dbFunFilterPreprocess: 'DbFun.GuidString'
    },
    {
      header: 'FirstName',
      fieldName: fnTDATA('firstName'),
      getData: x => x.firstName,
      // initialSortDirection: 'Ascending'
    },
    {
      header: 'LastName',
      fieldName: fnTDATA('lastName'),
      getData: x => x.lastName
    },
    {
      header: 'Email',
      fieldName: fnTDATA('email'),
      getData: x => x.email
    },
    {
      header: 'Phone',
      fieldName: fnTDATA('phone'),
      getData: x => x.phone
    },
    {
      header: 'Group',
      fieldName: fnTDATA('groupNumber'),
      fieldKind: FieldKind.numeric,
      getData: x => x.groupNumber,
      dataDivClass: 'number-div'
    },
    {
      header: 'Birth date',
      fieldName: fnTDATA('dateOfBirth'),
      fieldKind: FieldKind.dateTimeOffset,
      width: 240,
      getData: x => new Date(x.dateOfBirth ?? '').toISOString()
    },
  ]

}

