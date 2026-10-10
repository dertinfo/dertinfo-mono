import { Component, Input, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-website-terms',
    templateUrl: './website-terms.component.html',
    styleUrls: ['./website-terms.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class WebsiteTermsComponent {

  @Input() hideHeader = false;

  public onScrollToClick(anchor: HTMLElement) {
    anchor.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

}
