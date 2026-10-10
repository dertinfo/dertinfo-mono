import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-data-policy',
    templateUrl: './data-policy.component.html',
    styleUrls: ['./data-policy.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class DataPolicyComponent {

  public onScrollToClick(anchor: HTMLElement) {
    anchor.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

}
