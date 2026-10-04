import { Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';

import { AppComfirmComponent } from './app-confirm.component';

@Injectable()
export class AppConfirmService {

  constructor(private dialog: MatDialog) { }

  public confirm(title: string, message: string, dialogPanelClass: string = null): Observable<boolean> {
    let dialogRef: MatDialogRef<AppComfirmComponent>;
    dialogRef = this.dialog.open(AppComfirmComponent, {
      width: '380px',
      disableClose: true,
      data: {title, message},
      panelClass: dialogPanelClass
    });
    return dialogRef.afterClosed();
  }
}
