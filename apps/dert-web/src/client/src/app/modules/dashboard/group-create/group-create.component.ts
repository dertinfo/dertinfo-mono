import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { DashboardConductor } from 'app/modules/dashboard/services/dashboard.conductor';

import { GroupMinimalSubmissionDto } from 'app/models/dto';

@Component({
  selector: 'group-create',
  templateUrl: './group-create.component.html'
})
export class GroupCreateComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _groupNamePattern = '^(?!.*\/).*$';

  private groupId;
  private hasUploadChanges = false;

  public groupForm: UntypedFormGroup;

  public isSubmitting = false;

  constructor(
    private composeDialog: MatDialog,
    private dashboardConductor: DashboardConductor,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.groupForm = new UntypedFormGroup({
      groupName: new UntypedFormControl('', [Validators.required, Validators.pattern(this._groupNamePattern)]),
      groupBio: new UntypedFormControl('', [])
    });
  }

  ngOnDestroy() {
  }

  onCreateGroupSubmit() {
    if (this.groupForm.valid) {

      const groupMinimalSubmission: GroupMinimalSubmissionDto = {
        groupName: this.groupForm.value.groupName,
        groupBio: this.groupForm.value.groupBio
      };

      this.isSubmitting = true;
      this.dashboardConductor.addGroup(groupMinimalSubmission).then(
        (savedGroup) => {
          this.isSubmitting = false;
          this.dashboardConductor.pinNewGroup(savedGroup);
          this.composeDialog.closeAll();
          this.router.navigate(['group-configure', savedGroup.groupName, savedGroup.id]);
        },
        (error) => {
          console.error('ERROR: Adding Group Failed');
        }
      );
    }
  }

  onCancel() {
    this.composeDialog.closeAll();
  }
}
