import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as domHelper from '../../../helpers/dom.helper';
import { NavigationService } from '../../../core/services/navigation.service';

@Component({
  selector: 'app-scorebox',
  templateUrl: './scorebox.component.html',
  styleUrls: ['./scorebox.component.css']
})
export class ScoreBoxComponent {

  @Input() title: string;
  @Input() value: number;
  @Input() readonly: boolean = true;
  @Output() focusOut = new EventEmitter<number>();

  constructor() { }

  onFocusOut() {
    this.focusOut.emit(this.value);
  }
}
