import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { GroupSetupConductor } from './services/group-setup.conductor';
import { GroupSetupTracker } from './services/group-setup.tracker';

@Component({
  selector: 'app-group-setup',
  templateUrl: './group-setup.component.html'
})
export class GroupSetupComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private activeView: string = 'start';

  public informationHeader = '';
  public informationBody = '';

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: GroupSetupConductor,
    private _tracker: GroupSetupTracker,
    private _router: Router
  ) { }

  ngOnInit() {

    // Listen for changes to Route Paramaters
    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      this._tracker.reset();
      this._conductor.setOverview(this._activatedRoute.snapshot.data.groupConfigureOverview);
      this._conductor.initUserData();
      this.activeView = this._activatedRoute.snapshot.params['view'];
    }));

    this._subscriptions.push(this._tracker.configurationHelp$.subscribe(help => {
      if (help != null) {
        this.informationHeader = help.header;
        this.informationBody = help.body;
      }
    }));
  }

  onAbandonConfigurationClick() {
    const obs$ = this._conductor.abandonConfiguration();

    const subs = obs$.subscribe(() => {
      this._conductor.clear();
      // this.router.navigate(['group', escape(groupDto.name), groupDto.id]);
      this._router.navigate(['dashboard']);
    });
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
