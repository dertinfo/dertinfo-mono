import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import { NavigationService } from 'app/core/services/navigation.service';
import { GroupImage } from 'app/models/app';
import { GroupRegistration } from 'app/models/app/GroupRegistration';
import {
    GroupDto,
    GroupImageDto,
    GroupMemberDto,
    GroupMemberSubmissionDto,
    GroupMemberUpdateDto,
    GroupOverviewDto,
    GroupOverviewUpdateDto,
    GroupRegistrationDto,
    GroupTeamDto,
    GroupTeamSubmissionDto,
    GroupTeamUpdateDto,
    TeamImageDto,
    TeamImageSubmissionDto
} from 'app/models/dto';
import { EventRepository, GroupListCache } from 'app/modules/repositories';
import { GroupAdminRepository } from './group-admin.repository';
import { GroupAdminTracker } from './group-admin.tracker';

@Injectable()
export class GroupAdminConductor {

    private _snackBarDuration: number = 1200;

    constructor(

        private eventRepo: EventRepository,
        private groupRepo: GroupAdminRepository,
        private _groupTracker: GroupAdminTracker,
        private navigationService: NavigationService,
        private snackBar: MatSnackBar,
        private _groupListCache: GroupListCache
    ) {

        this._groupTracker.reset();
    }

    public clear() {

        this._groupTracker.reset();
    }

    public setOverview(groupOverviewDto: GroupOverviewDto) {

        if (this._groupTracker.overview && this._groupTracker.overview.id !== groupOverviewDto.id) {
            this._groupTracker.reset();
        }

        this._groupTracker.overview = { ...groupOverviewDto, isDeleting: false, isDeleted: false };
    }

    public initMembers(groupId, forceUpdate = false) {

        if (!this._groupTracker.hasLoadedMembers() || forceUpdate) {
            const subs = this.groupRepo.members(groupId).subscribe((members) => {
                subs.unsubscribe();
                this._groupTracker.members = members;
            });
        }
    }

    public initImages(groupId, forceUpdate = false) {

        if (!this._groupTracker.hasLoadedImages() || forceUpdate) {
            const subs = this.groupRepo.images(groupId).subscribe((groupImages) => {
                subs.unsubscribe();
                const overview = this._groupTracker.overview;

                // If there is only 1 image then this may be the first upload. Replace the overviewimage
                // if not a replacement it will be the original
                if (groupImages && groupImages.length === 1) {
                    overview.groupPictureUrl = groupImages[0].imageResourceUri;
                    this._groupTracker.overview = overview;
                }

                this._groupTracker.images = groupImages;
            });
        }
    }

    public initInvoices(groupId) {

        if (!this._groupTracker.hasLoadedInvoices()) {
            const subs = this.groupRepo.invoices(groupId).subscribe((invoices) => {
                subs.unsubscribe();
                this._groupTracker.invoices = invoices;
            });
        }
    }

    public initTeams(groupId) {

        if (!this._groupTracker.hasLoadedTeams()) {
            const subs = this.groupRepo.teams(groupId).subscribe((teams) => {
                subs.unsubscribe();
                this._groupTracker.teams = teams;
            });
        }
    }

    public initRegistrations(groupId) {

        if (!this._groupTracker.hasLoadedRegistrations()) {
            const subs = this.groupRepo.registrations(groupId).subscribe((registrations) => {
                subs.unsubscribe();
                this._groupTracker.registrations = registrations.map((rDto) => this.mapRegistrationDtoToGroupRegistration(rDto));

                // note - this is a hack and shouldn't really be here
                this.initPromotedEvents();
            });
        }
    }

    public initEventsAvailable() {

        if (!this._groupTracker.hasLoadedEvents()) {
            const subs = this.eventRepo.listAvailable().subscribe((eventDtos) => {
                subs.unsubscribe();
                this._groupTracker.availableEvents = eventDtos;
            });
        }
    }

    public initPromotedEvents() {

        if (!this._groupTracker.hasLoadedPromotedEvents()) {
            const subs = this.eventRepo.listPromoted().subscribe((eventDtos) => {
                subs.unsubscribe();
                this._groupTracker.promotedEvents = eventDtos;
            });
        }
    }

    public initFocussedGroupMember(memberId: number, forceUpdate: boolean = false) {

        if (!this._groupTracker.hasMemberDetail(memberId) || forceUpdate) {

            const subs = this.groupRepo.memberDetail(this._groupTracker.groupId, memberId).subscribe((groupMemberDetail) => {
                subs.unsubscribe();
                this._groupTracker.addMemberDetail(groupMemberDetail, true);
            });

        }
    }

    public initFocussedGroupTeam(teamId: number, forceUpdate: boolean = false) {

        if (!this._groupTracker.hasTeamDetail(teamId) || forceUpdate) {

            const subs = this.groupRepo.teamDetail(this._groupTracker.groupId, teamId).subscribe((teamDetail) => {
                subs.unsubscribe();
                this._groupTracker.addTeamDetail(teamDetail, true);
            });
        } else {
            this._groupTracker.focusTeamDetail(teamId);
        }
    }

    public setPrimaryImage(groupImage: GroupImage) {

        if (!groupImage.isPrimary) {
            const groupImageDto: GroupImageDto = {
                groupId: groupImage.groupId,
                groupImageId: groupImage.groupImageId,
                imageId: groupImage.imageId,
                imageResourceUri: null
            };

            const subs = this.groupRepo.setPrimaryImage(groupImageDto).subscribe(() => {
                subs.unsubscribe();
                this._groupTracker.setGroupPrimaryImage(groupImage.imageResourceUri);
            });
        }
    }

    public attachImageToTeam(teamImageSubmission: TeamImageSubmissionDto) {

        const subs = this.groupRepo.attachTeamImage(this._groupTracker.groupId, teamImageSubmission)
            .subscribe((teamImageDto: TeamImageDto) => {
                subs.unsubscribe();
                this._groupTracker.setTeamPrimaryImage(teamImageDto.teamId, teamImageDto.imageResourceUri);
            });
    }

    public refreshImages() {

        this.initImages(this._groupTracker.groupId, true);
    }

    public deleteImage(groupImage: GroupImageDto) {

        const subs = this.groupRepo.deleteImage(groupImage).subscribe((deletedImage: GroupImageDto) => {
            subs.unsubscribe();
            this._groupTracker.deleteImage(deletedImage.groupImageId);
        });
    }

    public addMember(groupMemberSubmission: GroupMemberSubmissionDto) {

        const memberAddObs = this.groupRepo.addMember(this._groupTracker.groupId, groupMemberSubmission);

        const subs = memberAddObs.subscribe((newMember: GroupMemberDto) => {
            subs.unsubscribe();
            this._groupTracker.addMember(newMember);
        });

        return memberAddObs;
    }

    public addTeam(groupTeamSubmission: GroupTeamSubmissionDto) {

        const teamAddObs = this.groupRepo.addTeam(this._groupTracker.groupId, groupTeamSubmission);

        const subs = teamAddObs.subscribe((newTeam: GroupTeamDto) => {
            subs.unsubscribe();
            this._groupTracker.addTeam(newTeam);
        });

        return teamAddObs;
    }

    public updateMember(groupMemberUpdate: GroupMemberUpdateDto) {

        const obs$ = this.groupRepo.updateMember(
            this._groupTracker.groupId,
            groupMemberUpdate
        );

        const subs = obs$.subscribe((updatedMember: GroupMemberDto) => {
            subs.unsubscribe();
            this._groupTracker.updateMember(updatedMember);
            this.snackBar.open('Member Settings Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public updateTeam(groupTeamUpdate: GroupTeamUpdateDto) {

        const subs = this.groupRepo.updateTeam(this._groupTracker.groupId, groupTeamUpdate).subscribe((updatedTeam: GroupTeamDto) => {
            subs.unsubscribe();
            this._groupTracker.updateTeam(updatedTeam);
            this.snackBar.open('Team Settings Updated', 'close', { duration: this._snackBarDuration });
        });
    }

    public removeMember(groupMember: GroupMemberDto) {

        const subs = this.groupRepo.removeMember(this._groupTracker.groupId, groupMember).subscribe((removedMember: GroupMemberDto) => {
            subs.unsubscribe();
            this._groupTracker.deleteMember(removedMember);
        });
    }

    public removeTeam(team: GroupTeamDto) {

        const subs = this.groupRepo.removeTeam(this._groupTracker.groupId, team).subscribe((removedTeam: GroupTeamDto) => {
            subs.unsubscribe();
            this._groupTracker.deleteTeam(removedTeam);
        });
    }

    public removeGroup() {

        this._groupTracker.startDelete();

        const subs = this.groupRepo.deleteGroup(this._groupTracker.groupId).subscribe((removedGroup: GroupDto) => {
            subs.unsubscribe();
            this._groupTracker.completeDelete();
            this.snackBar.open('Group Deleted', 'close', { duration: this._snackBarDuration });

            this._groupListCache.clearCache(); // todo - this would be tidier is we just remove the single

            // Insurance - to ensure the clear down has completed after the delete.
            setTimeout(() => {
                this.navigationService.removeGroupForUser(removedGroup);
                this._groupTracker.reset();
            }, 2000); // note - this matches with the redirect after delete.
        });
    }

    public updateGroupOverview(groupOverviewUpdate: GroupOverviewUpdateDto) {

        const subs = this.groupRepo.updateOverview(this._groupTracker.groupId, groupOverviewUpdate).subscribe(() => {
            subs.unsubscribe();
            this._groupTracker.updateOverview(groupOverviewUpdate);
            this.snackBar.open('Group Settings Updated', 'close', { duration: this._snackBarDuration });
            this.navigationService.updateGroupNameForUser(this._groupTracker.groupId, groupOverviewUpdate.groupName);
        });
    }

    public extractFlowState(registrationFlowStateId: number) {
        switch (registrationFlowStateId) {
            case 0: return ['pending', 'accent'];
            case 1: return ['submitted', 'accent'];
            case 2: return ['confirmed', 'accent'];
            case 3: return ['locked', 'warn'];
            case 4: return ['closed', 'default'];
            case 5: return ['cancelled', 'warn'];
            case 6: return ['deleted', 'warn'];
            case 7: return ['pulled', 'accent'];
            default: return ['unknown', 'warn'];
        }
    }

    public registrationActiveFlowStates(): number[] {
        return [0, 1, 2, 3];
    }

    public registrationInActiveFlowStates(): number[] {
        return [4, 5, 6, 7];
    }

    private mapRegistrationDtoToGroupRegistration(registrationDto: GroupRegistrationDto): GroupRegistration {

        const flowState = this.extractFlowState(registrationDto.flowState);

        const groupRegistration: GroupRegistration = {
            ...registrationDto,
            flowStateText: flowState[0],
            flowStateColour: flowState[1]
        };

        return groupRegistration;
    }
}
