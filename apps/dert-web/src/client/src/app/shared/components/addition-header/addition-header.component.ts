import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';
import * as domHelper from '../../../helpers/dom.helper';

@Component({
    selector: 'app-addition-header',
    templateUrl: './addition-header.component.html',
    styleUrls: ['./addition-header.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
