import { Component, EventEmitter, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { JudgeDto, JudgeSubmissionDto } from 'app/models/dto';
import { JudgeSelectItemModel } from '../models/judge-select-item.model';
import { JudgesSelectMediator } from '../services/judges-select.mediator';

@Component({
  selector: 'app-judges-select',
  templateUrl: './judges-select.component.html',
  styleUrls: ['./judges-select.component.css']
})
export class JudgesSelectComponent implements OnInit, OnDestroy {

  public get showSelect() {
     return this._mediator.showSelect;
  }

  public get showCreate() {
    return this._mediator.showCreate;
 }

  private _subscriptions: Subscription[] = [];

  public all: JudgeDto[] = [];
  public selected: JudgeDto[] = [];
  public edited: JudgeDto[] = [];

  public items: JudgeSelectItemModel[] = [];
  public form: UntypedFormGroup;

  constructor(
    private _mediator: JudgesSelectMediator
  ) {
  }

  ngOnInit() {
    this.form = new UntypedFormGroup({
      name: new UntypedFormControl('', [Validators.required]),
      telephone: new UntypedFormControl('', []),
      email: new UntypedFormControl('', [customEmailValidator()])
    });

    const allJudgesSubs = this._mediator.fullSetChange$.subscribe((all) => {
      if (all) {
        this.all = all;
        this.items = this.prepareItems();
      }
    });

    const attendingMembersSubs = this._mediator.selectedChange$.subscribe((selected) => {
      if (selected) {
        this.selected = selected;
        this.items = this.prepareItems();
      }
    });

    this._subscriptions.push(allJudgesSubs);
    this._subscriptions.push(attendingMembersSubs);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

  public onCancelClick() {

    this._mediator.announceCloseModal();
  }

  /**
   * Action taken when a selection event occours.
   * Manages state inside this component until save button hit.
   * @param judgeSelectModel the group member related to event
   */
  public onItemClick(judgeSelectModel: JudgeSelectItemModel) {

    const item = this.all.find(j => j.id === judgeSelectModel.id);
    const selecteditem = this.selected.find(j => j.id === judgeSelectModel.id);
    const editeditem = this.edited.find(j => j.id === judgeSelectModel.id);

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

      const newSubmission: JudgeSubmissionDto = {
        name: this.form.value.name,
        email: this.form.value.email,
        telephone: this.form.value.telephone
      };

      // const submitMemberSubscription =
      this._mediator.announceNew(newSubmission);
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
    this._mediator.announceChanges(this.edited);
  }

  private prepareItems() {
    return this.all.map(j => {

      const selectedMember: JudgeDto = this.selected.find(sj => sj.id === j.id);
      const editedMember: JudgeDto = this.edited.find(sj => sj.id === j.id);

      return {
        id: j.id,
        name: j.name,
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
  //   this.memberSelectMediator.announceChanges(this.editedJudges);
  //   // .subscribe(memberAttendaceStos => {
  //   //   subs.unsubscribe();
  //   //   this.composeDialog.closeAll();
  //   // });
  // }

}
