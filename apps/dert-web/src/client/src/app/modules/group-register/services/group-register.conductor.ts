import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { NavigationService } from 'app/core/services/navigation.service';
import { EventOverview, InformationPanelData } from 'app/models/app';
import { EventImage } from 'app/models/app';
import { EventRegistration } from 'app/models/app/EventRegistration';
import { UserData } from 'app/models/auth/userdata.model';
import {
    EventDto,
    GroupDto,
    GroupRegistrationDto,
    GroupRegistrationSubmissionDto,
} from 'app/models/dto';
import { EventRepository } from 'app/modules/repositories';
import { BehaviorSubject ,  Observable ,  Subject } from 'rxjs';
import { AuthService } from '../../../core/authentication/auth.service';
import { GroupRegisterRepository } from './group-register.repository';
import { GroupRegisterTracker } from './group-register.tracker';

declare function require(url: string);
const help = require('../../../../assets/help/app-group-register.en.json');

@Injectable()
export class GroupRegisterConductor {

    private startStepperOrder = ['event-details', 'event-pricing', 'terms'];

    constructor(
        public authService: AuthService,
        private eventRepo: EventRepository,
        private groupRepo: GroupRegisterRepository,
        private navigationService: NavigationService,
        private _tracker: GroupRegisterTracker,

    ) { }

    public setOverviews(groupDto: GroupDto, eventDto: EventDto) {

        let doReset = false;
        if (this._tracker.groupDto && this._tracker.groupId !== groupDto.id) { doReset = true; }
        if (this._tracker.eventDto && this._tracker.eventId !== eventDto.id) { doReset = true; }

        if (doReset) {
            this._tracker.reset();
        }

        this._tracker.groupDto = groupDto;
        this._tracker.eventDto = eventDto;
    }

    public clear() {
        this._tracker.reset();
    }

    public initUserData() {
        if (!this._tracker.hasLoadedUserData()) {
            this._tracker.userData = this.authService.userData();
        }
    }

    // public initEventDto(eventId) {

    //     if (!this._tracker.hasLoadedEventDto() || this._tracker.eventId !== parseInt(eventId, 10)) {
    //         const subs = this.eventRepo.get(eventId).subscribe((eventDto) => {
    //             subs.unsubscribe();
    //             this._tracker.eventDto = eventDto;
    //         });
    //     }
    // }

    // public initGroupDto(groupId) {

    //     if (!this._tracker.hasLoadedGroupDto() || this._tracker.groupId !== parseInt(groupId, 10)) {
    //         const subs = this.groupRepo.get(groupId).subscribe((groupDto) => {
    //             subs.unsubscribe();
    //             this._tracker.groupDto = groupDto;
    //         });
    //     }
    // }

    public startStepperStepChanged($event: any) {

        // Help information change
        if (this.startStepperOrder.length > $event.selectedIndex) {
            this._tracker.help = help[this.startStepperOrder[$event.selectedIndex]];
        } else {
            this._tracker.help = { header: 'Help Not Available', body: '' };
        }

        const currentOverview = this._tracker.eventDto;

        // Update the memory state of overview as progressing but do not submit.
        if ($event.previouslySelectedStep.stepControl) {
            const stepFormGroup = $event.previouslySelectedStep.stepControl;
            const formGroupValues = stepFormGroup.value;

            // note - this is risky as it depends on the form value property being the same as the object value property
            Object.keys(formGroupValues).forEach((key, index) => {
                if (currentOverview[key] !== formGroupValues[key]) {
                    currentOverview[key] = formGroupValues[key];
                }
            });
        }

        this._tracker.eventDto = currentOverview;
    }

    public submitGroupRegistration(groupRegistrationSubmissionDto: GroupRegistrationSubmissionDto): Observable<GroupRegistrationDto> {

        const submitGroupRegistration$ = this.groupRepo.addRegistration(
            this._tracker.groupId,
            groupRegistrationSubmissionDto
        );

        const subs = submitGroupRegistration$.subscribe((eventDto) => {
            subs.unsubscribe();
            this.navigationService.markEventConfiguredForUser(eventDto.id);
        });

        return submitGroupRegistration$;
    }
}
