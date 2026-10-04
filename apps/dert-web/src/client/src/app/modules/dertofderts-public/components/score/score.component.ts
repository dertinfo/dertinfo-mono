import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { RangeValidatorDirective } from 'app/shared/directives/range-validator.directive';
import { ClientSettingsService } from 'app/core/services/clientsettings.service';
import { Subscription } from 'rxjs';
import { Conductor } from '../../services/dertofderts-public.conductor';
import { SessionService } from '../../services/dertofderts-public.session';
import { Tracker } from '../../services/dertofderts-public.tracker';
import { ScoreDialogComponent } from '../scoredialog/scoredialog.component';
import { ScoreSubmittedDialogComponent } from '../scoresubmitteddialog/scoresubmitteddialog.component';

@Component({
  selector: 'app-score',
  templateUrl: './score.component.html',
  styleUrls: ['./score.component.scss']
})
export class ScoreComponent implements OnInit, OnDestroy {

  private _subscriptions: Array<Subscription> = new Array<Subscription>();
  private _submitDialogRef: MatDialogRef<ScoreDialogComponent>;
  private _submittedDialogRef: MatDialogRef<ScoreSubmittedDialogComponent>;

  private scoreCategories = [
    {
      scoreText: 'Music',
      scoreFieldName: 'musicScore',
      commentText: 'Music Comments',
      commentFieldName: 'musicComment',
      maxMarks: 15,
    },
    {
      scoreText: 'Stepping',
      scoreFieldName: 'steppingScore',
      commentText: 'Stepping Comments',
      commentFieldName: 'steppingComment',
      maxMarks: 15,
    },
    {
      scoreText: 'Sword Handling',
      scoreFieldName: 'swordHandlingScore',
      commentText: 'Sword Handling Comments',
      commentFieldName: 'swordHandlingComment',
      maxMarks: 15,
    },
    {
      scoreText: 'Dance Technique',
      scoreFieldName: 'danceTechniqueScore',
      commentText: 'Dance Technique Comments',
      commentFieldName: 'danceTechniqueComment',
      maxMarks: 15,
    },
    {
      scoreText: 'Buzz',
      scoreFieldName: 'buzzScore',
      commentText: 'Buzz Comments',
      commentFieldName: 'buzzComment',
      maxMarks: 15,
    },
    {
      scoreText: 'Presentation',
      scoreFieldName: 'presentationScore',
      commentText: 'Presentation Comments',
      commentFieldName: 'presentationComment',
      maxMarks: 15,
    },
    {
      scoreText: 'Characters Score',
      scoreFieldName: 'charactersScore',
      commentText: 'Characters Comments',
      commentFieldName: 'charactersComment',
      maxMarks: 10,
    }

  ];

  public form: UntypedFormGroup;
  public embedLink: string;
  public embedOrigin: string;
  public groupName: string;
  public dertYear: string;
  public dertVenue: string;
  public formSubmitted: boolean = false;
  public videoReady: boolean = false;
  public alreadyJudged: boolean = false;
  public detailHidden: boolean = false;
  public resultsPublished: boolean = false;

  constructor(
    private _dialog: MatDialog,
    private route: ActivatedRoute,
    private _conductor: Conductor,
    private _tracker: Tracker,
    private _sessionService: SessionService,
    private _router: Router,
    private _clientSettings: ClientSettingsService,
  ) {
  }

  ngOnInit() {

    this.form = this.toFormControlGroup();

    this._subscriptions.push(this.route.params.subscribe(params => {
      this._conductor.initScoreEntry(+params['id']); // (+) converts string 'id' to a number
    }));

    this._subscriptions.push(this._tracker.activeSubmission$.subscribe(activeSubmission => {
      if (activeSubmission != null) {
        this.embedLink = activeSubmission.embedLink;
        this.embedOrigin = activeSubmission.embedOrigin;
        this.groupName = activeSubmission.groupName;
        this.dertYear = activeSubmission.dertYearFrom;
        this.dertVenue = activeSubmission.dertVenueFrom;
        this.videoReady = true;
        this.alreadyJudged = this._sessionService.getDancesJudged().some(s => s === activeSubmission.id);
      }
    }));

    if (this._clientSettings.dodResultsPublished) {
      this.resultsPublished = true;
    }
  }

  ngOnDestroy() {
    this._subscriptions.forEach(s => s.unsubscribe());
    this._subscriptions = [];
  }

  public onReturnToListClick() {
    this._router.navigate(['/dertofderts/judging']);
  }

  public submitScoresForm() {

    if (this.form.valid) {
      this.openDialog();
    } else {
      this.form.markAllAsTouched();
    }

  }

  public onHideDetailClick() {
    this.detailHidden = true;
  }

  /**
   * toFormControlGroup
   * Takes the scores against the current dance object and creates a form group for these items with validation.
   */
  private toFormControlGroup(): UntypedFormGroup {

    const group: any = {};
    this.scoreCategories.forEach((scoreCategory) => {

      const rangeValidator = new RangeValidatorDirective();
      rangeValidator.initValidationFunction(0, scoreCategory.maxMarks);
      // Add to the group always with required validation

      const validators = [Validators.required, rangeValidator.validatorFunction];
      group[scoreCategory.scoreFieldName] = new UntypedFormControl({ value: '0', disabled: this.formSubmitted }, validators);
      group[scoreCategory.commentFieldName] = new UntypedFormControl({ value: '', disabled: this.formSubmitted });

    });

    group['overallComments'] = new UntypedFormControl({ value: '', disabled: this.formSubmitted });

    return new UntypedFormGroup(group); // return the group.
  }

  private openDialog() {

    const scores = {};

    this._submitDialogRef = this._dialog.open(
      ScoreDialogComponent,
      {
        width: '600px',
        data: {
          form: this.form.value
        },
        panelClass: 'dod-dialog-panel'
      },
    );

    const dialogSubscription = this._submitDialogRef.afterClosed().subscribe(dialogResponseData => {
      dialogSubscription.unsubscribe();
      if (dialogResponseData) {
        // Set all the fields to disabled
        Object.keys(this.form.controls).forEach((key: string) => {
          const abstractControl = this.form.controls[key];
          abstractControl.disable();
        });

        // Open the submitted dialog
        this.openSubmittedDialog();

      } else {

        console.error('Submit Dialog Closed With No Data');
      }
    });
  }

  private openSubmittedDialog() {
    this._dialog.closeAll();

    this._submittedDialogRef = this._dialog.open(
      ScoreSubmittedDialogComponent,
      {
        width: '600px',
        panelClass: 'dod-dialog-panel'
      },
    );

    const dialogSubscription = this._submittedDialogRef.afterClosed().subscribe(() => {
      dialogSubscription.unsubscribe();
    });

  }
}
