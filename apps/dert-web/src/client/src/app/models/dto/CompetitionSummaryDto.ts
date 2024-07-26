import { CompetitionDanceSummaryDto } from './CompetitionDanceSummaryDto';
import { CompetitionEntryAttributeDto } from './CompetitionEntryAttributeDto';

export interface CompetitionSummaryDto {
    competitionId: number;
    competitionName: string;
    venuesCount: number;
    competitionDanceSummaryDto: CompetitionDanceSummaryDto;
    numberOfCompetitionEntries: number;
    numberOfTicketsSold: number;
    resultsPublished: boolean;
    hasBeenPopulated: boolean;
    hasDancesGenerated: boolean;
    allowPopulation: boolean;
    allowDanceGeneration: boolean;
    allowAdHocDanceAddition: boolean;
    entryAttributes: CompetitionEntryAttributeDto;
}
