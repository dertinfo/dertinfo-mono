
    export interface EventRegistrationOverviewDto {
        id: number;
        eventId: number;
        groupId: number;
        eventName: string;
        groupName: string;
        eventPictureUrl: string;
        groupPictureUrl: string;
        teamAttendancesCount: number;
        memberAttendancesCount: number;
        guestAttendancesCount: number;
        invoicedTotal: number;
        currentTotal: number;
        flowState: number;
        isGroupDeleted: boolean;
    }
