import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserOverviewDto, UserSettingsDto } from 'app/models/dto';
import { UserRepository } from 'app/modules/repositories';
import { BehaviorSubject } from 'rxjs';

import { AuthService } from 'app/core/authentication/auth.service';

@Injectable()
export class UserAccountConductor {

  private _overviewLoading: boolean = false;
  private _snackBarDuration: number = 1200;

  private memoryStore: {
    overview: UserOverviewDto,
    settings: UserSettingsDto,
  };

  private _userOverview: BehaviorSubject<UserOverviewDto> = new BehaviorSubject<UserOverviewDto>(null);
  get userOverview$() { return this._userOverview.asObservable(); }

  private _userSettings: BehaviorSubject<UserSettingsDto> = new BehaviorSubject<UserSettingsDto>(null);
  get userSettings$() { return this._userSettings.asObservable(); }

  constructor(

    private _authService: AuthService,
    private userRepo: UserRepository,
    private snackBar: MatSnackBar
  ) {

    this.reset();
  }

  public reset() {
    this.memoryStore = {
      overview: null,
      settings: null,
    };

    this._userOverview.next(null);
    this._userSettings.next(null);
  }

  public initOverview() {

    if (!this.memoryStore.overview) {

      this._overviewLoading = true;

      this.userRepo.overview().subscribe((userOverview) => {
        this.memoryStore.overview = { ...userOverview };
        this._overviewLoading = false;
        this._userOverview.next(Object.assign({}, this.memoryStore).overview);

        this.memoryStore.settings = {
          email: userOverview.email,
          firstName: userOverview.firstName,
          lastName: userOverview.lastName,
          telephone: userOverview.telephone,
          gdprConsentGained: userOverview.gdprConsentGained,
          gdprConsentGainedDate: userOverview.gdprConsentGainedDate,
        };

        // All the information for settings is contained within overview. Therefore just populate.
        this._userSettings.next(Object.assign({}, this.memoryStore).settings);
      });

      this.memoryStore.settings = null;
    }
  }

  public initSettings() {

    if (!this.memoryStore.settings) {
      this.initOverview();
    }
  }

  public updateSettings(userSettingsUpdateSubmission) {

    this.memoryStore.settings.firstName = userSettingsUpdateSubmission.firstName;
    this.memoryStore.settings.lastName = userSettingsUpdateSubmission.lastName;
    this.memoryStore.settings.telephone = userSettingsUpdateSubmission.telephone;

    this.userRepo.updateUserSettings(userSettingsUpdateSubmission).subscribe(() => {

      this._userSettings.next({ ...this.memoryStore.settings });
      this.snackBar.open('User Settings Updated', 'close', { duration: this._snackBarDuration });

      this._authService.updateUserDetails(userSettingsUpdateSubmission);
    });
  }

  public removeAccount() {

    const obs = this.userRepo.deleteAccount();

    const subs = obs.subscribe(() => {
      subs.unsubscribe();
      this.snackBar.open('User Deleted', 'close', { duration: this._snackBarDuration });

      // Insurance - to ensure the clear down has completed after the delete.
      setTimeout(() => {
        this.memoryStore = {
          overview: null,
          settings: null,
        };
      }, 2000); // note - this matches with the redirect after delete.
    });

    return obs;
  }
}
