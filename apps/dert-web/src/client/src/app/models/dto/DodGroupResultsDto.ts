import { DodGroupResultsScoreCardDto } from './DodGroupResultsScoreCardDto';

export interface DodGroupResultsDto {
    submissionId: number;
    groupName: string;
    embedLink: string;
    embedOrigin: string;
    scoreCards: DodGroupResultsScoreCardDto[];
}
