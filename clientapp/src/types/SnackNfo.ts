export enum SnackNfoType { 'success', 'info', 'warning', "error" }

export interface SnackNfo {
  msg: string,
  type: SnackNfoType,
  duration?: number | null | undefined
}