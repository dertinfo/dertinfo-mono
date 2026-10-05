import { Component, Inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-group-view-marking-sheet-zoom',
    templateUrl: './marking-sheet-zoom.component.html',
    styleUrls: ['./marking-sheet-zoom.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class MarkingSheetZoomComponent implements OnInit {

  public markingSheetUrl;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {

    this.markingSheetUrl = this.data.markingSheetUrl;

    console.log('MarkingSheetZoomComponent - ngOnInit', this.markingSheetUrl);
  }

}
