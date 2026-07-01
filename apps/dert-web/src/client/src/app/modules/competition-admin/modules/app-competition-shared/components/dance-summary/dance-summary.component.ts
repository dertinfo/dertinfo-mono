import { Component, Input } from '@angular/core';
import { CompetitionDanceSummaryDto } from 'app/models/dto/CompetitionDanceSummaryDto';

@Component({
  selector: 'app-competition-dancesummary',
  templateUrl: './dance-summary.component.html',
  styleUrls: ['./dance-summary.component.css']
})
export class DanceSummaryComponent {

  @Input() summary: CompetitionDanceSummaryDto;

  constructor() { }
}
