import { Component, Input, OnInit } from '@angular/core';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  @Input() submissions: DodSubmissionDto[] = [];

  constructor() { }

  ngOnInit() {
  }

}
