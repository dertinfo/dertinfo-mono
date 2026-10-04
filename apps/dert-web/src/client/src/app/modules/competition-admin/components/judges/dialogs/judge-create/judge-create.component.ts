import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';

import { JudgesSelectMediator } from 'app/shared/components/judges-select/services/judges-select.mediator';
import { JudgeDto, JudgeSubmissionDto } from 'app/models/dto';
import { CompetitionAdminConductor } from 'app/modules/competition-admin/services/competition-admin.conductor';
import { CompetitionAdminTracker } from 'app/modules/competition-admin/services/competition-admin.tracker';

@Component({
  selector: 'app-judge-create',
  templateUrl: './judge-create.template.html'
})
export class JudgeCreateComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _allJudges: JudgeDto[] = [];

  form: UntypedFormGroup;

  constructor(
    private composeDialog: MatDialog,
    private _selectCreateMediator: JudgesSelectMediator,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,
  ) {
  }

  ngOnInit() {

    const addNewMemberSubs = this._selectCreateMediator.newItemChange$.subscribe(newJudgeSubmission => {
      const subs = this._conductor.addJudge(newJudgeSubmission).subscribe(
        (savedMember) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('JudgeCreateComponent - Adding Judge Failed');
        }
      );
    });

    this._subscriptions.push(this._selectCreateMediator.itemsChange$.subscribe(changedSelections => {
      const subs = this._conductor.toggleJudgeAllocations(changedSelections).subscribe(
        (changes) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('JudgeCreateComponent - Updating Judges Failed');
        }
      );
    }));

    this._subscriptions.push(this._tracker.allJudges$.subscribe((judges) => {

      if (judges) {
        this._selectCreateMediator.applyFullSet(judges);
      }
    }));

    this._subscriptions.push(this._tracker.judges$.subscribe((judges) => {

      if (judges) {

        // note the spread here is filthy. Just cannot seem to work out why base collection getting changed.
        this._selectCreateMediator.applySelectedSet([...judges]);
      }
    }));

    this._subscriptions.push(this._selectCreateMediator.closeModal$.subscribe(() => {
      this.composeDialog.closeAll();
    }));

    this._conductor.initAllJudges();

  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  onCreateSubmit() {

    if (this.form.valid) {

      const createSubmission: JudgeSubmissionDto = {
        name: this.form.value.name,
        telephone: this.form.value.telephone,
        email: this.form.value.email
      };

      const subs = this._conductor.addJudge(createSubmission).subscribe(
        (saved) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('JudgeCreateComponent - Adding Venue Failed');
        }
      );
    }
  }
}
