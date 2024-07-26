import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as domHelper from '../../../helpers/dom.helper';

@Component({
  selector: 'app-addition-header',
  templateUrl: './addition-header.component.html',
  styleUrls: ['./addition-header.component.css']
})
export class AdditionHeaderComponent implements OnInit {

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
