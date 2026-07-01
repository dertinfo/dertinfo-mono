import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { GroupMemberDto, GroupMemberSubmissionDto } from 'app/models/dto';
import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { Subscription } from 'rxjs';
import { MemberType } from '../../../../models/app/Enumerations/MemberType';
import { MembersSelectItemModel } from '../models/members-select-item.model';
import { MemberSelectMediator } from '../services/members-select.mediator';

@Component({
  selector: 'app-members-select',
  templateUrl: './members-select.component.html',
  styleUrls: ['./members-select.component.css']
})
export class MembersSelectComponent implements OnInit, OnDestroy {

  public get showSelect() {
     return this.memberSelectMediator.showSelect;
  }

  public get showCreate() {
    return this.memberSelectMediator.showCreate;
 }

  private _subscriptions: Subscription[] = [];

  public memberTypes = [MemberType.active, MemberType.guest];

  public all: GroupMemberDto[] = [];
  public selected: GroupMemberDto[] = [];
  public edited: GroupMemberDto[] = [];

  public items: MembersSelectItemModel[] = [];
  public form: UntypedFormGroup;

  constructor(
    private memberSelectMediator: MemberSelectMediator
  ) {
  }

  ngOnInit() {

    this.form = new UntypedFormGroup({
      name: new UntypedFormControl('', [
        Validators.required
      ]),
      email: new UntypedFormControl('', [
        customEmailValidator()
      ]),
      telephone: new UntypedFormControl('', []),
      dateOfBirth: new UntypedFormControl('', []),
      dateJoined: new UntypedFormControl('', []),
      memberType: new UntypedFormControl(MemberType.active, [])
    });

    const allGroupMembersSubs = this.memberSelectMediator.fullSetChange$.subscribe((all) => {

      if (all) {
        this.all = all;
        this.items = this.prepareItems();
      }
    });

    const attendingMembersSubs = this.memberSelectMediator.selectedChange$.subscribe((selected) => {

      if (selected) {
        this.selected = selected;
        this.items = this.prepareItems();
      }
    });

    this._subscriptions.push(allGroupMembersSubs);
    this._subscriptions.push(attendingMembersSubs);
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

  public onCancelClick() {

    this.memberSelectMediator.announceCloseModal();
  }

  /**
   * Action taken when a selection event occours.
   * Manages state inside this component until save button hit.
   * @param groupMemberSelectModel the group member related to event
   */
  public onItemClick(groupMemberSelectModel: MembersSelectItemModel) {

    const item = this.all.find(gm => gm.groupMemberId === groupMemberSelectModel.id);
    const selecteditem = this.selected.find(gm => gm.groupMemberId === groupMemberSelectModel.id);
    const editeditem = this.edited.find(gm => gm.groupMemberId === groupMemberSelectModel.id);

    if (editeditem) {
      this.edited.splice(this.edited.indexOf(editeditem), 1);
    } else {
      this.edited.push(item);
    }

    if (selecteditem) {
      this.selected.splice(this.selected.indexOf(selecteditem), 1);
    } else {
      this.selected.push(item);
    }

    this.items = this.prepareItems();
  }

  public onCreateNewClick() {

    if (this.form.valid) {

      const newSubmission: GroupMemberSubmissionDto = {
        name: this.form.value.name,
        emailAddress: this.form.value.email,
        telephoneNumber: this.form.value.telephone,
        dateOfBirth: this.form.value.dateOfBirth,
        dateJoined: this.form.value.dateJoined,
        facebook: null,
        memberType: this.form.value.memberType
      };

      // const submitMemberSubscription =
      this.memberSelectMediator.announceNew(newSubmission);
      // .subscribe(
      //   (savedMember) => {
      //     submitMemberSubscription.unsubscribe();
      //     this.composeDialog.closeAll();
      //   },
      //   (error) => {
      //     console.error('ERROR: Adding Member Failed');
      //   }
      // );
    }
  }

  /**
   * [Event] button click event to save the changes to the current members selection.
   */
  public onSelectionSaveClick() {
    this.memberSelectMediator.announceChanges(this.edited);
  }

  private prepareItems() {

    return this.all.map(gm => {

      const selectedMember: GroupMemberDto = this.selected.find(sgm => sgm.groupMemberId === gm.groupMemberId);
      const editedMember: GroupMemberDto = this.edited.find(sgm => sgm.groupMemberId === gm.groupMemberId);

      return {
        id: gm.groupMemberId,
        name: gm.name,
        selected: !!selectedMember,
        changed: !!editedMember
      };
    });
  }

  // /**
  //  * Persists the changes for the edited users in the members selection
  //  */
  // private saveSelectionChanges() {
  //   // const subs =
  //   this.memberSelectMediator.announceChanges(this.editedGroupMembers);
  //   // .subscribe(memberAttendaceStos => {
  //   //   subs.unsubscribe();
  //   //   this.composeDialog.closeAll();
  //   // });
  // }

}
