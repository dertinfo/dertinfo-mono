import { Component, Input, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Route } from '@angular/router';

@Component({
    selector: 'app-dod-navigation-card',
    templateUrl: './dod-navigation-card.component.html',
    styleUrls: ['./dod-navigation-card.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class DodNavigationCardComponent implements OnInit {

  @Input() routeTo: string;

  constructor() { }

  ngOnInit() {
  }
}
