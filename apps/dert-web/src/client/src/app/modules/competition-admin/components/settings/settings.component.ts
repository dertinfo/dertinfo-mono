import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CompetitionSettingsUpdateSubmissionDto, CompetitionSummaryDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

@Component({
  selector: 'app-competition-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public settingsForm: FormGroup;

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,

  ) { }

  ngOnInit() {

    // Start to listen for changes on the tracker
    this._subscriptions.push(this._tracker.settings$.subscribe((settings) => {
      this.prepareForms();
    }));

    // apply the settings through the conductor from the resolver
    this._conductor.applySettings(this._activatedRoute.snapshot.data.settings);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onSettingsUpdateSubmit() {

    const update: CompetitionSettingsUpdateSubmissionDto = {
      noOfJudgesPerVenue: this.settingsForm.value.noOfJudgesPerVenue,
      resultsPublished: this.settingsForm.value.resultsPublished,
      resultsCollated: this.settingsForm.value.resultsCollated,
      inTestingMode: this.settingsForm.value.inTestingMode,
      allowAdHocDanceAddition: this.settingsForm.value.allowAdHocDanceAddition
    };

    this._conductor.updateSettings(update);
  }

  private prepareForms() {

    if (this._tracker.hasLoadedSettings()) {
      this.settingsForm = new FormGroup({
        noOfJudgesPerVenue: new FormControl(this._tracker.settings.noOfJudgesPerVenue, [Validators.required]),
        resultsPublished: new FormControl(this._tracker.settings.resultsPublished, []),
        resultsCollated: new FormControl(this._tracker.settings.resultsCollated, []),
        inTestingMode: new FormControl(this._tracker.settings.inTestingMode, []),
        allowAdHocDanceAddition: new FormControl(this._tracker.settings.allowAdHocDanceAddition, []),
      });

    } else {
      this.resetForms();
    }
  }

  private resetForms() {
    this.settingsForm = new FormGroup({
      noOfJudgesPerVenue: new FormControl(0, [Validators.required]),
      resultsPublished: new FormControl(false, []),
      resultsCollated: new FormControl(true, []),
      inTestingMode: new FormControl(false, []),
      allowAdHocDanceAddition: new FormControl(false, []),
    });
  }

}
