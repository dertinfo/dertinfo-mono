import { Component, EventEmitter, HostListener, OnDestroy, OnInit, Output } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { SubscriptionLike } from 'rxjs';

import { NotificationConductor } from '../../services/notification.conductor';
import { NotificationTracker } from '../../services/notification.tracker';
import { DetailDialogComponent } from '../detail-dialog/detail-dialog.component';

@Component({
  selector: 'app-notifications-topbar-button',
  templateUrl: './topbar-button.component.html',
  styleUrls: ['./topbar-button.component.scss'],
})
export class TopbarButtonComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  public maxSeverity: number = 0;
  public hasNotifications: boolean = false;
  public showSecondary: boolean = true;
  public showAccent: boolean = false;
  public showWarn: boolean = false;
  public showNone: boolean = false;

  constructor(
    private conductor: NotificationConductor,
    private tracker: NotificationTracker,
    private dialog: MatDialog
  ) { }

  @HostListener('click')
  userClickedForMessages() {
    this.conductor.getMessageSummaries();
  }

  ngOnInit() {

    this._subscriptions.push(this.tracker.thumbnailInfo$.subscribe((thumbNailInfo) => {
      if (thumbNailInfo) {
        console.log('TopbarButtonComponent - thumbnailInfo$ - thumbNailInfo ###', thumbNailInfo);
        this.maxSeverity = thumbNailInfo.maximumMessageSeverity;
        this.hasNotifications = thumbNailInfo.hasUnreadMessages;
        if (thumbNailInfo.hasBlocking) {
          this.openBlockingDialog(thumbNailInfo.blockingNotificationLogId);
        }
      }
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public getSeverityClass() {
    switch (this.maxSeverity) {
      case 3:
        return 'mat-bg-warn';
      case 2:
        return 'mat-bg-accent';
      case 1:
      case 0:
        return 'mat-bg-secondary';
      default:
        console.error('severity value out of range');
    }
  }

  private openBlockingDialog(notificationLogId) {
    const dialogRef = this.dialog.open(DetailDialogComponent, {
      width: '640px',
      maxWidth: '95vw',
      maxHeight: '95vh',
      disableClose: true,
      data: {
        notificationLogId: notificationLogId,
        disableCancel: true,
      }
    });
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }
}
