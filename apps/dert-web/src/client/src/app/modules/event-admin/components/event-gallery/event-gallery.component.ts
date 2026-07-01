import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { Observable ,  Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';

import { EventImage } from 'app/models/app';
import { EventImageDto } from 'app/models/dto';
import { EventAdminTracker } from '../../services/event-admin.tracker';
import { UploadImageComponent } from '../upload-image/upload-image.component';

@Component({
  selector: 'app-event-gallery',
  templateUrl: './event-gallery.component.html',
  styleUrls: ['./event-gallery.component.css']
})
export class EventGalleryComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private routeEventId: number;
  public eventImages$: Observable<EventImage[]>;

  constructor(
    public composeDialog: MatDialog,
    private router: ActivatedRoute,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker
  ) { }

  ngOnInit() {

    this.eventImages$ = this._eventTracker.eventImages$; // Subscribe Direct uses | async

    this._subscriptions.push(this.router.parent.params.subscribe(params => {
      this.routeEventId = parseInt(params['id'], 10);
      this._eventConductor.initImages(this.routeEventId);
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  openUploadImageDialog() {
    const dialogRef = this.composeDialog.open(UploadImageComponent);
    dialogRef.afterClosed().subscribe(result => {
      this._eventConductor.refreshImages();
    });
  }

  onSetPrimaryClick(eventImage: EventImage) {
      this._eventConductor.setPrimaryImage(eventImage);
  }

  onDownloadClick(eventImage: EventImageDto) {
    const originalImageUrl = eventImage.imageResourceUri;
  }

  onDeleteClick(eventImage: EventImageDto) {
    this._eventConductor.deleteImage(eventImage);
  }

}
