import { ActivityMemberAttendanceDto } from 'app/models/dto';

    export interface MemberAttendanceDto {
        id: number;
        registrationId: number;
        groupMemberId: number;
        attendanceClassificationId: number;
        groupMemberName: string;
        groupMemberType: number;
        eventName: string;
        eventPictureUrl: string;
        attendanceClassificationName: string;
        attendanceClassificationPrice: string;
        attendanceActivities: ActivityMemberAttendanceDto[];
    }
