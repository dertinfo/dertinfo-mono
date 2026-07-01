import { Injectable } from '@angular/core';
import { DodIdentifyJudgeSubmissionDto } from 'app/models/dto/DodIdentifyJudgeSubmissionDto';
import { DodIdentifyJudgeSubmissionResponseDto } from 'app/models/dto/DodIdentifyJudgeSubmissionResponseDto';
import { DodRecoverSessionDto } from 'app/models/dto/DodRecoverSessionDto';
import { DodRecoverSessionSubmissionDto } from 'app/models/dto/DodRecoverSessionSubmissionDto';
import { DodResultDto } from 'app/models/dto/DodResultDto';
import { DodResultSubmissionDto } from 'app/models/dto/DodResultSubmissionDto';
import { AppConfirmService } from 'app/services/app-confirm/app-confirm.service';
import { Observable } from 'rxjs';
import { ScoreFormData } from '../models/scoreFormData.model';
import { SubmitFormData } from '../models/submitFormData.model';
import { Repository } from './dertofderts-public.repository';
import { SessionService } from './dertofderts-public.session';
import { Tracker } from './dertofderts-public.tracker';

@Injectable()
export class Conductor {

    constructor(
        private _tracker: Tracker,
        private _repository: Repository,
        private _sessionService: SessionService,
        private _appConfirmService: AppConfirmService,
    ) {
        this._tracker.reset();
    }

    public initSubmissions() {
        if (!this._tracker.hasLoadedSubmissions()) {
            const subs = this._repository.listSubmissions().subscribe((dodSubmissions) => {
                subs.unsubscribe();
                this._tracker.dodSubmissions = dodSubmissions;
            });
        }
    }

    public initTalks() {
        if (!this._tracker.hasLoadedTalks()) {
            const subs = this._repository.listTalks().subscribe((dodTalks) => {
                subs.unsubscribe();
                this._tracker.dodTalks = dodTalks;
            });
        }
    }

    public initScoreEntry(submissionId: number) {
        if (!this._tracker.hasLoadedSubmissions()) {
            const subs = this._repository.listSubmissions().subscribe((dodSubmissions) => {
                subs.unsubscribe();
                this._tracker.dodSubmissions = dodSubmissions;
                this._tracker.populateActiveSubmission(submissionId);
            });

        } else {
            this._tracker.populateActiveSubmission(submissionId);
        }
    }

    public initResults() {
        if (!this._tracker.hasLoadedResults()) {
            const subs = this._repository.getResults().subscribe((dodResults) => {
                subs.unsubscribe();
                this._tracker.dodResults = dodResults;
            });
        }
    }

    public submitScores(submissionId: number, scoresFormData: ScoreFormData, submitFormData: SubmitFormData): Observable<DodResultDto> {

        const submission = this.buildSubmitScoresSubmission(submissionId, scoresFormData, submitFormData);

        const obs$ = this._repository.submitScores(submission);

        const subs = obs$.subscribe((addedResult: DodResultDto) => {
            subs.unsubscribe();
            this._sessionService.addUserGuid(addedResult.userGuid);
            this._sessionService.addDanceJudged(addedResult.submissionId);
            this._sessionService.applyJudgeOfficialStatus(addedResult.isOfficialJudge);
            this._tracker.incrementResultCount(addedResult.submissionId);
        });

        return obs$;
    }

    public identifyJudge(submitFormData: any) {
        const submission = this.buildIdentifyJudgeSubmission(submitFormData);

        const obs$ = this._repository.identifyJudge(submission);

        const subs = obs$.subscribe((identificationResult: DodIdentifyJudgeSubmissionResponseDto) => {
            subs.unsubscribe();

            const newSession: SubmitFormData = {
                name: submission.userName,
                email: submission.userEmail,
                interestedInJudging: true, // as already a judge
                agreeToTermsAndConditions: true, // as rejected if not.
                officialJudge: identificationResult.isOfficialJudge,
                officialJudgePassword: '**********', // not relevant
            };

            this._sessionService.loadOrTemplateLocal();
            this._sessionService.loadOrTemplateSession(newSession);
            this._sessionService.addUserGuid(identificationResult.userGuid);
            this._sessionService.applyJudgeOfficialStatus(identificationResult.isOfficialJudge);
        });

        return obs$;
    }

    public recoverSession(email: any): Observable<DodRecoverSessionDto> {

        const userGuid = this._sessionService.getLocalUserGuid();
        const submission: DodRecoverSessionSubmissionDto = {
            userEmail: email,
            userGuid: userGuid
        };

        const obs$ = this._repository.recoverSession(submission);

        const subs = obs$.subscribe((recoveredSession: DodRecoverSessionDto) => {
            subs.unsubscribe();

            const submitFormData: SubmitFormData = {
                name: recoveredSession.userName,
                email: '', // Redundant
                agreeToTermsAndConditions: false, // Redundant
                interestedInJudging: false, // Redundant
                officialJudge: recoveredSession.officialJudge, // Redundant
                officialJudgePassword: null, // Redundant
            };

            this._sessionService.loadOrTemplateSession(submitFormData, userGuid);
            this._sessionService.applyJudgeOfficialStatus(recoveredSession.officialJudge);
            recoveredSession.dancesJudged.forEach(x => this._sessionService.addDanceJudged(x));
        });

        return obs$;
    }

    public confirmClearSession(): Observable<boolean> {
        const obs$ = this._appConfirmService.confirm(
            `Forget that I've ever been here!`,
            `Click 'OK' to clear all stored information from this device.
          We will no longer track the dances that you've scored and will remove any user infomation.
          Once cleared you will not be able to recover this data.`,
            'dod-dialog-panel');

        const subs = obs$.subscribe((confirmed: boolean) => {
            subs.unsubscribe();

            if (confirmed) { this.clearSession(); }
        });

        return obs$;
    }

    public clearSession() {
        this._sessionService.clearLocal();
        this._sessionService.clearSession();
    }

    private buildIdentifyJudgeSubmission(submitFormData: any): DodIdentifyJudgeSubmissionDto {

        return {
            userName: submitFormData.name,
            userEmail: submitFormData.email,
            judgePassword: submitFormData.officialJudgePassword,
            agreeToTermsAndConditions: submitFormData.agreeToTermsAndConditions,
        };
    }

    private buildSubmitScoresSubmission(
        submissionId: number,
        scoresFormData: ScoreFormData,
        submitFormData: SubmitFormData
    ): DodResultSubmissionDto {

        const userLocalGuid = this._sessionService.getLocalUserGuid();
        const userSessionGuid = this._sessionService.getSessionUserGuid();

        /*
         * note - we must check that the local guid matches the session guid as there may be
         *        latent local where not recovered. So we need to ensure that the new user isn't
         *        identified as the previous one.
         */
        const isSessionLocalMismatch = userLocalGuid !== userSessionGuid;

        const sendFormData = userLocalGuid == null || isSessionLocalMismatch;

        this._sessionService.loadOrTemplateLocal();
        this._sessionService.loadOrTemplateSession(submitFormData);

        return {
            submissionId: submissionId,
            userGuid: !sendFormData ? userLocalGuid : null,
            userName: sendFormData ? submitFormData.name : null,
            userEmail: sendFormData ? submitFormData.email : null,
            interestedInJudging: sendFormData ? submitFormData.interestedInJudging : false,
            officialJudge: sendFormData ? submitFormData.officialJudge : false,
            officialJudgePassword: sendFormData ? submitFormData.officialJudgePassword : null,
            agreeToTermsAndConditions: sendFormData ? submitFormData.agreeToTermsAndConditions : false,
            musicScore: scoresFormData.musicScore,
            musicComments: scoresFormData.musicComment,
            steppingScore: scoresFormData.steppingScore,
            steppingComments: scoresFormData.steppingComment,
            swordHandlingScore: scoresFormData.swordHandlingScore,
            swordHandlingComments: scoresFormData.swordHandlingComment,
            danceTechniqueScore: scoresFormData.danceTechniqueScore,
            danceTechniqueComments: scoresFormData.danceTechniqueComment,
            presentationScore: scoresFormData.presentationScore,
            presentationComments: scoresFormData.presentationComment,
            buzzScore: scoresFormData.buzzScore,
            buzzComments: scoresFormData.buzzComment,
            charactersScore: scoresFormData.charactersScore,
            charactersComments: scoresFormData.charactersComment,
            overallComments: scoresFormData.overallComments,
        };
    }

}
