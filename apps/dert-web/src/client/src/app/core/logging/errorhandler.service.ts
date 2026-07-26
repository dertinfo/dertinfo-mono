import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from '../authentication/auth.service';
import { AppInsightsService } from './appinsights.service';

@Injectable({ providedIn: 'root' })
export class ErrorHandlerService extends ErrorHandler {

    constructor(
        private appInsightsService: AppInsightsService,
        private router: Router,
        private injector: Injector,
    ) {
        super();
    }

    handleError(error: Error) {
        this.appInsightsService.logException(error); // Manually log exception

        // During Auth0 logout, ignore HTTP failures so we do not flash /session/404 before returnTo.
        const authService = this.injector.get(AuthService, null);
        if (authService?.isLoggingOut) {
            return;
        }

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
