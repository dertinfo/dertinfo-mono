import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { NotificationMessageDto } from '../models/NotificationMessageDto';
import { NotificationMessageSubmissionDto } from '../models/NotificationMessageSubmissionDto';
import { Constants } from '../notification-management.config';

@Injectable()
export class NotificationAdminRepository extends RepositoryBase  {

    constructor( http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    public getAllNotificationMessages(): Observable<Array<NotificationMessageDto>> {
        const url = `${this.configurationService.baseApiUrl}/${Constants.messagesUrl}`;

        return this.http.get(url).pipe(
            map((data) => data),
            share(), // We forward on the observable so we should share.
            catchError((err) => this.processApiError(err)));
    }

    public addNotificationMessage(notificationMessage: NotificationMessageSubmissionDto) {
        const url = `${this.configurationService.baseApiUrl}/${Constants.createUrl}`;

        return this.http.post(url, notificationMessage).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    deleteNotificationMessage(notificationMessage: NotificationMessageDto) {
        const url = `${this.configurationService.baseApiUrl}/${Constants.deleteUrl}/${notificationMessage.id}`;

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}
