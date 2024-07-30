
import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { GroupOverviewDto, GroupOverviewUpdateDto } from 'app/models/dto';
import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { Subscription } from 'rxjs';
import { GroupAdminConductor } from '../../services/group-admin.conductor';
import { GroupAdminTracker } from '../../services/group-admin.tracker';

@Component({
  selector: 'app-group-settings',
  templateUrl: './group-settings.component.html',
  styleUrls: ['./group-settings.component.css']
})
export class GroupSettingsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _groupNamePattern = '^(?!.*\/).*$';

  private groupOverview: GroupOverviewDto;
  private groupUpdateForm: UntypedFormGroup;
  private groupPrivacySettingsForm: UntypedFormGroup;

  public _isReady = false;
  public groupDeleted: boolean = false;

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

    private groupConductor: GroupAdminConductor,
    private _groupTracker: GroupAdminTracker
  ) { }
  ngOnInit() {

    const groupOverviewSubscription = this._groupTracker.groupOverview$.subscribe((groupOverview) => {
      if (groupOverview) {

        if (groupOverview.isDeleting && groupOverview.isDeleted) {
          setTimeout(() => {
            this.groupDeleted = true;
          }, 2000);
        } else {
          this.groupOverview = groupOverview;

          this.groupUpdateForm = new UntypedFormGroup({
            groupName: new UntypedFormControl(this.groupOverview.groupName, [
              Validators.required,
              Validators.pattern(this._groupNamePattern)
            ]),
            groupEmail: new UntypedFormControl(this.groupOverview.groupEmail, [
              Validators.required,
              customEmailValidator()
            ]),
            contactTelephone: new UntypedFormControl(this.groupOverview.contactTelephone, [Validators.required]),
            contactName: new UntypedFormControl(this.groupOverview.contactName, [Validators.required]),
            groupBio: new UntypedFormControl(this.groupOverview.groupBio, []),
            originTown: new UntypedFormControl(this.groupOverview.originTown, []),
            originPostcode: new UntypedFormControl(this.groupOverview.originPostcode, [])
          });

          this.groupPrivacySettingsForm = new UntypedFormGroup({
            visibility: new UntypedFormControl(this.groupOverview.visibility, [Validators.required]),
          });
        }

        this._isReady = true;
      }
    });

    // const parentRouteParamsSubscription = this.activatedRoute.parent.params.subscribe(params => {
    //   this.groupConductor.initOverview(params['id']);
    // });

    this._subscriptions.push(groupOverviewSubscription/*, parentRouteParamsSubscription*/);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  onGroupUpdateSubmit() {

    if (this.groupUpdateForm.valid) {

      // Update the visible entities
      this.groupOverview.groupName = this.groupUpdateForm.value.groupName;

      const groupOverviewUpdate: GroupOverviewUpdateDto = {
        groupId: this.groupOverview.id,
        groupName: this.groupUpdateForm.value.groupName,
        groupEmail: this.groupUpdateForm.value.groupEmail,
        contactTelephone: this.groupUpdateForm.value.contactTelephone,
        contactName: this.groupUpdateForm.value.contactName,
        groupBio: this.groupUpdateForm.value.groupBio,
        originTown: this.groupUpdateForm.value.originTown,
        originPostcode: this.groupUpdateForm.value.originPostcode,
        visibility: this.groupOverview.visibility
      };

      this.groupConductor.updateGroupOverview(groupOverviewUpdate);
    }
  }

  onGroupPrivacySubmit() {

    if (this.groupPrivacySettingsForm.valid) {
      // Update the visible entities
      this.groupOverview.groupName = this.groupUpdateForm.value.groupName;

      const groupOverviewUpdate: GroupOverviewUpdateDto = {
        groupId: this.groupOverview.id,
        groupName: this.groupOverview.groupName,
        groupEmail: this.groupOverview.groupEmail,
        contactTelephone: this.groupOverview.contactTelephone,
        contactName: this.groupOverview.contactName,
        groupBio: this.groupOverview.groupBio,
        originTown: this.groupOverview.originTown,
        originPostcode: this.groupOverview.originPostcode,
        visibility: this.groupPrivacySettingsForm.value.visibility
      };

      this.groupConductor.updateGroupOverview(groupOverviewUpdate);
    }
  }

  onDeleteGroupConfirmed() {
    this.groupConductor.removeGroup();
  }
}
