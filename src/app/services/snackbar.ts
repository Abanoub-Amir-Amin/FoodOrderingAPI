import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class SnackbarService {
  constructor(private snackBar: MatSnackBar) {}

  /***This service wraps (Angular Material's MatSnackBar) ==> a liberary --> to simplify showing notifications in your app.
   * By injecting this service, you can call snackbarService.show('Message') to display a notification without repeating positioning or duration config.
   */
  show(message: string, action = 'Close', duration = 5000): void {
    this.snackBar.open(message, action, {
      duration,
      horizontalPosition: 'right', 
      verticalPosition: 'top',      
    });
  }
}
