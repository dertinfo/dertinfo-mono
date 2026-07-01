
    export interface EventRegistrationDto {
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
        flowState: number;
    }
