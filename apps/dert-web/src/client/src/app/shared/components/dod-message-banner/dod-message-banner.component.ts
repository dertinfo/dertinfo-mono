import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-dod-message-banner',
    templateUrl: './dod-message-banner.component.html',
    styleUrls: ['./dod-message-banner.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class DodMessageBannerComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
