import { GroupTeamSubmissionDto } from './GroupTeamSubmissionDto';

    export interface RegistrationTeamAttendanceSubmissionDto {
        teamAttendanceId: number;
        teamId: number;
        groupTeamSubmission?: GroupTeamSubmissionDto;
    }
