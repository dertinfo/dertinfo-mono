import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { EventImageSubmissionDto } from 'app/models/dto';
import { AuthService } from 'app/core/authentication/auth.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { FileItem, FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
  selector: 'upload-image',
  templateUrl: './upload-image.template.html'
})
export class UploadImageComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private eventId;
  private hasUploadChanges = false;

  public uploader: FileUploader;
  public hasBaseDropZoneOver: boolean = false;

  constructor(
    private composeDialog: MatDialog,
    private authService: AuthService,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker,
    private configurationService: ConfigurationService
  ) {
  }

  ngOnInit() {

    this._subscriptions.push(this._eventTracker.eventOverview$.subscribe((eventOverview) => {
      if (eventOverview) {
        this.eventId = eventOverview.id;
        this.initUploader();
      }
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  initUploader() {
    const baseApiUrl = this.configurationService.baseApiUrl;
    const url = `${baseApiUrl}/event/${this.eventId}/image`;
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

  private formatImageUploadData(data: FileItem): Promise<EventImageSubmissionDto> {
    return new Promise((resolve, reject) => {
      const fileReader = new FileReader();
      const file = data._file;
      const fileName = data._file.name.toString();
      const fileType = data._file.type.toString();
      const byteArrayPrefix: string = 'data:' + fileType + ';base64,';

      fileReader.readAsDataURL(file);
      fileReader.onload = () => {
        const imageData = fileReader.result.toString();

        const eventImageSubmission: EventImageSubmissionDto = {
          eventId: this.eventId,
          base64StringImage: imageData.substring(byteArrayPrefix.length),
          uploadImageExtension: fileName.substring(fileName.lastIndexOf('.'))
        };

        resolve(eventImageSubmission);
      };
    });
  }


}
