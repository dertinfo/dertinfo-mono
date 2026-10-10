import { Component, EventEmitter, Inject, Input, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-email-preview-dialog',
    templateUrl: './email-preview-dialog.component.html',
    styleUrls: ['./email-preview-dialog.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class EmailPreviewDialogComponent implements OnInit {

  public previewHtml;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {

    this.previewHtml = this.data.previewHtml;
  }
}
