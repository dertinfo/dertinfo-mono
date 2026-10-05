import { Component, EventEmitter, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-notification-summary-item-deleted',
    templateUrl: './summary-item-deleted.component.html',
    styleUrls: ['./summary-item-deleted.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SummaryItemDeletedComponent implements OnInit {

  @Output() dismiss: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

  public onDismissClick() {
    this.dismiss.emit();
  }

}
