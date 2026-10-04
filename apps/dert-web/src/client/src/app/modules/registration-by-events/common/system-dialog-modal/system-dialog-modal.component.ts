import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';

@Component({
  selector: 'app-system-dialog-modal',
  templateUrl: './system-dialog-modal.component.html',
  styleUrls: ['./system-dialog-modal.component.css']
})
export class SystemDialogModalComponent implements OnInit {

  constructor(
      public dialog: MatDialog
    ) { }

  ngOnInit() {
  }

  openSystemDialog(dialogueComponent) {
    const dialogRef = this.dialog.open(dialogueComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }
}
