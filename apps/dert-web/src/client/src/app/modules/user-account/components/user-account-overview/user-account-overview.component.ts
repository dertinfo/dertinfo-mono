import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { UserAccountConductor } from '../../services/user-account.conductor';

@Component({
    selector: 'app-user-account-overview',
    templateUrl: './user-account-overview.component.html',
    styleUrls: ['./user-account-overview.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class UserAccountOverviewComponent implements OnInit {

  constructor(private _conductor: UserAccountConductor) { }

  ngOnInit() {

    this._conductor.initOverview();
  }

}
