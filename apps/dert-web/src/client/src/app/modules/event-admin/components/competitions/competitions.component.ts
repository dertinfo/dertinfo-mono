import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';

import { MatTabChangeEvent } from '@angular/material/tabs';
import { Flag } from 'app/models/app/Enumerations/Flags';
import { CompetitionSummaryDto, EventCompetitionDto, StatusBlockDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { EventCompetitionModel, StatusBlockModel } from './models/eventcompetition.model';

@Component({
  selector: 'app-event-competitions',
  templateUrl: './competitions.component.html',
  styleUrls: ['./competitions.component.css']
})
export class EventCompetitionsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public competitions: EventCompetitionModel[];
  public focussedCompetition: CompetitionSummaryDto;

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker,

  ) { }

  ngOnInit() {

    // The event number has changed
    this._subscriptions.push(this._activatedRoute.parent.params.subscribe(params => {
      this._eventConductor.initCompetitions(params['id']);
    }));

    this._subscriptions.push(this._eventTracker.competitions$.subscribe((competitions) => {

      if (competitions) {
        this.competitions = competitions.map((c) => {
          const eventCompetitionModel: EventCompetitionModel = {
            id: c.competitionId,
            name: c.competitionName,
            status: this.mapStatusBlockDtoToStatusBlockModel(c.status),
            entries: this.mapStatusBlockDtoToStatusBlockModel(c.entrants),
            venues: this.mapStatusBlockDtoToStatusBlockModel(c.venues),
            judges: this.mapStatusBlockDtoToStatusBlockModel(c.judges),
          };
          return eventCompetitionModel;
        });

      }
    }));
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public venuesClicked(competitionId: number) {

    const competition = this.competitions.find(ecm => ecm.id === competitionId);
    this._router.navigate(['competition', competition.name, competition.id, 'venues']);
  }
  public judgesClicked(competitionId: number) {

    const competition = this.competitions.find(ecm => ecm.id === competitionId);
    this._router.navigate(['competition', competition.name, competition.id, 'judges']);

  }
  public entrantsClicked(competitionId: number) {

    const competition = this.competitions.find(ecm => ecm.id === competitionId);
    this._router.navigate(['competition', competition.name, competition.id, 'entrants']);

  }
  public statusClicked(competitionId: number) {

    const competition = this.competitions.find(ecm => ecm.id === competitionId);
    this._router.navigate(['competition', competition.name, competition.id, 'overview']);

  }

  private mapStatusBlockDtoToStatusBlockModel(statusBlockDto: StatusBlockDto): StatusBlockModel {
    return {
      heading: statusBlockDto.title,
      icon: this.identifyIconForFlag(statusBlockDto.flag),
      iconClass: this.identifyIconClassForFlag(statusBlockDto.flag),
      subtext: statusBlockDto.subText
    };
  }

  private identifyIconForFlag(flag: Flag) {
    switch (flag) {
      case Flag.Critical: return 'error';
      case Flag.Error: return 'warning';
      case Flag.Warn: return 'warning';
      case Flag.Info: return 'info';
      case Flag.Ok: return 'check_circle';
      default: return 'remove_circle';
    }
  }

  private identifyIconClassForFlag(flag: Flag) {
    switch (flag) {
      case Flag.Critical: return 'critical';
      case Flag.Error: return 'error';
      case Flag.Warn: return 'warn';
      case Flag.Info: return 'info';
      case Flag.Ok: return 'ok';
      default: return '';
    }
  }
}
