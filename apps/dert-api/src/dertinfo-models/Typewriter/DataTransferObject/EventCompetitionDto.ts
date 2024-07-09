
    export interface EventCompetitionDto {
        competitionId: number;
        competitionName: string;
        status: StatusBlockDto;
        judges: StatusBlockDto;
        venues: StatusBlockDto;
        entrants: StatusBlockDto;
    }
