
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

import { GroupImage, GroupOverview } from 'app/models/app';
import {
    EventDto,
    GroupImageDto,
    GroupInvoiceDto,
    GroupMemberDetailDto,
    GroupMemberDto,
    GroupOverviewUpdateDto,
    GroupRegistrationDto,
    GroupTeamDetailDto,
    GroupTeamDto
} from 'app/models/dto';
@Injectable()
export class GroupAdminTracker {
    private _activeRegistrationStates: number[] = [0, 1, 2];

    private _groupOverview = new BehaviorSubject<GroupOverview>(null);
    private _groupMembers = new BehaviorSubject<GroupMemberDto[]>([]);
    private _groupImages = new BehaviorSubject<GroupImage[]>([]);
    private _groupInvoices = new BehaviorSubject<GroupInvoiceDto[]>([]);
    private _groupTeams = new BehaviorSubject<GroupTeamDto[]>([]);
    private _groupRegistrations = new BehaviorSubject<GroupRegistrationDto[]>([]);
    // private _activeRegistrations: BehaviorSubject<GroupRegistrationDto[]>;
    private _focussedGroupMember = new BehaviorSubject<GroupMemberDetailDto>(null);
    private _focussedGroupTeam = new BehaviorSubject<GroupTeamDetailDto>(null);
    private _teamImages = new BehaviorSubject<GroupImage[]>([]);
    private _availableEvents = new BehaviorSubject<EventDto[]>([]);
    private _promotedEvents = new BehaviorSubject<EventDto[]>([]);

    get groupOverview$() { return this._groupOverview.asObservable(); }
    get groupMembers$() { return this._groupMembers.asObservable(); }
    get groupImages$() { return this._groupImages.asObservable(); }
    get groupInvoices$() { return this._groupInvoices.asObservable(); }
    get groupTeams$() { return this._groupTeams.asObservable(); }
    get groupRegistrations$() { return this._groupRegistrations.asObservable(); }
    get activeRegistrations$() {
        return this.groupRegistrations$.pipe(map(
            groupRegistrations => groupRegistrations.filter(registration => {
                return this._activeRegistrationStates.indexOf(registration.flowState) > -1;
            })
        ));
    }
    get focussedGroupMember$() { return this._focussedGroupMember.asObservable(); }
    get focussedGroupTeam$() { return this._focussedGroupTeam.asObservable(); }
    get teamImages$() { return this._teamImages.asObservable(); }
    get availableEvents$() { return this._availableEvents.asObservable(); }
    get promotedEvents$() { return this._promotedEvents.asObservable(); }

    private _memoryStore: {
        overview: GroupOverview,
        members: GroupMemberDto[],
        images: GroupImageDto[],
        invoices: GroupInvoiceDto[],
        teams: GroupTeamDto[],
        registrations: GroupRegistrationDto[],
        memberDetail: { [id: number]: GroupMemberDetailDto },
        teamDetail: { [id: number]: GroupTeamDetailDto },
        availableEvents: EventDto[],
        promotedEvents: EventDto[],
    };

    public get groupId(): number {
        return this._memoryStore.overview.id;
    }

    public get overview(): GroupOverview {
        return this._memoryStore.overview;
    }
    public set overview(overview: GroupOverview) {
        this._memoryStore.overview = overview;
        this._groupOverview.next(this._memoryStore.overview);
    }

    public set members(members: GroupMemberDto[]) {
        this._memoryStore.members = members;
        this._groupMembers.next(this._memoryStore.members);
    }

    public set images(images: GroupImageDto[]) {
        this._memoryStore.images = images;
        this._groupImages.next(this.MarkSelectedImage(images, this.overview.groupPictureUrl || ''));
        this._teamImages.next(this.MarkSelectedImage(images, null));
    }

    public set invoices(invoices: GroupInvoiceDto[]) {
        this._memoryStore.invoices = invoices;
        this._groupInvoices.next(this._memoryStore.invoices);
    }

    public set registrations(registrations: GroupRegistrationDto[]) {
        this._memoryStore.registrations = registrations;
        this._groupRegistrations.next(registrations);

        this.filterEventsByAlreadyRegistered(registrations);
        this._availableEvents.next(this._memoryStore.availableEvents);
    }

    public set teams(teams: GroupTeamDto[]) {
        this._memoryStore.teams = teams;
        this._groupTeams.next(teams);
    }

    public set availableEvents(events: EventDto[]) {
        this._memoryStore.availableEvents = events;

        this.filterEventsByAlreadyRegistered(this._memoryStore.registrations || []);
        this._availableEvents.next(this._memoryStore.availableEvents);
    }

    public set promotedEvents(events: EventDto[]) {
        this._memoryStore.promotedEvents = events;

        this.filterEventsByAlreadyRegistered(this._memoryStore.registrations || []);
        this._promotedEvents.next(this._memoryStore.promotedEvents);
    }

    public hasLoadedOverview(): boolean {
        return this._memoryStore.overview !== null;
    }

    public hasLoadedMembers(): boolean {
        return this._memoryStore.members !== null;
    }

    public hasLoadedImages(): boolean {
        return this._memoryStore.images !== null;
    }

    public hasLoadedInvoices(): boolean {
        return this._memoryStore.invoices !== null;
    }

    public hasLoadedTeams(): boolean {
        return this._memoryStore.teams !== null;
    }

    public hasLoadedRegistrations(): boolean {
        return this._memoryStore.registrations !== null;
    }

    public hasLoadedEvents(): boolean {
        return this._memoryStore.availableEvents !== null;
    }

    public hasLoadedPromotedEvents(): boolean {
        return this._memoryStore.promotedEvents !== null;
    }

    public hasMemberDetail(memberId: number): boolean {
        return this._memoryStore.memberDetail !== null && this._memoryStore.memberDetail[memberId] !== undefined;
    }

    public hasTeamDetail(teamId: number): boolean {
        return this._memoryStore.teamDetail !== null && this._memoryStore.teamDetail[teamId] !== undefined;
    }

    public reset(): void {
        this._memoryStore = {
            overview: null,
            members: null,
            images: null,
            invoices: null,
            teams: null,
            registrations: null,
            memberDetail: {},
            teamDetail: {},
            availableEvents: null,
            promotedEvents: null,
        };

        this._groupOverview.next(null);
        this._groupMembers.next([]);
        this._groupImages.next([]);
        this._groupInvoices.next([]);
        this._groupTeams.next([]);
        this._groupRegistrations.next([]);
        this._availableEvents.next([]);
        this._promotedEvents.next([]);

        this._focussedGroupMember.next(null);
        this._focussedGroupTeam.next(null);
    }

    public updateOverview(update: GroupOverviewUpdateDto) { // todo - should be GroupOverviewUpdateSubmissionDto

        this._memoryStore.overview.groupName = update.groupName;
        this._memoryStore.overview.groupEmail = update.groupEmail;
        this._memoryStore.overview.groupBio = update.groupBio;
        this._memoryStore.overview.contactTelephone = update.contactTelephone;
        this._memoryStore.overview.contactName = update.contactName;
        this._memoryStore.overview.originTown = update.originTown;
        this._memoryStore.overview.originPostcode = update.originPostcode;
        this._memoryStore.overview.visibility = update.visibility;

        this._groupOverview.next(this._memoryStore.overview);
    }

    public addMember(member: GroupMemberDto) {
        this._memoryStore.members.push(member);
        this._memoryStore.overview.membersCount = this._memoryStore.members.length;
        this._groupMembers.next(this._memoryStore.members);
    }

    public addMemberDetail(memberDetail: GroupMemberDetailDto, setFocussed: boolean): void {
        this._memoryStore.memberDetail[memberDetail.groupMemberId] = memberDetail;

        if (setFocussed) {
            this._focussedGroupMember.next(this._memoryStore.memberDetail[memberDetail.groupMemberId]);
        }
    }

    public updateMember(updatedMember: GroupMemberDto) {
        // note - if we are updating a member they are already focussed.

        const memberIndex = this._memoryStore.members.findIndex(m => m.groupMemberId === updatedMember.groupMemberId);
        const foundMember = this._memoryStore.members[memberIndex];
        const foundMemberDetail = this._memoryStore.memberDetail[updatedMember.groupMemberId];

        updatedMember.memberAttendances = [...foundMember.memberAttendances];
        this._memoryStore.members[memberIndex] = updatedMember;

        foundMemberDetail.name = updatedMember.name;
        foundMemberDetail.emailAddress = updatedMember.emailAddress;
        foundMemberDetail.telephoneNumber = updatedMember.telephoneNumber;
        foundMemberDetail.dateOfBirth = updatedMember.dateOfBirth;
        foundMemberDetail.dateJoined = updatedMember.dateJoined;
        foundMemberDetail.memberType = updatedMember.memberType;

        this._groupMembers.next(this._memoryStore.members);
        this._focussedGroupMember.next(foundMemberDetail);
    }

    public deleteMember(member: GroupMemberDto) {
        this._memoryStore.members = this._memoryStore.members.filter(gm => gm.groupMemberId !== member.groupMemberId);
        this._memoryStore.overview.membersCount = this._memoryStore.members.length;
        this._groupMembers.next(this._memoryStore.members);

        const detail = this._memoryStore.memberDetail[member.groupMemberId];
        if (detail) {
            this._memoryStore.memberDetail[member.groupMemberId] = null;
        }
    }

    public addTeam(team: GroupTeamDto) {
        this._memoryStore.teams.push(team);
        this._memoryStore.overview.teamsCount = this._memoryStore.teams.length;
        this._groupTeams.next(this._memoryStore.teams);
    }

    public addTeamDetail(teamDetail: GroupTeamDetailDto, setFocussed: boolean): void {
        this._memoryStore.teamDetail[teamDetail.teamId] = teamDetail;

        if (setFocussed) { this.focusTeamDetail(teamDetail.teamId); }
    }

    public updateTeam(updatedTeam: GroupTeamDto) {
        // note - if we are updating a member they are already focussed.

        const teamIndex = this._memoryStore.teams.findIndex(t => t.teamId === updatedTeam.teamId);
        const foundTeam = this._memoryStore.teams[teamIndex];
        const foundTeamDetail = this._memoryStore.teamDetail[updatedTeam.teamId];

        updatedTeam.teamAttendances = [...foundTeam.teamAttendances];
        this._memoryStore.teams[teamIndex] = updatedTeam;

        foundTeamDetail.teamName = updatedTeam.teamName;
        foundTeamDetail.teamBio = updatedTeam.teamBio;

        this._groupMembers.next(this._memoryStore.members);
        this._focussedGroupTeam.next(foundTeamDetail);
    }

    public deleteTeam(team: GroupTeamDto) {
        this._memoryStore.teams = this._memoryStore.teams.filter(t => t.teamId !== team.teamId);
        this._memoryStore.overview.teamsCount = this._memoryStore.teams.length;
        this._groupTeams.next(this._memoryStore.teams);

        const detail = this._memoryStore.teamDetail[team.teamId];
        if (detail) {
            this._memoryStore.teamDetail[team.teamId] = null;
        }
    }

    public deleteImage(groupImageId: number) {

        const removalIndex = this._memoryStore.images.findIndex(i => i.groupImageId === groupImageId);
        const removedImage = this._memoryStore.images.splice(removalIndex, 1);
        this._groupImages.next(this.MarkSelectedImage(this._memoryStore.images, this.overview.groupPictureUrl || ''));
    }

    public focusMemberDetail(memberId: number) {
        this._focussedGroupMember.next(this._memoryStore.memberDetail[memberId]);
    }

    public focusTeamDetail(teamId: number) {
        this._teamImages.next(this.MarkSelectedImage(this._memoryStore.images || [], this._memoryStore.teamDetail[teamId].teamPictureUrl));
        this._focussedGroupTeam.next(this._memoryStore.teamDetail[teamId]);
    }

    public setGroupPrimaryImage(imageResourceUri: string) {
        this._memoryStore.overview.groupPictureUrl = imageResourceUri;
        this._groupOverview.next(this._memoryStore.overview);
        this._groupImages.next(this.MarkSelectedImage(this._memoryStore.images, imageResourceUri || ''));
    }

    public setTeamPrimaryImage(teamId: number, imageResourceUri: string) {
        const team = this._memoryStore.teams.find(t => t.teamId === teamId);
        const teamDetail = this._memoryStore.teamDetail[teamId];

        team.teamPictureUrl = imageResourceUri;

        teamDetail.teamPictureUrl = imageResourceUri;
        this.focusTeamDetail(teamId);
        this._teamImages.next(this.MarkSelectedImage(this._memoryStore.images, imageResourceUri || ''));

    }

    public startDelete() {
        this._memoryStore.overview.isDeleting = true;
        this._groupOverview.next(this._memoryStore.overview);
    }

    public completeDelete() {
        this._memoryStore.overview.isDeleted = true;
        this._groupOverview.next(this._memoryStore.overview);
    }

    private filterEventsByAlreadyRegistered(registrations: GroupRegistrationDto[]) {

        const eventIds = registrations.map(r => { return r.eventId; });

        if (this.hasLoadedEvents()) {
            const filtered = this._memoryStore.availableEvents.filter(e => eventIds.indexOf(e.id) === -1);
            this._memoryStore.availableEvents = filtered;
        }
    }

    private MarkSelectedImage(images: GroupImageDto[], currentUri: string): GroupImage[] {
        return images.map(i => {
            return {
                ...i,
                isPrimary: i.imageResourceUri === currentUri
            };
        });
    }
}
