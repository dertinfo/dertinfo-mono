import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-email-preview-dialog',
  templateUrl: './email-preview-dialog.component.html',
  styleUrls: ['./email-preview-dialog.component.css']
})
export class EmailPreviewDialogComponent implements OnInit {

  public previewHtml;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {

    this.previewHtml = this.data.previewHtml;
  }
}
