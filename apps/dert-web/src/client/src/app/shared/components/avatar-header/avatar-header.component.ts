import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as domHelper from '../../../helpers/dom.helper';

@Component({
  selector: 'app-avatar-header',
  templateUrl: './avatar-header.component.html',
  styleUrls: ['./avatar-header.component.css']
})
export class AvatarHeaderComponent implements OnInit {

  @Input() title: string;
  @Input() imageUrl: string;

  constructor() {}

  ngOnInit() { }

}
