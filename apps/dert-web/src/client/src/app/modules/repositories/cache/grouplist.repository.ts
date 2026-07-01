import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { GroupDto } from 'app/models/dto/GroupDto';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

@Injectable()
export class GroupListRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    list(): Observable<GroupDto[]> {
        const url = this.ApiUri + '/group';  // URL to web API
        // this._environmentService.getApiBaseUrl()

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}
