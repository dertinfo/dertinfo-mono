import { TeamAttendanceDto } from 'app/models/dto';

    export interface GroupTeamDetailDto {
        teamId: number;
        groupId: number;
        teamName: string;
        teamBio: string;
        teamPictureUrl: string;
        showShowcase: boolean;
        teamAttendances: TeamAttendanceDto[];
    }
