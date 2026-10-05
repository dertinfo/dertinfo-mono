import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-public-notations',
    templateUrl: './public-notations.component.html',
    styleUrls: ['./public-notations.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class PublicNotationsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
