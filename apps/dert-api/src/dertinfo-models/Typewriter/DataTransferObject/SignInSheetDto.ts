
    export interface SignInSheetDto {
        groupName: string;
        eventName: string;
        memberAttendanceCount: string;
        teamAttendanceCount: string;
        groupMemberPinCode: string;
        event: EventDto;
        registration: RegistrationDto;
        memberAttendances: MemberAttendanceDto[];
        teamAttendances: TeamAttendanceDto[];
    }
