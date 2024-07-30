import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'app/core/authentication/auth.service';
import { UserSettingsDto, UserSettingsUpdateSubmissionDto } from 'app/models/dto';
import { FileUploader } from 'ng2-file-upload';
import { UserAccountConductor } from '../../services/user-account.conductor';

@Component({
  selector: 'app-user-account-settings',
  templateUrl: './user-account-settings.component.html',
  styleUrls: ['./user-account-settings.component.css']
})
export class UserAccountSettingsComponent implements OnInit, OnDestroy {

  private _subscriptions = [];

  private userSettingsForm: UntypedFormGroup;

  public isReady = false;
  public userSettings = null;
  public gdprConsentGained = null;
  public gdprConsentGainedDate = null;

  public userAccountIsDeleting = false;
  public userAccountIsDeleted = false;
  public isActiveConfirmDeleteView = false;

  public myGroups = [];

  constructor(

    private _conductor: UserAccountConductor,
    private _router: Router,
    private _authService: AuthService
  ) { }

  ngOnInit() {

    const settingsSub = this._conductor.userSettings$.subscribe((userSettings: UserSettingsDto) => {

      if (userSettings != null) {
        this.userSettings = userSettings;

        this.userSettingsForm = new UntypedFormGroup({
          firstName: new UntypedFormControl(this.userSettings.firstName, [Validators.required]),
          lastName: new UntypedFormControl(this.userSettings.lastName, [Validators.required]),
          telephone: new UntypedFormControl(this.userSettings.telephone, [Validators.required]),
          email: new UntypedFormControl({ value: this.userSettings.email, disabled: true }),
        });

        this.gdprConsentGained = userSettings.gdprConsentGained;
        this.gdprConsentGainedDate = userSettings.gdprConsentGainedDate;

        this.isReady = true;
      }
    });
    this._subscriptions.push(settingsSub);

    this._conductor.initSettings();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onUserSettingsSubmit() {

    if (this.userSettingsForm.valid) {

      // Update the visible entities
      // this.overview.userName = this.userSettingsForm.value.firstName + this.userSettingsForm.value.lastName;

      const userSettingsUpdate: UserSettingsUpdateSubmissionDto = {
        firstName: this.userSettingsForm.value.firstName,
        lastName: this.userSettingsForm.value.lastName,
        telephone: this.userSettingsForm.value.telephone,
      };

      this._conductor.updateSettings(userSettingsUpdate);
    }
  }

  public onUserDeleteClick() {
    this.isActiveConfirmDeleteView = true;
    this.userAccountIsDeleting = true;
  }

  public onUserConfirmDeleteClick() {
    this._conductor.removeAccount().subscribe(() => {
      this.userAccountIsDeleted = true;

      // Insurance - to ensure the clear down has completed after the delete.
      setTimeout(() => {
        this._authService.logout();
      }, 2000); // note - this matches with the redirect after delete.

    });
  }

}
