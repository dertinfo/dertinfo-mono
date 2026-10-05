import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-spinner',
    templateUrl: './spinner.component.html',
    styleUrls: ['./spinner.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SpinnerComponent {

  @Input() title: string;
  @Input() value: number;
  @Input() readonly: boolean = true;
  @Output() focusOut = new EventEmitter<number>();

  constructor() { }

  onFocusOut() {
    this.focusOut.emit(this.value);
  }
}
