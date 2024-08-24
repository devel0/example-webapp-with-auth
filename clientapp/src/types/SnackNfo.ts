export enum SnackNfoType { 'success', 'info', 'warning', "error" }

export interface SnackNfo {
  open?: boolean,
  title?: string,
  msg: string[],
  type: SnackNfoType,
  durationMs?: number | null | undefined
}