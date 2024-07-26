import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EventShowcaseDetailDto, EventShowcaseDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { ShowcaseRepository } from '../../repositories';

@Component({
  selector: 'app-public-event',
  templateUrl: './public-event.component.html',
  styleUrls: ['./public-event.component.css']
})
export class PublicEventComponent implements OnInit, OnDestroy {

  private _eventId = 0;
  private _subscriptions: Subscription[] = [];

  public pEventDetail: EventShowcaseDetailDto;

  constructor(
    private activatedRoute: ActivatedRoute,
    private _showcaseRepository: ShowcaseRepository) { }

  ngOnInit() {

    const routeParamsSubscription = this.activatedRoute.params.subscribe(params => {
      this._eventId = params['id'];
      this.initEventShowcaseDetail(this._eventId);
    });
    this._subscriptions.push(routeParamsSubscription);
  }

  initEventShowcaseDetail(eventId: number) {
    if (this._eventId > 0) {
      const subs = this._showcaseRepository.getEvent(this._eventId).subscribe((pEventDetail => {
        subs.unsubscribe();
        this.pEventDetail = pEventDetail;
      }));
    }
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
