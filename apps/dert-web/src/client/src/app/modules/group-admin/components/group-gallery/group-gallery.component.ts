import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { Observable ,  Subscription } from 'rxjs';
import { GroupAdminConductor } from '../../services/group-admin.conductor';

import { GroupImage } from 'app/models/app';
import { GroupImageDto } from 'app/models/dto';
import { GroupAdminTracker } from '../../services/group-admin.tracker';
import { UploadImageComponent } from '../upload-image/upload-image.component';

@Component({
  selector: 'app-group-gallery',
  templateUrl: './group-gallery.component.html',
  styleUrls: ['./group-gallery.component.css']
})
export class GroupGalleryComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private routeGroupId: number;
  public groupImages$: Observable<GroupImage[]>;

  constructor(
    public composeDialog: MatDialog,
    private router: ActivatedRoute,
    private groupConductor: GroupAdminConductor,
    private _groupTracker: GroupAdminTracker
  ) { }

  ngOnInit() {

    this.groupImages$ = this._groupTracker.groupImages$; // Subscribe Direct uses | async

    const routeParamsSubscription = this.router.parent.params.subscribe(params => {
      this.routeGroupId = parseInt(params['id'], 10);
      this.groupConductor.initImages(this.routeGroupId);
    });

    this._subscriptions.push(routeParamsSubscription);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  openUploadImageDialog() {
    const dialogRef = this.composeDialog.open(UploadImageComponent);
    dialogRef.afterClosed().subscribe(result => {
      this.groupConductor.refreshImages();
    });
  }

  onSetPrimaryClick(groupImage: GroupImage) {
      this.groupConductor.setPrimaryImage(groupImage);
  }

  onDownloadClick(groupImage: GroupImageDto) {
    const originalImageUrl = groupImage.imageResourceUri;
  }

  onDeleteClick(groupImage: GroupImageDto) {
    this.groupConductor.deleteImage(groupImage);
  }

}
