import { TeamAttendanceDto } from 'app/models/dto';

export interface GroupTeamDto {
    teamId: number;
    groupId: number;
    teamName: string;
    teamBio?: string;
    showShowcase?: boolean;
    teamPictureUrl?: string;
    teamAttendances: TeamAttendanceDto[];
}
