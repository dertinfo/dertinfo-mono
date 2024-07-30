import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { NotificationMessageSubmissionDto } from '../../models/NotificationMessageSubmissionDto';
import { NotificationAdminMediator } from '../../services/notification-management.mediator';

@Component({
  selector: 'app-create-dialog',
  templateUrl: './create-dialog.component.html',
  styleUrls: ['./create-dialog.component.scss']
})
export class CreateDialogComponent implements OnInit {

  public form: UntypedFormGroup;
  public editorData: UntypedFormGroup;

  constructor(
    private _mediator: NotificationAdminMediator,
  ) { }

  ngOnInit() {

    this.form = new UntypedFormGroup({
      messageHeader: new UntypedFormControl('', [Validators.required]),
      messageSummary: new UntypedFormControl('', [Validators.required]),
      messageBody: new UntypedFormControl('', []),
      hasDetails: new UntypedFormControl('', []),
      requiresOpening: new UntypedFormControl('', []),
      requiresAcknowledgement: new UntypedFormControl('', []),
      severity: new UntypedFormControl(1, [Validators.required]),
      blocksUser: new UntypedFormControl('', []),
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
