import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandler, Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { AppInsightsService } from './appinsights.service';

@Injectable({ providedIn: 'root' })
export class ErrorHandlerService extends ErrorHandler {

    constructor(private appInsightsService: AppInsightsService, private router: Router) {
        super();
    }

    handleError(error: Error) {
        this.appInsightsService.logException(error); // Manually log exception

        // Always handle Http errors differently
        if (error instanceof HttpErrorResponse) {
            const err = error as HttpErrorResponse;
            switch (err.status) {
                case 401:
                    this.router.navigate(['/session/401']);
                    break;
                case 403:
                    this.router.navigate(['/session/403']);
                    break;
                case 404:
                    this.router.navigate(['/session/404']);
                    break;
                default:
                    this.router.navigate(['/session/error']);
                    break;
            }
        }
    }
}
