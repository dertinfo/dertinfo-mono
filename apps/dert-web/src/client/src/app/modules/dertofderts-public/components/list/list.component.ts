import { Component, Input, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';

@Component({
    selector: 'app-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ListComponent implements OnInit {

  @Input() submissions: DodSubmissionDto[] = [];

  constructor() { }

  ngOnInit() {
  }

}
