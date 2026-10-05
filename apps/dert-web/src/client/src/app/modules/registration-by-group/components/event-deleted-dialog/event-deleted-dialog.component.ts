import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

@Component({
    selector: 'app-event-deleted-dialog',
    templateUrl: './event-deleted-dialog.component.html',
    styleUrls: ['./event-deleted-dialog.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class EventDeletedDialogComponent {
  constructor() { }
}
