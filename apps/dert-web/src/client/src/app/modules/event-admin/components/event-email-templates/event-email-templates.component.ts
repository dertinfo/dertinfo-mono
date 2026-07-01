import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EmailTemplateDetailDto, EmailTemplateDto, EmailTemplateUpdateSubmissionDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
  selector: 'app-event-email-templates',
  templateUrl: './event-email-templates.component.html',
  styleUrls: ['./event-email-templates.component.css']
})
export class EventEmailTemplatesComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public isReady = true;
  public emailTemplates: Array<EmailTemplateDto>;
  public emailTemplateDetail: EmailTemplateDetailDto;
  public showEditor = false;

  public emailTemplateUpdateForm: UntypedFormGroup;

  public editorData;
  public selectedTemplate;

  // public emailTemplates = [
  //   {
  //     id: 1022,
  //     templateRef: 'REGISTRATION_SUBMIT',
  //     templateName: 'Registration Submit',
  //     subject: 'Registration Received'
  //   },
  //   {
  //     id: 1023,
  //     templateRef: 'REGISTRATION_CONFIRM',
  //     templateName: 'Registration Confirmation',
  //     subject: 'Registration Confirmed'
  //   }
  // ];

  constructor(

    private _activatedRoute: ActivatedRoute,
    private _conductor: EventAdminConductor,
    private _tracker: EventAdminTracker
  ) { }

  ngOnInit() {

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyEmailTemplates(this._activatedRoute.snapshot.data.emailTemplates);

    this._subscriptions.push(this._tracker.emailTemplates$.subscribe((templates) => {
      this.emailTemplates = templates;
    }));

    this._subscriptions.push(this._tracker.focussedEmailTemplate$.subscribe((emailTemplateDetail) => {
      if (emailTemplateDetail) {

        // Set the detail and build the form.
        this.emailTemplateDetail = emailTemplateDetail;
        this.emailTemplateUpdateForm = new UntypedFormGroup({
          emailTemplateName: new UntypedFormControl(this.emailTemplateDetail.templateName, [Validators.required]),
          emailTemplateSubject: new UntypedFormControl(this.emailTemplateDetail.subject, [Validators.required]),
          emailTemplateBody: new UntypedFormControl(this.emailTemplateDetail.body, [Validators.required])
        });
        this.editorData = emailTemplateDetail.body;

        // Now show the form
        this.showEditor = true;
      }
    }));

    // Replaced by resolver
    // this._subscriptions.push(this._activatedRoute.parent.params.subscribe(params => {
    //   this._conductor.initEmailTemplates(params['id']);
    // }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onEmailTemplateSubmit() {

    if (this.emailTemplateUpdateForm.valid) {
      const emailTemplateUpdateSubmissionDto: EmailTemplateUpdateSubmissionDto = {
        id: this.emailTemplateDetail.id,
        eventId: this.emailTemplateDetail.eventId,
        templateName: this.emailTemplateUpdateForm.value.emailTemplateName,
        subject: this.emailTemplateUpdateForm.value.emailTemplateSubject,
        body: this.emailTemplateUpdateForm.value.emailTemplateBody,
      };

      this._conductor.updateEmailTemplate(emailTemplateUpdateSubmissionDto);
      this.showEditor = false;
    }
  }

  public onEmailTemplateSelectChange() {
    console.log('EventEmailTemplatesComponent - onEmailTemplateSelectChange');

    this._conductor.initFocussedEmailTemplate(this.selectedTemplate.id);
  }

  public onEditorContentChanged() {

    if (this.editorData !== this.emailTemplateUpdateForm.value.emailTemplateBody) {
      this.emailTemplateUpdateForm.controls.emailTemplateBody.markAsDirty();
    }

    this.emailTemplateUpdateForm.setValue({
      emailTemplateName: this.emailTemplateUpdateForm.value.emailTemplateName,
      emailTemplateSubject: this.emailTemplateUpdateForm.value.emailTemplateSubject,
      emailTemplateBody: this.editorData
    });
  }

  public onEditorSelectionChanged() {

  }
}
