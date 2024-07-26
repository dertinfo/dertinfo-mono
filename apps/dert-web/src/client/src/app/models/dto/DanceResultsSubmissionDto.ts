import { DanceScoreSubmissionDto } from '.';

    export interface DanceResultsSubmissionDto {
        danceId: number;
        danceScores: DanceScoreSubmissionDto[];
        overrun: boolean;
    }
