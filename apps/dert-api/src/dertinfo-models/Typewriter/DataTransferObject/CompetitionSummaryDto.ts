
    export interface CompetitionSummaryDto {
        competitionId: number;
        competitionName: string;
        venuesCount: number;
        competitionDanceSummaryDto: CompetitionDanceSummaryDto;
        numberOfCompetitionEntries: number;
        hasBeenPopulated: boolean;
        hasDancesGenerated: boolean;
        allowPopulation: boolean;
        allowDanceGeneration: boolean;
        allowAdHocDanceAddition: boolean;
        entryAttributes: CompetitionEntryAttributeDto[];
        numberOfTicketsSold: number;
        resultsPublished: boolean;
    }
