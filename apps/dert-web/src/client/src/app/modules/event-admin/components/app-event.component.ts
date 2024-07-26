import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventOverviewDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { EventAdminConductor } from '../services/event-admin.conductor';
import { EventAdminTracker } from '../services/event-admin.tracker';

@Component({
  selector: 'app-event',
  templateUrl: './app-event.component.html'
})
export class EventComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private eventOverview: Observable<EventOverviewDto>;

  private eventId: number;
  public eventName: string;
  public eventBio: string;
  public eventEmail: string;
  public eventSynopsis: string;
  public eventPictureUrl: string;
  public contactName: string;
  public contactTelephone: string;
  public locationTown: string;

  activeView: string = 'overview';

  // Doughnut
  doughnutChartColors: any[] = [{
    backgroundColor: ['#fff', 'rgba(0, 0, 0, .24)', ]
  }];

  total1: number = 500;
  data1: number = 200;
  doughnutChartData1: number[] = [this.data1, (this.total1 - this.data1)];

  total2: number = 1000;
  data2: number = 400;
  doughnutChartData2: number[] = [this.data2, (this.total2 - this.data2)];

  doughnutChartType = 'doughnut';
  doughnutOptions: any = {
    cutoutPercentage: 85,
    responsive: true,
    maintainAspectRatio: true,
    legend: {
      display: false,
      position: 'bottom'
    },
    elements: {
      arc: {
        borderWidth: 0,
      }
    },
    tooltips: {
      enabled: false
    }
  };

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker,

    ) { }

  ngOnInit() {

    // Listen for changes to Route Paramaters
    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      // Insert the data from the resolver
      this._eventConductor.setOverview(this._activatedRoute.snapshot.data.eventOverview);
      this.activeView = this._activatedRoute.snapshot.params['view'];
    }));

    // Listen for changes to Overview
    this._subscriptions.push(this._eventTracker.eventOverview$.subscribe((eventOverview) => {
      if (eventOverview) {
        this.eventId = eventOverview.id;
        this.eventName = eventOverview.name;
        this.eventBio = eventOverview.eventSynopsis;
        this.eventEmail = eventOverview.contactEmail;
        this.eventSynopsis = eventOverview.eventSynopsis;
        this.eventPictureUrl = eventOverview.eventPictureUrl;
        this.contactName = eventOverview.contactName;
        this.contactTelephone = eventOverview.contactTelephone;
        this.locationTown = eventOverview.locationTown;
      }
    }));
  }

  ngOnDestroy() {

    this._eventTracker.reset();

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
