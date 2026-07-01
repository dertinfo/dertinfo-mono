import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScoreSheetDto } from 'app/models/dto';
import { PaperworkComponent } from 'app/modules/competition-admin/components/paperwork/paperwork.component';
import { Subscription } from 'rxjs';
import { PaperworkGeneratorConductor } from '../../services/paperwork-generator.conductor';

@Component({
  selector: 'app-scoresheets',
  templateUrl: './scoresheets.component.html',
  styleUrls: ['./scoresheets.component.css']
})
export class ScoreSheetsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private _competitionId: number;
  private _produceSpares: number;

  public dataLoaded: boolean = false;
  public scoreSheetData: Array<ScoreSheetDto> = [];

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: PaperworkGeneratorConductor,

  ) { }

  ngOnInit() {

    this._produceSpares = this._activatedRoute.snapshot.data['spares'];

    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      this._competitionId = +params['id']; // (+) converts string 'id' to a number

      if (this._produceSpares) {
        this.loadSparesData();
      } else {
        this.loadPopulatedData();
      }
    }));

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  private loadPopulatedData() {

    const subs = this._conductor.getScoresheetPopulatedData(this._competitionId).subscribe((data) => {

      subs.unsubscribe();
      this.dataLoaded = true;
      this.scoreSheetData = data;
    });
  }

  private loadSparesData() {

    const subs = this._conductor.getScoresheetSparesData(this._competitionId).subscribe((data) => {

      subs.unsubscribe();
      this.dataLoaded = true;
      this.scoreSheetData = data;
    });
  }

}
