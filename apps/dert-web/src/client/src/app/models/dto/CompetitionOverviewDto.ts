import { CompetitionSummaryDto } from './CompetitionSummaryDto';

export interface CompetitionOverviewDto {
    id: number;
    name: string;
    summary: CompetitionSummaryDto;
}
