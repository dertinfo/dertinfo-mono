import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { DodRecoverSessionDto } from 'app/models/dto/DodRecoverSessionDto';
import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { Conductor } from '../../services/dertofderts-public.conductor';
import { Tracker } from '../../services/dertofderts-public.tracker';

@Component({
  selector: 'app-recoversessiondialog',
  templateUrl: './recoversessiondialog.component.html',
  styleUrls: ['./recoversessiondialog.component.scss']
})
export class RecoverSessionDialogComponent implements OnInit {

  public form: FormGroup;
  public dataRecoverySucceeded: boolean = false;
  public dataRecoveryFailedMismatch: boolean = false;
  public dataRecoveryFailedExpired: boolean = false;
  public dataUserName: string;

  constructor(
    private _conductor: Conductor,
    private _tracker: Tracker,
    public dialogRef: MatDialogRef<RecoverSessionDialogComponent>,
  ) { }

  ngOnInit() {
    this.prepareForm();
  }

  public submitForm() {
    if (this.form.valid) {
      const subs = this._conductor.recoverSession(this.form.value.email).subscribe(
        (recoveryData) => {
          subs.unsubscribe();
          this.recoverySucceeded(recoveryData);
        },
        (err) => this.recoveryFailed(err),
        () => { }
      );
    }
  }

  public onDialogCloseClick() {
    if (this.form.value.dontAskAgain) {
      // Clear the session storage
      this._conductor.clearSession();
    }

    this.dialogRef.close();

  }

  private reset() {
    this.dataRecoverySucceeded = false;
    this.dataRecoveryFailedMismatch = false;
    this.dataUserName = '';
    this.form.reset();
  }

  private recoverySucceeded(dodRecoverSessionDto: DodRecoverSessionDto) {
    this.dataRecoverySucceeded = true;
    this.dataUserName = dodRecoverSessionDto.userName;

  }

  private recoveryFailed(err: any) {

    const status: number = +err.status;

    switch (status) {
      case 500: this.dataRecoveryFailedMismatch = true; break;
      case 403: this.dataRecoveryFailedExpired = true; break;
      default: this.dataRecoveryFailedMismatch = true;
    }
  }

  private prepareForm() {

    this.form = new FormGroup({
      email: new FormControl('', [Validators.required, customEmailValidator()]),
      dontAskAgain: new FormControl(false, [])
    });

  }

}
