import { Injectable } from '@angular/core';
import { LocalStorageData } from '../types/local-storage-data';
import { LOCAL_STORAGE_KEY_DATA } from '../constants/general';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  private _data: LocalStorageData | null = null

  constructor() {

  }

  /** retrieve local storage data from {@link LOCAL_STORAGE_KEY_DATA} key 
   * or initialize an new empty {@link LocalStorageData} */
  get data(): LocalStorageData {
    if (this._data == null) {
      const q = localStorage.getItem(LOCAL_STORAGE_KEY_DATA)

      if (q == null) {
        this._data = new LocalStorageData()
        this.save()
      }
      else {
        this._data = JSON.parse(q) as LocalStorageData
        if (this._data.colorScheme == null) this._data.colorScheme = 'dark'
      }
    }

    return this._data
  }

  /** save current {@link data} to the local storage under {@link LOCAL_STORAGE_KEY_DATA} key */
  save() {
    localStorage.setItem(LOCAL_STORAGE_KEY_DATA, JSON.stringify(this._data))
  }

}
