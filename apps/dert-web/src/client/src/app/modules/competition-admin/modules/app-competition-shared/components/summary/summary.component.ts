import { Component, Input } from '@angular/core';

import { Router } from '@angular/router';
import { CompetitionSummaryDto } from 'app/models/dto';

@Component({
  selector: 'app-competition-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.css']
})
export class SummaryComponent  {

  @Input() summary: CompetitionSummaryDto;
  @Input() hideLinkToDetails: boolean = false;

  constructor(private _router: Router) { }

  public onConfigureCompetitionClick() {
    this._router.navigate(['competition', this.summary.competitionName, this.summary.competitionId]);
  }

  public onSparesClick() {

    const window = this.getNativeWindow();

    window.open(`./paperwork/scoresheets/${this.summary.competitionId}/spares`, '_blank');

  }

  public onPopulatedClick() {

    const window = this.getNativeWindow();

    window.open(`./paperwork/scoresheets/${this.summary.competitionId}/populated`, '_blank');

  }

  private getNativeWindow(): Window {
    return window;
  }
}
