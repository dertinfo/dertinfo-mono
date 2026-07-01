import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Route } from '@angular/router';

@Component({
  selector: 'app-dod-navigation-card',
  templateUrl: './dod-navigation-card.component.html',
  styleUrls: ['./dod-navigation-card.component.scss']
})
export class DodNavigationCardComponent implements OnInit {

  @Input() routeTo: string;

  constructor() { }

  ngOnInit() {
  }
}
