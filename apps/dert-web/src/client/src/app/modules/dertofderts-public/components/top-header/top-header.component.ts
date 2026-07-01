import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-top-header',
  templateUrl: './top-header.component.html',
  styleUrls: ['./top-header.component.scss']
})
export class TopHeaderComponent implements OnInit {

  @Input() showBackToHome = true;

  constructor() { }

  ngOnInit() {
  }

}
