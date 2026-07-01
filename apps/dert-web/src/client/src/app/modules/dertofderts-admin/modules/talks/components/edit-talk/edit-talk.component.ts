import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DodTalkDto } from 'app/models/dto/DodTalkDto';
import { DodTalkUpdateSubmissionDto } from 'app/models/dto/DodTalkUpdateSubmissionDto';
import { SubscriptionLike } from 'rxjs';
import { Conductor } from '../../../../services/dertofderts-admin.conductor';
import { Tracker } from '../../../../services/dertofderts-admin.tracker';

@Component({
  selector: 'app-edit-talk',
  templateUrl: './edit-talk.component.html',
  styleUrls: ['./edit-talk.component.css']
})
export class EditTalkComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  public isReady = false;
  public talk: DodTalkDto;
  public form: UntypedFormGroup;

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: Conductor,
    private _tracker: Tracker
  ) { }

  ngOnInit() {

    const subsA = this._activatedRoute.params.subscribe(params => {
      this._conductor.initFocussedTalk(+params['id']);
    });

    const subsB = this._tracker.focussedTalk$.subscribe((talk) => {
      if (talk) {
        this.talk = talk;

        this.form = new UntypedFormGroup({
          title: new UntypedFormControl(this.talk.title, [Validators.required]),
          subTitle: new UntypedFormControl(this.talk.subTitle, []),
          description: new UntypedFormControl(this.talk.description, [Validators.required]),
          broadcastDateTime: new UntypedFormControl(this.talk.broadcastDateTime, [Validators.required]),
          broadcastWebLink: new UntypedFormControl(this.talk.broadcastWebLink, []),
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

      const talkUpdate: DodTalkUpdateSubmissionDto = {
        talkId: this.talk.id,
        title: this.form.value.title,
        subTitle: this.form.value.subTitle,
        description: this.form.value.description,
        broadcastDateTime: this.form.value.broadcastDateTime,
        broadcastWebLink: this.form.value.broadcastWebLink
      };

      this._conductor.updateTalk(talkUpdate);
    }
  }

}
