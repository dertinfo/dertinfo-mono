import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'dert2026-intro-banner',
    templateUrl: './dert2026-intro-banner.component.html',
    styleUrls: ['./dert2026-intro-banner.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class Dert2026IntroBannerComponent implements OnInit {

  public registrationLink = '/dashboard';

  constructor() { }

  ngOnInit() {
  }

}
