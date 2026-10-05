import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-content-region',
    templateUrl: './content-region.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ContentRegionComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
