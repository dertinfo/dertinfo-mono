import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  isAnnualSelected: boolean = false;
  contactForm: FormGroup;
  constructor(
    private router: Router,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.contactForm = this.fb.group({
      name: ['', [Validators.required]],
      email: ['', [Validators.email]],
      subject: ['', [Validators.required]],
      message: ['', Validators.required]
    });
  }

  public routeToTerms() {
    this.router.navigate(['./terms']);
  }

  public routeToDataPolicy() {
    this.router.navigate(['./datapolicy']);
  }

  public routeToCookiePolicy() {
    this.router.navigate(['./cookiepolicy']);
  }
}
