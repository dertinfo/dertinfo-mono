import { Location } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { GroupMemberDetailDto, GroupMemberUpdateDto } from 'app/models/dto';
import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { Subscription } from 'rxjs';
import { GroupAdminConductor } from '../../services/group-admin.conductor';
import { GroupAdminTracker } from '../../services/group-admin.tracker';

@Component({
  selector: 'app-group-members-detail',
  templateUrl: './group-members-detail.component.html',
  styleUrls: ['./group-members-detail.component.css']
})
export class GroupMembersDetailComponent implements OnInit, OnDestroy {
  private _subscriptions: Subscription[] = [];

  private groupMember: GroupMemberDetailDto;
  private memberUpdateForm: FormGroup;

  private rows = [];

  public _isReady = false;

  constructor(
    private activatedRoute: ActivatedRoute,
    private groupConductor: GroupAdminConductor,
    private _groupTracker: GroupAdminTracker,
    private _location: Location
  ) { }

  ngOnInit() {

    const focussedMemberSubscription = this._groupTracker.focussedGroupMember$.subscribe((groupMemberDetail) => {
      if (groupMemberDetail) {
        this.groupMember = groupMemberDetail;
        this.rows = this.groupMember.memberAttendances;

        this.memberUpdateForm = new FormGroup({
          name: new FormControl(this.groupMember.name, [
            Validators.required
          ]),
          email: new FormControl(this.groupMember.emailAddress, [
            customEmailValidator()
          ]),
          telephone: new FormControl(this.groupMember.telephoneNumber, []),
          dateOfBirth: new FormControl(this.groupMember.dateOfBirth, []),
          dateJoined: new FormControl(this.groupMember.dateJoined, []),
          memberType: new FormControl(this.groupMember.memberType, [])
        });

        this._isReady = true;
      }
    });

    const parentRouteParamsSubscription = this.activatedRoute.parent.params.subscribe(params => {
      this.groupConductor.initMembers(params['id']);
    });

    const routeParamsSubscription = this.activatedRoute.params.subscribe(params => {
      this.groupConductor.initFocussedGroupMember(params['groupMemberId']);
    });

    this._subscriptions.push(focussedMemberSubscription, parentRouteParamsSubscription, routeParamsSubscription);

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  onUpdateMemberSubmit() {

    const myDob = new Date(this.memberUpdateForm.value.dateOfBirth);
    const dobUTC = new Date(Date.UTC(myDob.getFullYear(), myDob.getMonth(), myDob.getDate()));
    const myDateJoined = new Date(this.memberUpdateForm.value.dateJoined);
    const dateJoinedUTC = new Date(Date.UTC(myDateJoined.getFullYear(), myDateJoined.getMonth(), myDateJoined.getDate()));

    if (this.memberUpdateForm.valid) {

      // Update the visible entities
      this.groupMember.name = this.memberUpdateForm.value.name;
      this.groupMember.emailAddress = this.memberUpdateForm.value.email;
      this.groupMember.telephoneNumber = this.memberUpdateForm.value.telephone;
      this.groupMember.dateOfBirth = dobUTC;
      this.groupMember.dateJoined = dateJoinedUTC;
      this.groupMember.memberType = this.memberUpdateForm.value.memberType;

      const groupMemberUpdate: GroupMemberUpdateDto = {
        groupMemberId: this.groupMember.groupMemberId,
        name: this.memberUpdateForm.value.name,
        emailAddress: this.memberUpdateForm.value.email,
        telephoneNumber: this.memberUpdateForm.value.telephone,
        dateOfBirth: dobUTC,
        dateJoined: dateJoinedUTC,
        facebook: null,
        memberType: this.memberUpdateForm.value.memberType,
      };

      this.groupConductor.updateMember(groupMemberUpdate);
    }
  }

  onGroupMemberRemoveClick() {
    this.groupConductor.removeMember(this.groupMember);
    this._location.back();
  }
}
