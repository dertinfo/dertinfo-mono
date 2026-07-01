import { Injectable } from '@angular/core';
import { NavigationService } from 'app/core/services/navigation.service';
import {
    EventConfigurationSubmissionDto,
    EventOverviewDto
} from 'app/models/dto';
import { EventRepository } from 'app/modules/repositories';
import { AuthService } from '../../../core/authentication/auth.service';
import { EventSetupTracker } from './event-setup.tracker';

declare function require(url: string);
const help = require('../../../../assets/help/app-event-configure.en.json');

@Injectable()
export class EventSetupConductor {

    private startStepperOrder = ['event-details', 'contact-details', 'event-image', 'visibility', 'event-type', 'terms', 'done'];

    constructor(
        public authService: AuthService,
        private eventRepo: EventRepository,
        private navigationService: NavigationService,
        private _tracker: EventSetupTracker,

    ) { }

    public setOverview(eventOverviewDto: EventOverviewDto) {

        if (this._tracker.overview && this._tracker.overview.id !== eventOverviewDto.id) {
            this._tracker.reset();
        }

        this._tracker.overview = eventOverviewDto;
    }

    // #region Init

    /**
     * Get the user data for the currently logged in user
     */
    public initUserData() {
        if (!this._tracker.hasLoadedUserData()) {
            this._tracker.userData = this.authService.userData();
        }
    }

    // #endregion

    // #region Public Methods

    public startStepperStepChanged($event: any) {

        // Help information change
        if (this.startStepperOrder.length > $event.selectedIndex) {
            this._tracker.help = help[this.startStepperOrder[$event.selectedIndex]];
        } else {
            this._tracker.help = { header: 'Help Not Available', body: '' };
        }

        // Update the memory state of overview as progressing but do not submit.
        const stepFormGroup = $event.previouslySelectedStep.stepControl;
        const formGroupValues = stepFormGroup.value;

        const currentOverview = this._tracker.overview;

        // note - this is risky as it depends on the form value property being the same as the object value property
        Object.keys(formGroupValues).forEach((key, index) => {
            if (currentOverview[key] !== formGroupValues[key]) {
                currentOverview[key] = formGroupValues[key];
            }
        });

        this._tracker.overview = currentOverview;
    }

    public submitConfiguration(eventConfigurationSubmissionDto: EventConfigurationSubmissionDto) {

        const applyConfiguration$ = this.eventRepo.applyConfiguration(
            this._tracker.eventId,
            eventConfigurationSubmissionDto
        );

        const subs = applyConfiguration$.subscribe((eventDto) => {
            subs.unsubscribe();
            this.navigationService.markEventConfiguredForUser(eventDto.id);
        });

        return applyConfiguration$;
    }

    public newImageUploaded() {
        const subs = this.eventRepo.images(this._tracker.eventId).subscribe((eventImages) => {
            subs.unsubscribe();
            if (eventImages && eventImages.length > 0) {

                const subs2 = this.eventRepo.setPrimaryImage(eventImages[eventImages.length - 1]).subscribe((eventImage) => {
                    subs2.unsubscribe();

                    const eventOverview = this._tracker.overview;
                    eventOverview.eventPictureUrl = eventImages[eventImages.length - 1].imageResourceUri;
                    this._tracker.overview = eventOverview;
                });
            }
        });
    }

    public clear() {
        this._tracker.reset();
    }

    // #endregion
}
