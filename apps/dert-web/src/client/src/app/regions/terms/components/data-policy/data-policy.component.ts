import { Component } from '@angular/core';

@Component({
    selector: 'app-data-policy',
    templateUrl: './data-policy.component.html',
    styleUrls: ['./data-policy.component.scss'],
    standalone: false
})
export class DataPolicyComponent {

  public onScrollToClick(anchor: HTMLElement) {
    anchor.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

}
