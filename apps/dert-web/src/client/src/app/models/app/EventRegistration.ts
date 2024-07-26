    export interface EventRegistration {
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
        flowStateText: string;
        flowStateColour: string;
    }
