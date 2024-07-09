
    export interface GroupTeamCompetitionEntryDto {
        competitionEntryId: number;
        teamId: number;
        groupId: number;
        teamName: string;
        teamBio: string;
        teamPictureUrl: string;
        showShowcase: boolean;
        hasBeenAddedToCompetition: boolean;
        entryAttributes: CompetitionEntryAttributeDto[];
    }
