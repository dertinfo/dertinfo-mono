import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject ,  Observable ,  Subscription } from 'rxjs';
import { EventDto, GroupRegistrationDto } from '../../../../models/dto';
import { GroupAdminConductor } from '../../services/group-admin.conductor';
import { GroupAdminTracker } from '../../services/group-admin.tracker';

@Component({
  selector: 'app-group-registrations',
  templateUrl: './group-registrations.component.html',
  styleUrls: ['./group-registrations.component.css']
})
export class GroupRegistrationsComponent implements OnInit, OnDestroy {

  private activeRegistrationsSubject = new BehaviorSubject<GroupRegistrationDto[]>([]);
  private previousRegistrationsSubject = new BehaviorSubject<GroupRegistrationDto[]>([]);
  private eventsAvailableSubject = new BehaviorSubject<EventDto[]>([]);
  private otherRegistrationsSubject = new BehaviorSubject<GroupRegistrationDto[]>([]);

  private _subscriptions: Subscription[] = [];

  private groupRegistrations$;

  private _activeFlowStates;
  private _previousFlowStates;
  private _groupId;

  get groupActiveRegistrations$(): Observable<GroupRegistrationDto[]> {
    return this.activeRegistrationsSubject.asObservable();
  }

  get groupPreviousRegistrations$(): Observable<GroupRegistrationDto[]> {
    return this.previousRegistrationsSubject.asObservable();
  }

  get eventsAvailable$(): Observable<EventDto[]> {
    return this.eventsAvailableSubject.asObservable();
  }

  get otherRegistrations$(): Observable<GroupRegistrationDto[]> {
    return this.otherRegistrationsSubject.asObservable();
  }

  constructor(
    private router: ActivatedRoute,
    private groupConductor: GroupAdminConductor,
    private _groupTracker: GroupAdminTracker
    ) { }

  ngOnInit() {

    this._activeFlowStates = this.groupConductor.registrationActiveFlowStates();
    this._previousFlowStates = this.groupConductor.registrationInActiveFlowStates();

    const groupRegistrationsSubscription = this._groupTracker.groupRegistrations$.subscribe(
      (allGroupRegistrations) => {

        const activeRegistrations = allGroupRegistrations.filter((registration) => {
          return this._activeFlowStates.indexOf(registration.flowState) > -1;
        });

        const previousRegistrations = allGroupRegistrations.filter((registration) => {
          return this._previousFlowStates.indexOf(registration.flowState) > -1;
        });

        const otherRegistrations = allGroupRegistrations.filter((registration) => {

          const isNotActive = this._activeFlowStates.indexOf(registration.flowState) === -1;
          const isNotPassed = this._previousFlowStates.indexOf(registration.flowState) === -1;

          return  isNotActive && isNotPassed;
        });

        this.activeRegistrationsSubject.next(activeRegistrations);
        this.previousRegistrationsSubject.next(previousRegistrations);
        this.otherRegistrationsSubject.next(otherRegistrations);
      }
    );

    const eventsAvailableSubscription = this._groupTracker.availableEvents$.subscribe((allEventsAvailable) => {
      this.eventsAvailableSubject.next(allEventsAvailable);
    });

    const routeParamsSubscription = this.router.parent.params.subscribe(params => {
      this._groupId = params['id'];
      this.groupConductor.initRegistrations(params['id']);
      this.groupConductor.initEventsAvailable();
    });

    this._subscriptions.push(groupRegistrationsSubscription);
    this._subscriptions.push(eventsAvailableSubscription);
    this._subscriptions.push(routeParamsSubscription);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
