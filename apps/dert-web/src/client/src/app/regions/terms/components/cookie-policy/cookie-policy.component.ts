import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-cookie-policy',
    templateUrl: './cookie-policy.component.html',
    styleUrls: ['./cookie-policy.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class CookiePolicyComponent implements OnInit {

  constructor() { }

  ngOnInit() { }

  public onScrollToClick(anchor: HTMLElement) {
    anchor.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

}
