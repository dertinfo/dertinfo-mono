import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

@Component({
    selector: 'app-group-deleted-dialog',
    templateUrl: './group-deleted-dialog.component.html',
    styleUrls: ['./group-deleted-dialog.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class GroupDeletedDialogComponent {
  constructor() { }
}
