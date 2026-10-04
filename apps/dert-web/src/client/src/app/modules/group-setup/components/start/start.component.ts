import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { GroupConfigurationSubmissionDto, GroupDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { GroupSetupConductor } from '../../services/group-setup.conductor';
import { GroupSetupTracker } from '../../services/group-setup.tracker';
import { UploadImageComponent } from '../upload-image/upload-image.component';

import { AppConstants } from 'app/app.constants';
import { GroupConfigureTermsComponent } from 'app/regions/terms/components/group-configure-terms/group-configure-terms.component';

@Component({
  selector: 'app-start',
  templateUrl: './start.component.html',
  styleUrls: ['./start.component.css']
})
export class StartComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  contactDetailsFormGroup: UntypedFormGroup;
  visabilityFormGroup: UntypedFormGroup;
  groupDetailsFormGroup: UntypedFormGroup;
  groupImageFormGroup: UntypedFormGroup;
  configurationTemplateFormGroup: UntypedFormGroup;
  termsAndConditionsFormGroup: UntypedFormGroup;

  public groupName: string;
  public groupPictureUrl: string;
  public selectedVisibilityOption = 'public';
  public visibilityOptions = [
    {
      name: 'Public',
      value: 'public',
      id: 0
    },
    {
      name: 'Restricted',
      value: 'restricted',
      id: 1
    },
    {
      name: 'Private',
      value: 'private',
      id: 2
    }
  ];

  constructor(

    private fb: UntypedFormBuilder,
    private _conductor: GroupSetupConductor,
    private _tracker: GroupSetupTracker,
    public composeDialog: MatDialog,
    private router: Router
  ) { }

  ngOnInit() {

    this.contactDetailsFormGroup = this.fb.group({
      contactName: ['', Validators.required],
      contactEmail: ['', Validators.required],
      contactTelephone: ['', Validators.required]
    });
    this.visabilityFormGroup = this.fb.group({
      visibility: [0, Validators.required]
    });
    this.groupDetailsFormGroup = this.fb.group({
      groupBio: [''],
      originTown: [''],
      originPostcode: ['']
    });
    this.groupImageFormGroup = this.fb.group({
      groupPictureUrl: ['', Validators.required]
    });
    this.configurationTemplateFormGroup = this.fb.group({
      groupTemplate: [0, Validators.required]
    });
    this.termsAndConditionsFormGroup = this.fb.group({
      agreeToTermsAndConditions: [false, Validators.required]
    });

    this._subscriptions.push(this._tracker.groupOverview$.subscribe((groupOverview) => {
      if (groupOverview) {

        this.groupName = groupOverview.groupName;

        this.groupDetailsFormGroup.setValue(
          {
            originTown: groupOverview.originTown,
            originPostcode: groupOverview.originPostcode,
            groupBio: groupOverview.groupBio
          }
        );

        this.groupImageFormGroup.setValue(
          {
            groupPictureUrl: groupOverview.groupPictureUrl
          }
        );
        this.groupPictureUrl = groupOverview.groupPictureUrl;
      }
    }));

    this._subscriptions.push(this._tracker.userData$.subscribe((userData) => {
      if (userData) {

        let userFullName = '';
        if (userData.lastname) { userFullName = userData.lastname.trim(); }
        if (userData.firstname) { userFullName = userData.firstname.trim(); }
        if (userData.firstname && userData.lastname) { userFullName = (userData.firstname + ' ' + userData.lastname).trim(); }

        this.contactDetailsFormGroup.setValue(
          {
            contactName: userFullName,
            contactEmail: userData.email || '',
            contactTelephone: userData.phone || '',
          }
        );
      }
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  submit() {

    const groupConfigurationSubmissionDto: GroupConfigurationSubmissionDto = {
      contactName: this.contactDetailsFormGroup.value.contactName,
      contactEmail: this.contactDetailsFormGroup.value.contactEmail,
      contactTelephone: this.contactDetailsFormGroup.value.contactTelephone,
      groupBio: this.groupDetailsFormGroup.value.groupBio,
      originTown: this.groupDetailsFormGroup.value.originTown,
      originPostcode: this.groupDetailsFormGroup.value.originPostcode,
      visibilityType: this.visabilityFormGroup.value.visibility,
      agreeToTermsAndConditions: this.termsAndConditionsFormGroup.value.agreeToTermsAndConditions,
    };

    this._conductor.submitConfiguration(groupConfigurationSubmissionDto).subscribe((groupDto: GroupDto) => {
      this._conductor.clear();
      // this.router.navigate(['group', escape(groupDto.name), groupDto.id]);
      this.router.navigate(['group', groupDto.groupName, groupDto.id]);
    });
  }

  onStepperStepChange($event) {

    this._conductor.startStepperStepChanged($event);
  }

  onTermsAndConditionsClick() {

    const dialogRef = this.composeDialog.open(GroupConfigureTermsComponent, AppConstants.largeMatDialogConfig);
    dialogRef.afterClosed().subscribe(result => {

    });

  }

  openUploadImageDialog() {
    const dialogRef = this.composeDialog.open(UploadImageComponent);
    dialogRef.afterClosed().subscribe(result => {
      this._conductor.newImageUploaded();
    });
  }
}
