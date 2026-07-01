import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DodResultDto } from 'app/models/dto/DodResultDto';
import { DodResultSubmissionDto } from 'app/models/dto/DodResultSubmissionDto';
import { WebsiteTermsComponent } from 'app/regions/terms/components/website-terms/website-terms.component';
import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { ScoreFormData } from '../../models/scoreFormData.model';
import { Conductor } from '../../services/dertofderts-public.conductor';
import { SessionService } from '../../services/dertofderts-public.session';
import { Tracker } from '../../services/dertofderts-public.tracker';

@Component({
  selector: 'app-scoredialog',
  templateUrl: './scoredialog.component.html',
  styleUrls: ['./scoredialog.component.scss']
})
export class ScoreDialogComponent implements OnInit {

  public dialogResponseData: any = {};

  public formGroup: UntypedFormGroup;
  public sessionPreLoaded = false;
  public hasError = false;
  public hasErrorMessage = '';
  public isSubmitting = false;

  constructor(
    public dialogRef: MatDialogRef<ScoreDialogComponent>,
    private _conductor: Conductor,
    private _tracker: Tracker,
    private _sessionService: SessionService,
    private _dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { form: any }
  ) { }

  ngOnInit() {
    this.prepareForm();
  }

  public submitForm() {

    if (!this.isSubmitting) {
      this.isSubmitting = true;

      setTimeout(() => {
        if (this.formGroup.valid || this.sessionPreLoaded) {

          const obs$ = this._conductor.submitScores(this._tracker.currentSubmissionId, this.data.form, this.formGroup.value);

          const subs = obs$.subscribe(
            (dodResultDto) => {
              this.isSubmitting = false;
              this.submitSucceeded(dodResultDto);
            },
            (err) => {
              this.isSubmitting = false;
              this.submitFailed(err.error);
            },
            () => { /* Observable Completed */ }
          );

        } else {
          this.formGroup.markAllAsTouched();
          this.isSubmitting = false;
        }
      }, 250);
    }
  }

  public onNotMeClick() {

    const subs = this._conductor.confirmClearSession().subscribe((cleared) => {
      subs.unsubscribe();

      if (cleared) {
        this.sessionPreLoaded = false;
        this.prepareForm();
      }
    });
  }

  public onTermsAndConditionsClick() {
    this._dialog.open(WebsiteTermsComponent, {
      width: '850px',
      height: '80vh'
    });
  }

  private submitSucceeded(dodResultDto: DodResultDto) {
    this.dialogRef.close(this.dialogResponseData);
  }

  private submitFailed(error: any) {
    this.hasError = true;
    this.hasErrorMessage = error;
  }

  private prepareForm() {

    const sessionInfo = this._sessionService.getSessionInfo();
    const localInfo = this._sessionService.getLocalInfo();

    if (sessionInfo != null && localInfo != null) { this.sessionPreLoaded = true; }

    // Identify the default values depending on session state
    const userGuid = this.sessionPreLoaded ? localInfo.userGuid : null;
    const name = this.sessionPreLoaded ? sessionInfo.name : '';
    const email = this.sessionPreLoaded ? sessionInfo.email : '';
    const interestedInJudging = this.sessionPreLoaded ? sessionInfo.interestedInJudging : false;
    const officialJudge = this.sessionPreLoaded ? sessionInfo.officialJudge : false;
    const officialJudgePassword = this.sessionPreLoaded && userGuid ? '**********' : '';
    const agreeToTermsAndConditions = this.sessionPreLoaded ? sessionInfo.agreeToTermsAndConditions : false;

    // Build the form group
    this.formGroup = new UntypedFormGroup({
      name: new UntypedFormControl(name, [Validators.required]),
      email: new UntypedFormControl(email, [Validators.required, customEmailValidator()]),
      interestedInJudging: new UntypedFormControl(interestedInJudging, []),
      officialJudge: new UntypedFormControl(officialJudge, []),
      officialJudgePassword: new UntypedFormControl(officialJudgePassword, []),
      agreeToTermsAndConditions: new UntypedFormControl(agreeToTermsAndConditions, []),
    });

    this.dialogResponseData.userGuid = userGuid;
  }

}
