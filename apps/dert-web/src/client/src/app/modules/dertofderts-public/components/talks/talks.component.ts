import { Component, OnDestroy, OnInit } from '@angular/core';
import { DodTalkDto } from 'app/models/dto/DodTalkDto';
import { Observable, SubscriptionLike } from 'rxjs';
import { Conductor } from '../../services/dertofderts-public.conductor';
import { Tracker } from '../../services/dertofderts-public.tracker';

@Component({
  selector: 'app-talks',
  templateUrl: './talks.component.html',
  styleUrls: ['./talks.component.scss']
})
export class TalksComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  public talks: Array<DodTalkDto> = [];

  constructor(
    private _tracker: Tracker,
    private _conductor: Conductor
  ) { }

  ngOnInit() {

    this._subscriptions.push(this._tracker.dodTalks$.subscribe((talks: DodTalkDto[]) => {
      this.talks = talks;
    }));

    this._conductor.initTalks();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public hasLink(talk: DodTalkDto) {
    return talk.broadcastWebLink != null && talk.broadcastWebLink !== '';
  }

  public openLink(webLink: string) {
    window.open(webLink, '_blank');
  }

}
