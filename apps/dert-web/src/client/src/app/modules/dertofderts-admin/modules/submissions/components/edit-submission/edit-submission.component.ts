import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { GroupDto } from 'app/models/dto';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { DodSubmissionUpdateSubmissionDto } from 'app/models/dto/DodSubmissionUpdateSubmissionDto';
import { Observable, SubscriptionLike } from 'rxjs';
import { Conductor } from '../../../../services/dertofderts-admin.conductor';
import { Tracker } from '../../../../services/dertofderts-admin.tracker';

@Component({
  selector: 'app-edit-submission',
  templateUrl: './edit-submission.component.html',
  styleUrls: ['./edit-submission.component.scss']
})
export class EditSubmissionComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  public isReady = false;
  public submission: DodSubmissionDto;
  public form: UntypedFormGroup;

  public groups$: Observable<GroupDto[]>;

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: Conductor,
    private _tracker: Tracker
  ) { }

  ngOnInit() {

    this._conductor.initGroups();

    this.groups$ = this._tracker.groups$;

    const subsA = this._activatedRoute.params.subscribe(params => {
      this._conductor.initFocussedSubmission(+params['id']);
    });

    const subsB = this._tracker.focussedSubmission$.subscribe((submission) => {
      if (submission) {
        this.submission = submission;

        let entryCategory = 'Premier';
        if (this.submission.isPremier) { entryCategory = 'Premier'; }
        if (this.submission.isChampionship) { entryCategory = 'Championship'; }
        if (this.submission.isOpen) { entryCategory = 'Open'; }

        this.form = new UntypedFormGroup({
          groupId: new UntypedFormControl(this.submission.groupId, [Validators.required]),
          embedLink: new UntypedFormControl(this.submission.embedLink, [Validators.required]),
          embedOrigin: new UntypedFormControl(this.submission.embedOrigin, []),
          dertYearFrom: new UntypedFormControl(this.submission.dertYearFrom, [Validators.required]),
          dertVenueFrom: new UntypedFormControl(this.submission.dertVenueFrom, []),
          entryCategory: new UntypedFormControl(entryCategory, []),
        });

        this.isReady = true;
      }
    });
    this._subscriptions.push(subsA, subsB);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onSubmit() {

    if (this.form.valid) {

      const submissionUpdate: DodSubmissionUpdateSubmissionDto = {
        submissionId: this.submission.id,
        groupId: this.form.value.groupId,
        embedLink: this.form.value.embedLink,
        embedOrigin: this.form.value.embedOrigin,
        dertYearFrom: this.form.value.dertYearFrom,
        dertVenueFrom: this.form.value.dertVenueFrom,
        isPremier: this.form.value.entryCategory === 'Premier',
        isChampionship: this.form.value.entryCategory === 'Championship',
        isOpen: this.form.value.entryCategory === 'Open',
      };

      this._conductor.updateSubmission(submissionUpdate);
    }
  }
}
