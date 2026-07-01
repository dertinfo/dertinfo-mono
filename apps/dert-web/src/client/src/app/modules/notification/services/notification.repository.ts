import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { NotificationDetailDto } from '../models/NotificationDetailDto';
import { NotificationSummaryDto } from '../models/NotificationSummaryDto';
import { NotificationThumbnailInfoDto } from '../models/NotificationThumbnailInfoDto';
import { Constants } from '../notification.config';

@Injectable()
export class NotificationRepository extends RepositoryBase {

    constructor( http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    public getCheck(): Observable<NotificationThumbnailInfoDto> {
        const url = `${this.configurationService.baseApiUrl}/${Constants.checkUrl}`;

        return this.http.get(url).pipe(
            map((data) => data),
            share(), // We forward on the observable so we should share.
            catchError((err) => this.processApiError(err)));
    }

    public getSummaries(): Observable<Array<NotificationSummaryDto>> {
        const url = `${this.configurationService.baseApiUrl}/${Constants.summariesUrl}`;

        return this.http.get(url).pipe(
            map((data) => data),
            share(), // We forward on the observable so we should share.
            catchError((err) => this.processApiError(err)));
    }

    public getDetail(id: number): Observable<NotificationDetailDto> {
        const url = `${this.configurationService.baseApiUrl}/${Constants.controllerUrl}/${id}/${Constants.detailsUrl}`;

        return this.http.get(url).pipe(
            map((data) => data),
            share(), // We forward on the observable so we should share.
            catchError((err) => this.processApiError(err)));
    }

    public dismissMessage(id: number) {
        const url = `${this.configurationService.baseApiUrl}/${Constants.controllerUrl}/${id}/${Constants.dismissUrl}`;

        return this.http.put(url, null).pipe(
            map((data) => data),
            share(), // We forward on the observable so we should share.
            catchError((err) => this.processApiError(err)));
    }

    public acknowledgeNotification(id: number) {
        const url = `${this.configurationService.baseApiUrl}/${Constants.controllerUrl}/${id}/${Constants.acknowledgeUrl}`;

        return this.http.put(url, null).pipe(
            map((data) => data),
            share(), // We forward on the observable so we should share.
            catchError((err) => this.processApiError(err)));
    }
}
