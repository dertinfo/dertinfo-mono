import { Component, Inject, OnInit } from '@angular/core';
import { MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';

@Component({
  selector: 'app-group-view-marking-sheet-zoom',
  templateUrl: './marking-sheet-zoom.component.html',
  styleUrls: ['./marking-sheet-zoom.component.css']
})
export class MarkingSheetZoomComponent implements OnInit {

  public markingSheetUrl;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {

    this.markingSheetUrl = this.data.markingSheetUrl;

    console.log('MarkingSheetZoomComponent - ngOnInit', this.markingSheetUrl);
  }

}
