import { Component, Inject, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SubscriptionLike } from 'rxjs';
import { NotificationDetailDto } from '../../models/NotificationDetailDto';
import { NotificationConductor } from '../../services/notification.conductor';
import { NotificationTracker } from '../../services/notification.tracker';

@Component({
  selector: 'app-detail-dialog',
  templateUrl: './detail-dialog.component.html',
  styleUrls: ['./detail-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DetailDialogComponent implements OnInit, OnDestroy {

  private _subscriptions: Array<SubscriptionLike> = [];

  public notificationDetail: NotificationDetailDto = null;
  public hasDetailLoaded: boolean = false;
  public hideCancelButton: boolean = false;

  public isAcknowledging = false;

  constructor(
    public dialogRef: MatDialogRef<DetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private conductor: NotificationConductor,
    private tracker: NotificationTracker
  ) { }

  ngOnInit() {
    // Listen for the notification loaded.
    this._subscriptions.push(this.tracker.focussedNotification$.subscribe((notificationDetail => {
      if (notificationDetail) {
        this.notificationDetail = notificationDetail;
        this.hasDetailLoaded = true;
        this.hideCancelButton = notificationDetail.requestAcknowledgement;
        this.conductor.tagNotificationOpened(this.data.notificationLogId);
      }
    })));

    // Load the message
    if (this.data.notificationLogId > 0) {
      this.conductor.getMessageDetail(this.data.notificationLogId);
      this.hideCancelButton = this.data.disableCancel ? true : false;
    }
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onAcknowledgeClick() {
    this.isAcknowledging = true;

    const subs = this.conductor.tagNotificationAcknowledged(this.data.notificationLogId).subscribe(() => {
      subs.unsubscribe();
      this.isAcknowledging = false;
      this.dialogRef.close();
    });
  }

  public onCancel() {
    this.dialogRef.close();
  }
}
