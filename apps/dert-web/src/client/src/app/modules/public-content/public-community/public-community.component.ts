import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-public-community',
    templateUrl: './public-community.component.html',
    styleUrls: ['./public-community.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class PublicCommunityComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
