import { Injectable } from '@angular/core';

export interface SnackNfo {
  matIcon?: string,
  title: string,
  message?: string,
  className: string | null
}

@Injectable({
  providedIn: 'root'
})
export class SnackService {
  snacks: SnackNfo[] = []

  show(props: { title: string, message?: string, duration?: number, className?: string, matIcon?: string }) {
    const { title, message, duration, className, matIcon } = props

    this.snacks.push({
      title,
      message,
      className: className ?? null,
      matIcon
    })

    setTimeout(() => {
      this.snacks.shift()
    }, duration ?? 3000)
  }

  showError(title: string, message?: string) {
    this.show({ title, message, duration: 3000, className: 'snack-error', matIcon: 'dangerous' })
  }

  showWarning(title: string, message?: string) {
    this.show({ title, message, duration: 3000, className: 'snack-warning', matIcon: 'warning' })
  }

  showInfo(title: string, message?: string) {
    this.show({ title, message, duration: 2000, className: 'snack-info', matIcon: 'info' })
  }

  showSuccess(title: string, message?: string) {
    this.show({ title, message, duration: 2000, className: 'snack-success', matIcon: 'check_circle' })
  }

}
