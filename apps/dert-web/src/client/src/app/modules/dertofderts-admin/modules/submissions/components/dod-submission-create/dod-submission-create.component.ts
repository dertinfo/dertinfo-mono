import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { GroupDto } from 'app/models/dto';
import { DodSubmissionSubmissionDto } from 'app/models/dto/DodSubmissionSubmissionDto';
import { Observable } from 'rxjs';
import { Conductor } from '../../../../services/dertofderts-admin.conductor';
import { Mediator } from '../../../../services/dertofderts-admin.mediator';
import { Tracker } from '../../../../services/dertofderts-admin.tracker';

@Component({
  selector: 'app-dod-submission-create',
  templateUrl: './dod-submission-create.component.html',
  styleUrls: ['./dod-submission-create.component.scss']
})
export class DodSubmissionCreateComponent implements OnInit {

  public groups$: Observable<GroupDto[]>;

  public form: UntypedFormGroup;

  constructor(
    private _conductor: Conductor,
    private _tracker: Tracker,
    private _mediator: Mediator
  ) { }

  ngOnInit() {

    this._conductor.initGroups();

    this.groups$ = this._tracker.groups$;

    this.form = new UntypedFormGroup({
      groupId: new UntypedFormControl('', [Validators.required]),
      embedLink: new UntypedFormControl('', [Validators.required]),
      embedOrigin: new UntypedFormControl('youtube', []),
      dertYearFrom: new UntypedFormControl('', [Validators.required]),
      dertVenueFrom: new UntypedFormControl('', []),
      entryCategory: new UntypedFormControl('Premier', []),
    });
  }

  public onCancelClick() {
    this._mediator.announceCloseModal();
  }

  public onCreateNewClick() {
    if (this.form.valid) {
      const newSubmission: DodSubmissionSubmissionDto = {
        groupId: this.form.value.groupId,
        embedLink: this.form.value.embedLink,
        embedOrigin: this.form.value.embedOrigin,
        dertYearFrom: this.form.value.dertYearFrom,
        dertVenueFrom: this.form.value.dertVenueFrom,
        isPremier: this.form.value.entryCategory === 'Premier',
        isChampionship: this.form.value.entryCategory === 'Championship',
        isOpen: this.form.value.entryCategory === 'Open',
      };

      this._mediator.announceNewSubmission(newSubmission);
    }
  }
}
