import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { SubscriptionLike } from 'rxjs';
import { NotificationMessageDto } from '../../models/NotificationMessageDto';
import { NotificationMessageSubmissionDto } from '../../models/NotificationMessageSubmissionDto';
import { NotificationAdminConductor } from '../../services/notification-management.conductor';
import { NotificationAdminMediator } from '../../services/notification-management.mediator';
import { NotificationManagementTracker } from '../../services/notification-management.tracker';
import { CreateDialogComponent } from '../create-dialog/create-dialog.component';

@Component({
  selector: 'app-notifications-admin-list',
  templateUrl: './admin-list.component.html',
  styleUrls: ['./admin-list.component.scss']
})
export class AdminListComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  public dataSource = new MatTableDataSource<NotificationMessageDto>();
  public displayedColumns: string[] = ['dateCreated', 'messageHeader', 'hasDetails', 'isDismissable', 'severity', 'blocksUser', 'buttons'];

  constructor(
    private conductor: NotificationAdminConductor,
    private tracker: NotificationManagementTracker,
    private mediator: NotificationAdminMediator,
    private dialog: MatDialog
  ) { }

  ngOnInit() {

    this._subscriptions.push(this.mediator.closeModal$.subscribe(() => {
      this.dialog.closeAll();
    }));

    this._subscriptions.push(this.tracker.allNotificationMessages$.subscribe((notifications: Array<NotificationMessageDto>) => {
      this.dataSource.data = notifications;
    }));

    this._subscriptions.push(this.mediator.newNotificationMessage$.subscribe((newNotificationMessage: NotificationMessageSubmissionDto) => {
      this.conductor.addNotificationMessage(newNotificationMessage);
      this.dialog.closeAll();
    }));

    this.conductor.getAllMessages();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

  openCreateDialog() {
    const dialogRef = this.dialog.open(CreateDialogComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  public delete(notificationMessage: NotificationMessageDto) {
    this.conductor.deleteNotificationMessage(notificationMessage);
  }

}
