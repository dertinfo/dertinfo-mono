import { Component, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSidenav } from '@angular/material/sidenav';
import { NavigationEnd, Router } from '@angular/router';
import * as moment from 'moment';
import { SubscriptionLike } from 'rxjs';
import { NotificationSummary } from '../../models/NotificationSummary';
import { NotificationSummaryDto } from '../../models/NotificationSummaryDto';
import { NotificationConductor } from '../../services/notification.conductor';
import { NotificationTracker } from '../../services/notification.tracker';
import { DetailDialogComponent } from '../detail-dialog/detail-dialog.component';

@Component({
  selector: 'app-notifications-drawer',
  templateUrl: './drawer.component.html',
  styleUrls: ['./drawer.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DrawerComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  @Input() notificPanel;

  public notificationSummaries: Array<NotificationSummary> = [];

  // Dummy notifications
  // notifications = [{
  //     message: 'No Notifictions To Show!',
  //     icon: 'assignment_ind',
  //     time: '',
  //     route: '',
  //     color: 'primary'
  //   }]

  notifications = [{
    message: 'New contact added',
    icon: 'assignment_ind',
    time: '1 min ago',
    route: '/inbox',
    color: 'primary'
  }, {
    message: 'New message',
    icon: 'chat',
    time: '4 min ago',
    route: '/chat',
    color: 'accent'
  }, {
    message: 'Server rebooted',
    icon: 'settings_backup_restore',
    time: '12 min ago',
    route: '/charts',
    color: 'warn'
  }];

  constructor(
    private router: Router,
    private condudtor: NotificationConductor,
    private tracker: NotificationTracker,
    private dialog: MatDialog
  ) { }

  ngOnInit() {

    // Close the panel if we go to another page.
    this._subscriptions.push(this.router.events.subscribe((routeChange) => {
      if (routeChange instanceof NavigationEnd) {
        this.notificPanel.close();
      }
    }));

    this._subscriptions.push(this.tracker.summaries$.subscribe((summaries) => {
      this.notificationSummaries = summaries.map(s => this.mapNotificationDto(s));
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  clearAll(e) {
    e.preventDefault();
    this.notificationSummaries = this.notificationSummaries.filter(ns => !ns.canDismiss);
  }

  public onDismissClick(notificationSuammry: NotificationSummary) {
    this.dismissMessageSummary(notificationSuammry.id);
  }

  public onOpenClick(notificationSuammry: NotificationSummary) {
    this.openNotificationDetailDialog(notificationSuammry.id);
  }

  private openNotificationDetailDialog(notificationLogId: number) {
    const dialogRef = this.dialog.open(DetailDialogComponent, {
      width: '640px',
      maxHeight: '95vh',
      disableClose: false,
      data: {notificationLogId}
    });
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  private dismissMessageSummary(notificationLogId: number) {
    this.condudtor.dismissMessage(notificationLogId);
  }

  private mapNotificationDto(dto: NotificationSummaryDto): NotificationSummary {

    const dateCreated = moment(dto.dateCreated);
    return {
      id: dto.notificationAudienceLogId,
      title: dto.title,
      time: dateCreated.fromNow(),
      summary: dto.summary,
      icon: this.parseIconType(dto.messageType),
      showAsDeleted: dto.hasBeenDeleted,
      showAsNew: dto.canOpen && !dto.hasBeenOpened || !dto.hasBeenSeen,
      canDismiss: dto.canDismiss,
      canOpen: dto.canOpen,
      severity: dto.severity
    };
  }

  private parseIconType(typeId: number): string {
    switch (typeId) {
      case 1: return 'settings_backup_restore';
      case 2: return 'chat';
      case 3: return 'assignment_ind';
      default:
        return 'assignment_ind';

    }
  }
}
