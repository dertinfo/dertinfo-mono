import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { GroupTeamDto, GroupTeamSubmissionDto } from 'app/models/dto';
import { TeamsSelectItemModel } from '../models/teams-select-item.model';
import { TeamSelectMediator } from '../services/teams-select.mediator';

@Component({
  selector: 'app-teams-select',
  templateUrl: './teams-select.component.html',
  styleUrls: ['./teams-select.component.css']
})
export class TeamsSelectComponent implements OnInit, OnDestroy {

  public get showSelect() {
     return this.teamSelectMediator.showSelect;
  }

  public get showCreate() {
    return this.teamSelectMediator.showCreate;
 }

  private _subscriptions: Subscription[] = [];
  private _teamNamePattern = '^(?!.*\/).*$';

  public all: GroupTeamDto[] = [];
  public selected: GroupTeamDto[] = [];
  public edited: GroupTeamDto[] = [];

  public items: TeamsSelectItemModel[] = [];
  public form: FormGroup;

  constructor(

    private teamSelectMediator: TeamSelectMediator
  ) {
  }

  ngOnInit() {

    this.form = new FormGroup({
      teamName: new FormControl('', [
        Validators.required,
        Validators.pattern(this._teamNamePattern)
      ]),
      teamBio: new FormControl('', [])
    });

    const allGroupTeamsSubs = this.teamSelectMediator.fullSetChange$.subscribe((all) => {

      if (all) {
        this.all = all;
        this.items = this.prepareItems();
      }
    });

    const attendingTeamsSubs = this.teamSelectMediator.selectedChange$.subscribe((selected) => {

      if (selected) {
        this.selected = selected;
        this.items = this.prepareItems();
      }
    });

    this._subscriptions.push(allGroupTeamsSubs);
    this._subscriptions.push(attendingTeamsSubs);
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

  public onCancelClick() {

    this.teamSelectMediator.announceCloseModal();
  }

  /**
   * Action taken when a selection event occours.
   * Manages state inside this component until save button hit.
   * @param groupTeamSelectModel the group team related to event
   */
  public onItemClick(groupTeamSelectModel: TeamsSelectItemModel) {

    const item = this.all.find(gm => gm.teamId === groupTeamSelectModel.id);
    const selecteditem = this.selected.find(gm => gm.teamId === groupTeamSelectModel.id);
    const editeditem = this.edited.find(gm => gm.teamId === groupTeamSelectModel.id);

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

      const newSubmission: GroupTeamSubmissionDto = {
        teamName: this.form.value.teamName,
        teamBio: this.form.value.teamBio
      };

      this.teamSelectMediator.announceNew(newSubmission);
    }
  }

  /**
   * [Event] button click event to save the changes to the current teams selection.
   */
  public onSelectionSaveClick() {

    this.teamSelectMediator.announceChanges(this.edited);
  }

  private prepareItems() {

    return this.all.map(gm => {

      const selectedTeam: GroupTeamDto = this.selected.find(sgm => sgm.teamId === gm.teamId);
      const editedTeam: GroupTeamDto = this.edited.find(sgm => sgm.teamId === gm.teamId);

      return {
        id: gm.teamId,
        name: gm.teamName,
        selected: !!selectedTeam,
        changed: !!editedTeam
      };
    });
  }
}
