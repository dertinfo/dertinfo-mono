import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DodTalkSubmissionDto } from 'app/models/dto/DodTalkSubmissionDto';
import { Conductor } from '../../../../services/dertofderts-admin.conductor';
import { Mediator } from '../../../../services/dertofderts-admin.mediator';
import { Tracker } from '../../../../services/dertofderts-admin.tracker';

@Component({
  selector: 'app-create-talk',
  templateUrl: './create-talk.component.html',
  styleUrls: ['./create-talk.component.scss']
})
export class CreateTalkComponent implements OnInit {

  public form: FormGroup;

  constructor(
    private _conductor: Conductor,
    private _mediator: Mediator,
    private _dialog: MatDialog,
  ) { }

  ngOnInit() {

    this.form = new FormGroup({
      title: new FormControl('', [Validators.required]),
      subTitle: new FormControl('', []),
      description: new FormControl('', [Validators.required]),
      broadcastDateTime: new FormControl('', [Validators.required]),
      broadcastWebLink: new FormControl('', []),
    });
  }

  public onCancelClick() {
    this._mediator.announceCloseModal();
  }

  public onCreateNewClick() {
    if (this.form.valid) {
      const newSubmission: DodTalkSubmissionDto = {
        title: this.form.value.title,
        subTitle: this.form.value.subTitle,
        description: this.form.value.description,
        broadcastDateTime: this.form.value.broadcastDateTime,
        broadcastWebLink: this.form.value.broadcastWebLink
      };

      this._mediator.announceNewTalk(newSubmission);
    }
  }

}
