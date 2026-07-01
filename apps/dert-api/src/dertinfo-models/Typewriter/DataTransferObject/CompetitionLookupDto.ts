
    export interface CompetitionLookupDto {
        events: CompetitionLookupEventDto[];
    }
    export interface CompetitionLookupEventDto {
        eventName: string;
        competitions: CompetitionLookupCompetitionDto[];
    }
    export interface CompetitionLookupCompetitionDto {
        competitionId: number;
        competitionName: string;
        resultTypes: CompetitionLookupResultTypeDto[];
    }
    export interface CompetitionLookupResultTypeDto {
        resultTypeKey: string;
        resultTypeName: string;
    }
