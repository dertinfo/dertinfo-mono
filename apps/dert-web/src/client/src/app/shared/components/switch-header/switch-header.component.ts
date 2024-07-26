import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as domHelper from '../../../helpers/dom.helper';

@Component({
  selector: 'app-switch-header',
  templateUrl: './switch-header.component.html',
  styleUrls: ['./switch-header.component.css']
})
export class SwitchHeaderComponent implements OnInit {

  @Input() title;
  @Input() icon;
  @Input() disabled: boolean;
  @Input() hideButton: boolean = false;
  @Output() addClick = new EventEmitter<string>();

  constructor() {}

  ngOnInit() { }

  onAddClicked() {
    this.addClick.emit();
  }

}
