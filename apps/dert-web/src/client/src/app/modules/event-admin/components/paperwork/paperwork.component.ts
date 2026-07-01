import { Component, OnInit } from '@angular/core';

import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
  selector: 'app-event-paperwork-blank',
  templateUrl: './paperwork.component.html',
  styleUrls: ['./paperwork.component.css']
})
export class PaperworkComponent implements OnInit {

  constructor(

    private _tracker: EventAdminTracker
  ) { }

  ngOnInit() {
  }

  public onSignInSheetsClick() {

    const window = this.getNativeWindow();

    window.open(`./paperwork/signinsheets/${this._tracker.eventId}`, '_blank');

  }

  private getNativeWindow(): Window {
    return window;
  }
}
