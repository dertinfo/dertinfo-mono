import { Injectable } from '@angular/core';
import { AuthService } from 'app/core/authentication/auth.service';
import { NavigationService } from 'app/core/services/navigation.service';
import {
    GroupConfigurationSubmissionDto,
    GroupDto,
    GroupOverviewDto,
} from 'app/models/dto';
import { Observable } from 'rxjs';
import { GroupSetupRepository } from './group-setup.repository';
import { GroupSetupTracker } from './group-setup.tracker';

declare function require(url: string);
const help = require('../../../../assets/help/app-group-configure.en.json');

@Injectable()
export class GroupSetupConductor {

    private startStepperOrder = ['group-details', 'contact-details', 'group-image', 'visibility', 'terms', 'done'];

    constructor(
        public authService: AuthService,
        private groupRepo: GroupSetupRepository,
        private navigationService: NavigationService,
        private _tracker: GroupSetupTracker
    ) { }

    public setOverview(groupOverviewDto: GroupOverviewDto) {

        if (this._tracker.overview && this._tracker.overview.id !== groupOverviewDto.id) {
            this._tracker.reset();
        }

        this._tracker.overview = groupOverviewDto;
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

    public startStepperStepChanged($group: any) {

        // Help information change
        if (this.startStepperOrder.length > $group.selectedIndex) {
            this._tracker.help = help[this.startStepperOrder[$group.selectedIndex]];
        } else {
            this._tracker.help = { header: 'Help Not Available', body: '' };
        }

        // Update the memory state of overview as progressing but do not submit.
        const stepFormGroup = $group.previouslySelectedStep.stepControl;
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

    public submitConfiguration(groupConfigurationSubmissionDto: GroupConfigurationSubmissionDto) {

        const applyConfiguration$ = this.groupRepo.applyConfiguration(
            this._tracker.groupId,
            groupConfigurationSubmissionDto
        );

        const applyConfigurationSubscription = applyConfiguration$.subscribe((groupDto) => {
            this.navigationService.markGroupConfiguredForUser(groupDto.id);
            applyConfigurationSubscription.unsubscribe();
        });

        return applyConfiguration$;
    }

    public newImageUploaded() {
        const subs = this.groupRepo.images(this._tracker.groupId).subscribe((groupImages) => {
            subs.unsubscribe();
            if (groupImages && groupImages.length > 0) {
                const subs2 = this.groupRepo.setPrimaryImage(groupImages[groupImages.length - 1]).subscribe((groupImage) => {
                    subs2.unsubscribe();

                    const groupOverview = this._tracker.overview;
                    groupOverview.groupPictureUrl = groupImages[groupImages.length - 1].imageResourceUri;
                    this._tracker.overview = groupOverview;
                });
            }
        });
    }

    public abandonConfiguration(): Observable<GroupDto> {
        const abandonConfiguration$ = this.groupRepo.abandonConfiguration(
            this._tracker.groupId
        );

        const abandonConfigurationSubscription = abandonConfiguration$.subscribe((groupDto) => {
            abandonConfigurationSubscription.unsubscribe();
            this.authService.renewToken()
                .then((data) => {
                    this.navigationService.removeGroupForUser(groupDto);
                })
                .catch((err) => {
                    console.error('abandonConfiguration failed to refresh the token');
                });
        });

        return abandonConfiguration$;
    }

    public clear() {
        this._tracker.reset();
    }

    // #endregion
}
