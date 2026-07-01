import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {catchError, map} from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { EmailPreviewDto, EmailRegistrationConfirmationDataDto } from 'app/models/dto';

@Injectable()
export class RegistrationByEventsRepository extends RepositoryBase  {

    constructor( http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    previewRegistrationConfirmation(registrationConfirmationEmailData: EmailRegistrationConfirmationDataDto) {
        const url = this.ApiUri + '/sendemail/preview-group-registration-confirmation';  // URL to web API

        return this.http.post<EmailPreviewDto>(url, registrationConfirmationEmailData).pipe(
            map((data) => data.htmlBody),
            catchError((err) => this.processApiError(err)));
    }
}
