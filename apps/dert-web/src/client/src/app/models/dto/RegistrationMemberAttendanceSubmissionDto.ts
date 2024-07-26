import { GroupMemberSubmissionDto } from './GroupMemberSubmissionDto';

export interface RegistrationMemberAttendanceSubmissionDto {
    memberAttendanceId: number;
    groupMemberId: number;
    groupMemberSubmission?: GroupMemberSubmissionDto;
}
