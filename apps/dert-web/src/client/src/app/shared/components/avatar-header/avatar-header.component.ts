import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';
import * as domHelper from '../../../helpers/dom.helper';

@Component({
    selector: 'app-avatar-header',
    templateUrl: './avatar-header.component.html',
    styleUrls: ['./avatar-header.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AvatarHeaderComponent implements OnInit {

  @Input() title: string;
  @Input() imageUrl: string;

  constructor() {}

  ngOnInit() { }

}
