import { CompetitionEntryAttributeDto } from '.';

    export interface GroupTeamCompetitionEntryDto {
        competitionEntryId: number;
        teamId: number;
        groupId: number;
        teamName: string;
        teamBio: string;
        teamPictureUrl: string;
        showShowcase: boolean;
        entryAttributes: Array<CompetitionEntryAttributeDto>;
        hasBeenAddedToCompetition: boolean;
    }
