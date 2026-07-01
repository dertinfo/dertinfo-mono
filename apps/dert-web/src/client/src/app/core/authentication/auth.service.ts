import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { UserSettingsUpdateSubmissionDto } from 'app/models/dto';
import * as auth0 from 'auth0-js';
import { Subject } from 'rxjs';
import { UserData } from '../../models/auth/userdata.model';
import { ConfigurationService } from '../services/configuration.service';

@Injectable({ providedIn: 'root' })
export class AuthService {

  /**
   * This subject is used for the side nav to be able to identify when a user has changed the settings so
   * that it can update the name displayed.
   */
  private userDataChangedSubject: Subject<UserData> = new Subject<UserData>();
  private _autoRefreshTokenAt = 60000;

  public auth0 = null;
  public auth0Silent = null;

  public get userDataChanged$() {
    return this.userDataChangedSubject.asObservable();
  }

  constructor(public router: Router, private _configurationService: ConfigurationService) {

    const auth0ClientId = _configurationService.auth0ClientId;
    const auth0TenantDomain = _configurationService.auth0TenantDomain;
    const auth0CallbackUrl = _configurationService.auth0CallbackUrl;
    const auth0Audience = _configurationService.auth0Audience;
    const callbackUrl = _configurationService.auth0CallbackUrl;

    this.auth0 = new auth0.WebAuth({
      clientID: auth0ClientId,
      domain: auth0TenantDomain,
      responseType: 'token id_token',
      audience: auth0Audience,
      redirectUri: `${auth0CallbackUrl}/callback`,
      scope: 'openid profile email offline_access'
    });

    this.auth0Silent = new auth0.WebAuth({
      clientID: auth0ClientId,
      domain: auth0TenantDomain,
      scope: 'openid profile email offline_access',
      responseType: 'token id_token',
      redirectUri: `${callbackUrl}`
    });

  }

  public login(): void {
    this.auth0.authorize();
  }

  public handleAuthentication(): void {
    this.auth0.parseHash((err, authResult) => {
      if (authResult && authResult.accessToken && authResult.idToken) {
        // window.location.hash = ''; //note - causes error in app. Does not redirect if set.
        this.setSession(authResult);
        this.router.navigate(['/dashboard']);
      } else if (err) {
        this.router.navigate(['/home']);
        console.log(err);
        alert(`Error: ${err.error}. Check the console for further details.`);
      }
    });
  }

  public updateUserDetails(updatedSettings: UserSettingsUpdateSubmissionDto) {
    const user_data: UserData = JSON.parse(localStorage.getItem('user_data'));

    user_data.firstname = updatedSettings.firstName;
    user_data.lastname = updatedSettings.lastName;

    // Update the details in the local storage
    localStorage.setItem('user_data', JSON.stringify(user_data));

    // Notify that there has been a change so that the side navigation can update
    this.userDataChangedSubject.next(user_data);
  }

  public logout(): void {
    // Remove tokens and expiry time from localStorage
    localStorage.removeItem('access_token');
    localStorage.removeItem('id_token');
    localStorage.removeItem('expires_at');
    localStorage.removeItem('user_data');

    this.auth0.logout();

    // Go back to the home route
    this.router.navigate(['/']);
  }

  public accessToken(): string {
    // Check whether the current time is past the
    // access token's expiry time
    const accessToken = localStorage.getItem('access_token');
    return accessToken;
  }

  public isAuthenticated(): boolean {
    // Check whether the current time is past the
    // access token's expiry time
    const expiresAt = JSON.parse(localStorage.getItem('expires_at'));

    if (expiresAt !== null) {
      const millisecondsToExpiry = expiresAt - new Date().getTime();

      if (millisecondsToExpiry < this._autoRefreshTokenAt) {
        console.log('***Auto Refreshing Token***');
        this.renewToken();
      }

      return new Date().getTime() < expiresAt;
    }

    return false;
  }

  public userData(): UserData {
    const user_data = JSON.parse(localStorage.getItem('user_data'));
    return user_data;
  }

  public renewToken(): Promise<void> {

    const callbackUrl = this._configurationService.auth0CallbackUrl;
    const auth0Audience = this._configurationService.auth0Audience;

    return new Promise<void>((resolve, reject) => {
      console.log('AuthService - renewToken');
      this.auth0.renewAuth({
        audience: auth0Audience,
        redirectUri: `${callbackUrl}/silent`,
        usePostMessage: true,
        postMessageOrigin: `${callbackUrl}`
      }, (err, result) => {
        if (err) {
          console.error(err);
          reject(err);
        } else {
          console.log(result);
          this.setSession(result);
          resolve();
        }
      });
    });

  }

  public parseSilentResponse() {

    const callbackUrl = this._configurationService.auth0CallbackUrl;

    this.auth0Silent.parseHash((err, response) => {
      parent.postMessage(err || response, `${callbackUrl}`);
    });
  }

  public addGdprConsent() {
    const user_data: UserData = JSON.parse(localStorage.getItem('user_data'));
    user_data.gdprConsentGained = true;
    localStorage.setItem('user_data', JSON.stringify(user_data));
  }

  private setSession(authResult): void {
    // Set the time that the access token will expire at
    const expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
    localStorage.setItem('access_token', authResult.accessToken);
    localStorage.setItem('id_token', authResult.idToken);
    localStorage.setItem('expires_at', expiresAt);

    if (authResult.idTokenPayload) {
      const user_data: UserData = {
        email: authResult.idTokenPayload['email'],
        name: authResult.idTokenPayload['name'],
        nickname: authResult.idTokenPayload['nickname'],
        picture: authResult.idTokenPayload['picture'],
        firstname: authResult.idTokenPayload['https://dertinfo.co.uk/firstname'],
        lastname: authResult.idTokenPayload['https://dertinfo.co.uk/lastname'],
        phone: authResult.idTokenPayload['https://dertinfo.co.uk/phone'],
        gdprConsentGained: authResult.idTokenPayload['https://dertinfo.co.uk/gdprconsentgained'],
        dertOfDertsAdmin: authResult.idTokenPayload['https://dertinfo.co.uk/dodadmin'],
        superAdmin: authResult.idTokenPayload['https://dertinfo.co.uk/superadmin']
      };

      localStorage.setItem('user_data', JSON.stringify(user_data));

      // Populate the subject so the side nav can get the data
      this.userDataChangedSubject.next(user_data);
    }
  }
}
