import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SettingsRepository } from '../services/settings.repository';

@Component({
  selector: 'app-dod-admin-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  private _snackBarDuration: number = 1200;

  public openToPublic = false;
  public resultsPublished = false;
  public publicResultsForwarded = false;
  public officialResultsForwarded = false;
  public validJudgePasswords = '';
  public isLoaded = false;

  constructor(
    private _settingRepository: SettingsRepository,
    private _snackBar: MatSnackBar
  ) {

  }

  ngOnInit() {
    const subs = this._settingRepository.getSettings().subscribe((settings) => {
      subs.unsubscribe();
      this.openToPublic = settings.openToPublic;
      this.resultsPublished = settings.resultsPublished;
      this.publicResultsForwarded = settings.publicResultsForwarded;
      this.officialResultsForwarded = settings.officialResultsForwarded;
      this.validJudgePasswords = settings.validJudgePasswords;
      this.isLoaded = true;
    });
  }

  public onValidJudgePasswordsSubmit() {
    const subs = this._settingRepository.updateValidJudgePasswords(this.validJudgePasswords).subscribe((success) => {
      subs.unsubscribe();
      this._snackBar.open('Judge Passwords Updated', 'close', { duration: this._snackBarDuration });
    });
  }

  public onResultsPublishedChange() {
    const subs = this._settingRepository.updateResultsPublished(this.resultsPublished).subscribe((success) => {
      subs.unsubscribe();
      this._snackBar.open('Results Published Updated', 'close', { duration: this._snackBarDuration });
    });
  }

  public onPublicResultsForwardedChange() {
    const subs = this._settingRepository.updatePublicResultsForwarded(this.publicResultsForwarded).subscribe((success) => {
      subs.unsubscribe();
      this._snackBar.open('Public Results Forwarded Updated', 'close', { duration: this._snackBarDuration });
    });
  }

  public onOfficialResultsForwardedChange() {
    const subs = this._settingRepository.updateOfficialResultsForwarded(this.officialResultsForwarded).subscribe((success) => {
      subs.unsubscribe();
      this._snackBar.open('Official Results Forwarded Updated', 'close', { duration: this._snackBarDuration });
    });
  }

  public onOpenToPublicChange() {
    const subs = this._settingRepository.updateOpenToPublic(this.openToPublic).subscribe((success) => {
      subs.unsubscribe();
      this._snackBar.open('Open To Public Updated', 'close', { duration: this._snackBarDuration });
    });
  }

  public onClearCacheClick() {
    const subs = this._settingRepository.clearResultsCache().subscribe((success) => {
      subs.unsubscribe();
      this._snackBar.open('Cache Cleared', 'close', { duration: this._snackBarDuration });
    });
  }

}
