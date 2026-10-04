import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from 'app/core/authentication/auth.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { GroupImageSubmissionDto } from 'app/models/dto';
import { FileItem, FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { Subscription } from 'rxjs';
import { GroupSetupConductor } from '../../services/group-setup.conductor';
import { GroupSetupTracker } from '../../services/group-setup.tracker';

@Component({
  selector: 'upload-image',
  templateUrl: './upload-image.component.html'
})
export class UploadImageComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private groupId;
  private hasUploadChanges = false;

  public uploader: FileUploader;
  public hasBaseDropZoneOver: boolean = false;

  constructor(
    private composeDialog: MatDialog,
    private authService: AuthService,
    private _conductor: GroupSetupConductor,
    private _tracker: GroupSetupTracker,
    private configurationService: ConfigurationService
  ) {
  }

  ngOnInit() {

    const groupOverviewSubscription = this._tracker.groupOverview$.subscribe((groupOverview) => {
      if (groupOverview) {
        this.groupId = groupOverview.id;
        this.initUploader();
      }
    });

    this._subscriptions.push(groupOverviewSubscription);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  initUploader() {
    const baseApiUrl = this.configurationService.baseApiUrl;
    const url = `${baseApiUrl}/group/${this.groupId}/image`;
    this.uploader = new FileUploader(
      {
        url: url,
        authTokenHeader: 'Authorization',
        authToken: 'Bearer ' + this.authService.accessToken(),
        formatDataFunction: async (data: FileItem) => this.formatImageUploadData(data),
        formatDataFunctionIsAsync: true,
        disableMultipart: true,
        allowedMimeType: ['image/png', 'image/jpeg'],
        headers: [
          {
            name: 'Content-Type',
            value: 'application/json'
          }]
      }
    );

    this.uploader.onAfterAddingFile = (item => {
      item.withCredentials = false;
    });

    this.uploader.onCompleteItem = (item) => {
      this.hasUploadChanges = true;
    };

    this.uploader.onCompleteAll = () => {
      let canClose = true;
      this.uploader.queue.forEach(fileItem => {
        canClose = fileItem.isUploaded;
      });
      if (canClose) {
        this.composeDialog.closeAll();
      }
    };
  }

  public onUploadAllClick() {
    this.uploader.uploadAll();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  public onCancel() {
    this.composeDialog.closeAll();
  }

  private formatImageUploadData(data: FileItem): Promise<GroupImageSubmissionDto> {
    return new Promise((resolve, reject) => {
      const fileReader = new FileReader();
      const file = data._file;
      const fileName = data._file.name.toString();
      const fileType = data._file.type.toString();
      const byteArrayPrefix: string = 'data:' + fileType + ';base64,';

      fileReader.readAsDataURL(file);
      fileReader.onload = () => {
        const imageData = fileReader.result;

        const groupImageSubmission: GroupImageSubmissionDto = {
          groupId: this.groupId,
          base64StringImage: imageData.toString().substring(byteArrayPrefix.length),
          uploadImageExtension: fileName.substring(fileName.lastIndexOf('.'))
        };

        resolve(groupImageSubmission);
      };
    });
  }
}
