import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScoreSheetDto, SignInSheetDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { PaperworkGeneratorConductor } from '../../services/paperwork-generator.conductor';

@Component({
  selector: 'app-signinsheets',
  templateUrl: './signinsheets.component.html',
  styleUrls: ['./signinsheets.component.css']
})
export class SignInSheetsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private _eventId: number;

  public dataLoaded: boolean = false;
  public signInSheetData: Array<SignInSheetDto> = [];

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: PaperworkGeneratorConductor,

  ) { }

  ngOnInit() {

    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      this._eventId = +params['id']; // (+) converts string 'id' to a number
      this.loadPopulatedData();
    }));

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  private loadPopulatedData() {

    const subs = this._conductor.getSignInSheetPopulatedData(this._eventId).subscribe((data) => {

      subs.unsubscribe();
      this.dataLoaded = true;
      this.signInSheetData = data;
    });
  }
}
