import { BaseVariant } from "notistack";

export enum SnackNfoType { 'success', 'info', 'warning', "error" }

export interface SnackNfo {  
  title?: string,
  msg: string[],
  type: BaseVariant,
  durationMs?: number | null | undefined
}