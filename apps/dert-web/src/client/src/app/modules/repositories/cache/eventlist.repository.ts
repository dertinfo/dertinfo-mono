import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { EventDto } from 'app/models/dto';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

@Injectable()
export class EventListRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    list(): Observable<EventDto[]> {
        const url = this.ApiUri + '/event/web';  // URL to web API
        // this._environmentService.getApiBaseUrl()

        return this.http.get(url).pipe(
            share(),
            map((data) => this.eventDtosMap(data)),
            catchError((err) => this.processApiError(err)));
    }

    private eventDtosMap(data: any) {
        const eventDtos: EventDto[] = data;

        return eventDtos;
    }
}
