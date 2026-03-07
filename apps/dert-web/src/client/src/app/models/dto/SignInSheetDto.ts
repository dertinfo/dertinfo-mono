import { EventDto, MemberAttendanceDto, RegistrationDto } from '.';

    export interface SignInSheetDto {
        event: EventDto;
        registration: RegistrationDto;
        memberAttendances: MemberAttendanceDto[];
    }
