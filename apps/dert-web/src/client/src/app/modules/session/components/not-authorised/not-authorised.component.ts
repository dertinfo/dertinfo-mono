import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-not-authorised',
    templateUrl: './not-authorised.component.html',
    styleUrls: ['./not-authorised.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class NotAuthorisedComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
