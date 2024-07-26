import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatProgressBar } from '@angular/material/progress-bar';
import { AuthService } from '../../../../core/authentication/auth.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SigninComponent implements OnInit {
  @ViewChild(MatProgressBar, { static: false }) progressBar: MatProgressBar;
  @ViewChild(MatButton, { static: false }) submitButton: MatButton;

  signinForm: FormGroup;

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.authService.login();

    // this.signinForm = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', Validators.required),
    //   rememberMe: new FormControl(false)
    // })
  }

  // signin() {
  //   const signinData = this.signinForm.value;

  //   this.submitButton.disabled = true;
  //   this.progressBar.mode = 'indeterminate';
  // }

}
