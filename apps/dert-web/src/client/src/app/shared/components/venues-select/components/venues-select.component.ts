import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { VenueDto, VenueSubmissionDto } from 'app/models/dto';
import { VenueSelectItemModel } from '../models/venue-select-item.model';
import { VenuesSelectMediator } from '../services/venues-select.mediator';

@Component({
  selector: 'app-venues-select',
  templateUrl: './venues-select.component.html',
  styleUrls: ['./venues-select.component.css']
})
export class VenuesSelectComponent implements OnInit, OnDestroy {

  public get showSelect() {
    return this._mediator.showSelect;
  }

  public get showCreate() {
    return this._mediator.showCreate;
  }

  private _subscriptions: Subscription[] = [];

  public all: VenueDto[] = [];
  public selected: VenueDto[] = [];
  public edited: VenueDto[] = [];

  public items: VenueSelectItemModel[] = [];
  public form: UntypedFormGroup;

  constructor(

    private _mediator: VenuesSelectMediator
  ) {
  }

  ngOnInit() {

    this.form = new UntypedFormGroup({
      name: new UntypedFormControl('', [Validators.required]),
      telephone: new UntypedFormControl('', []),
      email: new UntypedFormControl('', [customEmailValidator()])
    });

    this._subscriptions.push(this._mediator.fullSetChange$.subscribe((all) => {

      if (all) {
        this.all = all;
        this.items = this.prepareItems();
      }
    }));

    this._subscriptions.push(this._mediator.selectedChange$.subscribe((selected) => {

      if (selected) {
        this.selected = selected;
        this.items = this.prepareItems();
      }
    }));
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
   * @param selectModel the group member related to event
   */
  public onItemClick(selectModel: VenueSelectItemModel) {

    const item = this.all.find(j => j.id === selectModel.id);
    const selecteditem = this.selected.find(j => j.id === selectModel.id);
    const editeditem = this.edited.find(j => j.id === selectModel.id);

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

      const newSubmission: VenueSubmissionDto = {
        name: this.form.value.name
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

      const selected: VenueDto = this.selected.find(sj => sj.id === j.id);
      const edited: VenueDto = this.edited.find(sj => sj.id === j.id);

      return {
        id: j.id,
        name: j.name,
        selected: !!selected,
        changed: !!edited
      };
    });
  }

  // /**
  //  * Persists the changes for the edited users in the members selection
  //  */
  // private saveSelectionChanges() {
  //   // const subs =
  //   this.memberSelectMediator.announceChanges(this.editedVenues);
  //   // .subscribe(memberAttendaceStos => {
  //   //   subs.unsubscribe();
  //   //   this.composeDialog.closeAll();
  //   // });
  // }

}
