import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NotificationMessageSubmissionDto } from '../../models/NotificationMessageSubmissionDto';
import { NotificationAdminMediator } from '../../services/notification-management.mediator';

@Component({
  selector: 'app-create-dialog',
  templateUrl: './create-dialog.component.html',
  styleUrls: ['./create-dialog.component.scss']
})
export class CreateDialogComponent implements OnInit {

  public form: FormGroup;
  public editorData: FormGroup;

  constructor(
    private _mediator: NotificationAdminMediator,
  ) { }

  ngOnInit() {

    this.form = new FormGroup({
      messageHeader: new FormControl('', [Validators.required]),
      messageSummary: new FormControl('', [Validators.required]),
      messageBody: new FormControl('', []),
      hasDetails: new FormControl('', []),
      requiresOpening: new FormControl('', []),
      requiresAcknowledgement: new FormControl('', []),
      severity: new FormControl(1, [Validators.required]),
      blocksUser: new FormControl('', []),
    });
  }

  public onCancelClick() {
    this._mediator.announceCloseModal();
  }

  public onCreateNewClick() {
    if (this.form.valid) {
      const newSubmission: NotificationMessageSubmissionDto = {
        messageHeader: this.form.value.messageHeader,
        messageSummary: this.form.value.messageSummary,
        messageBody: this.form.value.messageBody,
        hasDetails: this.form.value.hasDetails || false,
        requiresOpening: this.form.value.requiresOpening || false,
        requiresAcknowledgement: this.form.value.requiresAcknowledgement || false,
        severity: this.form.value.severity,
        blocksUser: this.form.value.blocksUser || false
      };

      this._mediator.announceNewNotificationMessage(newSubmission);
    }
  }

  public onEditorContentChanged() {

    if (this.editorData !== this.form.value.body) {
      this.form.controls.body.markAsDirty();
    }
  }

  public onEditorSelectionChanged() {

  }
}
