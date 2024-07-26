import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserAccountConductor } from './services/user-account.conductor';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.css']
})
export class UserAccountComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public email = '';
  public telephone = '';
  public gravitarLink = '';
  public fullname = '';

  constructor(
    private _conductor: UserAccountConductor
  ) { }

  ngOnInit() {
    this.subscribe();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  private subscribe() {
    const overviewSubs = this._conductor.userOverview$.subscribe(userOverview => {
      if (userOverview != null) {
        this.email = userOverview.email;
        this.telephone = userOverview.telephone;
        this.fullname = `${userOverview.firstName} ${userOverview.lastName}`;
        this.gravitarLink = userOverview.picture;
      }
    });

    this._subscriptions.push(overviewSubs);
  }
}
