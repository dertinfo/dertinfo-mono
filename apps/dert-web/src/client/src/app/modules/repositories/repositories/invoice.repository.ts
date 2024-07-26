
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { EventInvoiceDto, InvoiceDto } from 'app/models/dto';
import { ConfigurationService } from 'app/core/services/configuration.service';

@Injectable()
export class InvoiceRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    invoicesByEvent(eventId: number): Observable<EventInvoiceDto[]> {
        const url = this.ApiUri + '/invoice/event/' + eventId.toString();  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    invoiceHistory(invoiceId: number): Observable<InvoiceDto[]> {
        const url = this.ApiUri + '/invoice/' + invoiceId.toString() + '/history';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }
}
