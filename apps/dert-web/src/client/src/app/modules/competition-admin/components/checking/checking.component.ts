import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { RangeValidatorDirective } from 'app/shared/directives/range-validator.directive';
import {
    DanceCheckedUpdateSubmissionDto,
    DanceDetailDto,
    DanceMarkingSheetDto,
    DanceResultsSubmissionDto,
    DanceScoreDto,
    DanceScorePartDto,
    JudgeSlotInformationDto
} from 'app/models/dto';
import { Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

@Component({
    selector: 'app-competition-checking',
    templateUrl: './checking.component.html',
    styleUrls: ['./checking.component.css']
})
export class CheckingComponent implements OnInit, OnDestroy {

    private _subscriptions: Subscription[] = [];
    private _isLoading = true;

    public danceResult: DanceDetailDto;
    public judgeSlotInformation: Array<JudgeSlotInformationDto>;

    public danceId: number;
    public headerImageSize = 'avatar';
    public danceScores: DanceScoreDto[];
    public danceMarkingSheets: DanceMarkingSheetDto[];
    public scoresLocked = false;
    public currentSlide = 'scores';
    public danceScoresFormGroup: FormGroup = new FormGroup({});
    public errorMessage: any;
    public overrun = false;
    public hasScoresChecked = false;
    public splitScoresHasChanged = false;

    constructor(
        private _activatedRoute: ActivatedRoute,
        private _conductor: CompetitionAdminConductor,
        private _tracker: CompetitionAdminTracker,

    ) { }

    ngOnInit() {

        this._subscriptions.push(this._tracker.focussedDanceResult$.subscribe((result: DanceDetailDto) => {
            this.focussedDanceResultChanged(result);
        }));

        this._subscriptions.push(this._tracker.focussedJudgeSlotInformation$.subscribe((result: Array<JudgeSlotInformationDto>) => {
            this.focussedDanceSplitResultChanged(result);
        }));

        // apply the settings through the conductor to the tracker from the resolver
        const danceResult: DanceDetailDto = this._activatedRoute.snapshot.data.danceResult;
        this._conductor.applyDanceResult(danceResult, true);
        this._conductor.applyJudgeSlotInformation(danceResult.danceId, this._activatedRoute.snapshot.data.judgeSlotInformation, true);
        // note - this grabbing of the danceId from the other object is dirty. Do not clone.
    }

    ngOnDestroy() {
        this._subscriptions.map((sub) => { sub.unsubscribe(); });
        this._subscriptions = []; // For good measure
    }

    public onScoresCheckedChanged() {

        this.hasScoresChecked = !this.hasScoresChecked;

        const danceHasScoresCheckedSubmission: DanceCheckedUpdateSubmissionDto = {
            hasScoresChecked: this.hasScoresChecked
        };

        this._conductor.updateDanceHasScoresChecked(this.danceId, danceHasScoresCheckedSubmission);
    }

    public onSaveScoreParts() {
        this._conductor.updateJudgeSlotInfomation(this.danceId, this.judgeSlotInformation);
        this.splitScoresHasChanged = false;
    }

    public onSplitScoresGridChanged(danceScorePartDto: DanceScorePartDto) {
        this.splitScoresHasChanged = true;
    }

    public onOverrunChange() {

        this.overrun = !this.overrun;
        for (const control in this.danceScoresFormGroup.controls) {
            if (this.overrun) {
                if (this.danceScoresFormGroup.controls[control]) {
                    this.danceScoresFormGroup.controls[control].setValue(this.danceScoresFormGroup.controls[control].value * 0.9);
                }
            } else {
                if (this.danceScoresFormGroup.controls[control]) {
                    this.danceScoresFormGroup.controls[control].setValue(this.danceScoresFormGroup.controls[control].value / 0.9);
                }

            }
        }
    }

    public submitScoresForm(value: any) {

        const danceId = this.danceResult.danceId;

        const danceResultsSubmission: DanceResultsSubmissionDto = {
            danceId: danceId,
            danceScores: [],
            overrun: this.overrun
        };

        this.danceScores.forEach((danceScore) => {
            danceResultsSubmission.danceScores.push(
                {
                    danceId: danceId,
                    scoreCategoryId: danceScore.scoreCatagoryId,
                    marksGiven: value[danceScore.scoreCatagoryId]
                }
            );
        });

        this._conductor.submitScores(danceResultsSubmission);

    }

    private focussedDanceResultChanged(danceResult: DanceDetailDto) {
        if (danceResult) {
            this.hasScoresChecked = danceResult.hasScoresChecked; // Must come before building the form
            this.danceId = danceResult.danceId;
            this.danceResult = danceResult;
            this.danceScores = danceResult.danceScores;
            this.danceMarkingSheets = danceResult.danceMarkingSheets;
            this.danceScoresFormGroup = this.toFormControlGroup();
            this.overrun = danceResult.overrun;
            this.scoresLocked = danceResult.hasScoresLocked;

            this._isLoading = false;
        } else {
            this.reset();
        }
    }

    private focussedDanceSplitResultChanged(judgeSlotInformation: Array<JudgeSlotInformationDto>) {
        if (judgeSlotInformation) {
            this.judgeSlotInformation = judgeSlotInformation;
        } else {
            this.reset();
        }
    }

    /**
     * toFormControlGroup
     * Takes the scores against the current dance object and creates a form group for these items with validation.
     */
    private toFormControlGroup() {

        const sortFunction = (a: DanceScoreDto, b: DanceScoreDto) => { return a.scoreCategorySortOrder - b.scoreCategorySortOrder; };

        const group: any = {};
        this.danceScores.sort(sortFunction).forEach((danceScore) => {

            const rangeValidator = new RangeValidatorDirective();
            rangeValidator.initValidationFunction(0, danceScore.scoreCatagoryMaxMarks);
            // Add to the group always with required validation

            const validators = this.scoresLocked ? null : [Validators.required, rangeValidator.validatorFunction];
            group[danceScore.scoreCatagoryId] = new FormControl({
                value: danceScore.markGiven,
                disabled: this.hasScoresChecked
            }, validators);

        });
        // group['overrun'] = new FormControl(this.overrun);
        return new FormGroup(group); // return the group.
    }

    private reset() {
        this.danceResult = null;
        this.danceScores = null;
        this.danceMarkingSheets = null;
        this.scoresLocked = false;
        this.danceScoresFormGroup = null;
        this.overrun = false;

        this._isLoading = false;
    }
}
