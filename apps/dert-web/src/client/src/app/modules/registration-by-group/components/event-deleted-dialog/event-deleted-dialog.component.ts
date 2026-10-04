import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

@Component({
    selector: 'app-event-deleted-dialog',
    templateUrl: './event-deleted-dialog.component.html',
    styleUrls: ['./event-deleted-dialog.component.css'],
    standalone: false
})
export class EventDeletedDialogComponent {
  constructor() { }
}
