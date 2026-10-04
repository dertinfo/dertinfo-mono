import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm',
  template: `<h1 matDialogTitle>{{ data.title }}</h1>
    <div mat-dialog-content>{{ data.message }}</div>
    <div mat-dialog-actions>
    <button
    type="button"
    color="secondary"
    mat-raised-button
    (click)="dialogRef.close(false)">Cancel</button>
    &nbsp;
    <span fxFlex></span>
    <button
    type="button"
    mat-raised-button
    color="primary"
    (click)="dialogRef.close(true)">OK</button>
    </div>`,
})
export class AppComfirmComponent {
  constructor(
    public dialogRef: MatDialogRef<AppComfirmComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }
}
