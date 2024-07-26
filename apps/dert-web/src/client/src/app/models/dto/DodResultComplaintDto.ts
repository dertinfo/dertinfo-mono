import { ScoreCard } from '../app/ScoreCard';
export interface DodResultComplaintDto {
    id: number;
    resultId: number;
    forScores: boolean;
    forComments: boolean;
    isResolved: boolean;
    isValidated: boolean;
    isRejected: boolean;
    notes: string;
    scoreCard: ScoreCard;
}
