
    export interface GroupRegistrationDto {
        id: number;
        eventId: number;
        groupId: number;
        eventName: string;
        groupName: string;
        eventPictureUrl: string;
        groupPictureUrl: string;
        teamAttendancesCount: number;
        guestAttendancesCount: number;
        memberAttendancesCount: number;
        flowState: number;
    }
