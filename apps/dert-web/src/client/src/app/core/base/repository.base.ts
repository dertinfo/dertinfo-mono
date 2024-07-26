import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';

export class RepositoryBase {

    constructor(
        protected http: HttpClient,
        protected configurationService: ConfigurationService,
        protected appInsightsService: AppInsightsService
    ) { }

    protected get ApiUri() {
        return this.configurationService.baseApiUrl;
    }

    protected processApiError(err: any): Observable<any> {
        // if (err && err.error instanceof Error) {
        //     // A client-side or network error occurred. Handle it accordingly.
        //     this.appInsightsService.logException(err.error);
        // } else {

        //     // The backend returned an unsuccessful response code.
        // }

        return throwError(err.json().error || 'Server error');
    }
}
