import { Component, OnInit } from '@angular/core';
import { AuthService } from 'app/core/authentication/auth.service';

/**
 * Auth flow step 4 — Auth0 redirects here after Universal Login ({callback}/callback).
 * The SDK exchanges ?code=&state= for tokens; AuthService.handleAuthentication waits for
 * isAuthenticated$ then navigates to /dashboard (or appState.target).
 */
@Component({
    selector: 'app-auth-callback',
    templateUrl: './auth-callback.component.html'
})
export class AuthCallbackComponent implements OnInit {

    constructor(public auth: AuthService) {}

    ngOnInit() {
        this.auth.handleAuthentication();
        // Cache access token for upload widgets that cannot use AuthHttpInterceptor.
        this.auth.ensureAccessTokenCached().catch((err) => {
            console.warn('Could not cache access token on callback', err);
        });
    }
}
