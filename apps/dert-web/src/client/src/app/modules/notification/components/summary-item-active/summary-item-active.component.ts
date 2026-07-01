import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-notification-summary-item-active',
  templateUrl: './summary-item-active.component.html',
  styleUrls: ['./summary-item-active.component.scss'],
  encapsulation: ViewEncapsulation.None

})
export class SummaryItemActiveComponent implements OnInit {

  @Input() title: string;
  @Input() summary: string;
  @Input() severity: string;
  @Input() time: string;
  @Input() icon: string;
  @Input() isNew: boolean;
  @Input() canOpen: boolean;
  @Input() canDismiss: boolean;

  @Output() open: EventEmitter<any> = new EventEmitter();
  @Output() dismiss: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

  public onDismissClick() {
    this.dismiss.emit();
  }

  public onOpenClick() {
    this.open.emit();
  }

}
