import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.css']
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
