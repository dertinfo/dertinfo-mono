import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DodBlockUserSubmissionDto } from 'app/models/dto/DodBlockUserSubmissionDto';
import { DodResultComplaintDto } from 'app/models/dto/DodResultComplaintDto';
import { DodResultRejectComplaintSubmissionDto } from 'app/models/dto/DodResultRejectComplaintSubmissionDto';
import { DodResultValidateComplaintSubmissionDto } from 'app/models/dto/DodResultValidateComplaintSubmissionDto';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { DodSubmissionSubmissionDto } from 'app/models/dto/DodSubmissionSubmissionDto';
import { DodSubmissionUpdateSubmissionDto } from 'app/models/dto/DodSubmissionUpdateSubmissionDto';
import { DodTalkDto } from 'app/models/dto/DodTalkDto';
import { DodTalkSubmissionDto } from 'app/models/dto/DodTalkSubmissionDto';
import { DodTalkUpdateSubmissionDto } from 'app/models/dto/DodTalkUpdateSubmissionDto';

import { Repository } from './dertofderts-admin.repository';
import { Tracker } from './dertofderts-admin.tracker';

@Injectable()
export class Conductor {

    private _snackBarDuration: number = 1200;

    constructor(

        private _tracker: Tracker,
        private _repository: Repository,
        private _snackbar: MatSnackBar,
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

    public initGroups() {
        if (!this._tracker.hasLoadedGroups()) {
            const subs = this._repository.listGroups().subscribe((groups) => {
                subs.unsubscribe();
                this._tracker.groups = groups;
            });
        }
    }

    public initJudges() {
        if (!this._tracker.hasLoadedJudgeInfos()) {
            const subs = this._repository.listJudges().subscribe((judges) => {
                subs.unsubscribe();
                this._tracker.dodJudgeInfos = judges;
            });
        }
    }

    public initComplaints(openCompaints: boolean, force: boolean = false) {
        if (!this._tracker.hasLoadedComplaints() || force) {
            const subs = this._repository.listComplaints(openCompaints).subscribe((complaints) => {
                subs.unsubscribe();
                this._tracker.dodComplaints = complaints;
            });
        }
    }

    public initFocussedSubmission(submissionId: number) {
        if (!this._tracker.hasLoadedSubmissions()) {
            const subs = this._repository.listSubmissions().subscribe((dodSubmissions) => {
                subs.unsubscribe();
                this._tracker.dodSubmissions = dodSubmissions;
                this._tracker.focusSubmission(submissionId);
            });
        } else {
            this._tracker.focusSubmission(submissionId);
        }
    }

    public initFocussedTalk(talkId: number) {
        if (!this._tracker.hasLoadedTalks()) {
            const subs = this._repository.listTalks().subscribe((dodTalks) => {
                subs.unsubscribe();
                this._tracker.dodTalks = dodTalks;
                this._tracker.focusTalk(talkId);
            });
        } else {
            this._tracker.focusTalk(talkId);
        }
    }

    public addSubmission(dodSubmissionSubmission: DodSubmissionSubmissionDto) {
        const subs = this._repository.addSubmission(dodSubmissionSubmission).subscribe((addedSubmission: DodSubmissionDto) => {
            subs.unsubscribe();
            this._tracker.addSubmission(addedSubmission);
        });
    }

    public addTalk(newTalkSubmission: DodTalkSubmissionDto) {
        const obs$ = this._repository.addTalk(newTalkSubmission);
        const subs = obs$.subscribe((addedTalk: DodTalkDto) => {
            subs.unsubscribe();
            this._tracker.addTalk(addedTalk);
        });
        return obs$;
    }

    public updateSubmission(submissionUpdate: DodSubmissionUpdateSubmissionDto) {
        const subs = this._repository.updateSubmission(submissionUpdate).subscribe((updatedSubmission: DodSubmissionDto) => {
            subs.unsubscribe();
            this._tracker.updateSubmission(updatedSubmission);
            this._snackbar.open('Submission Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public updateTalk(talkUpdate: DodTalkUpdateSubmissionDto) {
        const subs = this._repository.updateTalk(talkUpdate).subscribe((updatedTalk: DodTalkDto) => {
            subs.unsubscribe();
            this._tracker.updateTalk(updatedTalk);
            this._snackbar.open('Talk Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public deleteSubmission(dodSubmission: DodSubmissionDto) {
        const subs = this._repository.deleteSubmission(dodSubmission.id).subscribe((deletedSubmission: DodSubmissionDto) => {
            subs.unsubscribe();
            this._tracker.deleteSubmission(deletedSubmission.id);
        });
    }

    public deleteTalk(dodTalk: DodTalkDto) {
        const subs = this._repository.deleteTalk(dodTalk.id).subscribe((deletedTalk: DodTalkDto) => {
            subs.unsubscribe();
            this._tracker.deleteTalk(deletedTalk.id);
        });
    }

    public validateComplaint(complaint: DodResultComplaintDto) {
        const submission: DodResultValidateComplaintSubmissionDto = {
            dodResultComplaintId: complaint.id
        };

        const obs$ = this._repository.validateComplaint(submission);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.markComplaintResolved(complaint);
        });

        return obs$;
    }

    public rejectComplaint(complaint: DodResultComplaintDto) {

        const submission: DodResultRejectComplaintSubmissionDto = {
            dodResultComplaintId: complaint.id
        };

        const obs$ = this._repository.rejectComplaint(submission);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.markComplaintResolved(complaint);
        });

        return obs$;
    }

    blockJudge(userId: number) {
        const submission: DodBlockUserSubmissionDto = {
            dodUserId: userId,
            block: true,
            unBlock: false
        };

        const obs$ = this._repository.blockUser(submission);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.toggleJudgeBlocked(userId);
        });

        return obs$;
    }

    unblockJudge(userId: number) {
        const submission: DodBlockUserSubmissionDto = {
            dodUserId: userId,
            block: false,
            unBlock: true
        };

        const obs$ = this._repository.blockUser(submission);

        const subs = obs$.subscribe(() => {
            subs.unsubscribe();
            this._tracker.toggleJudgeBlocked(userId);
        });

        return obs$;
    }

}
