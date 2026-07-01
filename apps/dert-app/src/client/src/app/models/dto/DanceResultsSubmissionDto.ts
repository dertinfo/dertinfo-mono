import { DanceScoreSubmissionDto } from './DanceScoreSubmissionDto';


export interface DanceResultsSubmissionDto {
    danceId: number;
    danceScores: DanceScoreSubmissionDto[];
    overrun: boolean;
}
