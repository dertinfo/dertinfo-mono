import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ClientSettingsDto } from 'app/models/dto/ClientSettingsDto';

import { ConfigurationService } from 'app/core/services/configuration.service';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

/**
 * Note that currently we only load the settings when entering the dert of derts
 * functionality in order to prevent api hits on the homepage
 */
@Injectable()
export class ClientSettingRepository extends RepositoryBase {

    constructor(
        http: HttpClient,
        configurationService: ConfigurationService,
        appInsightsService: AppInsightsService
    ) {
        super(http, configurationService, appInsightsService);
    }

    public getAll(): Observable<ClientSettingsDto> {
        const url = this.ApiUri + `/clientsettings`;  // URL to web API

        return this.http.get(url).pipe(
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}
