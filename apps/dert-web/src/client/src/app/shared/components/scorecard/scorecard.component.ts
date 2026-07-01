import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ScoreCard } from 'app/models/app/ScoreCard';

@Component({
  selector: 'app-score-card',
  templateUrl: './scorecard.component.html',
  styleUrls: ['./scorecard.component.scss']
})
export class ScoreCardComponent implements OnInit {

  @Input() scoreCard: ScoreCard;
  @Input() allowReport: boolean = true;
  @Input() linkSubmission: boolean = false;

  @Output() reported: EventEmitter<any> = new EventEmitter();
  @Output() view: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

  onReportClicked() {
    this.reported.emit(this.scoreCard);
  }

  onViewSubmissionClicked() {
    this.view.emit(this.scoreCard);
  }

}
