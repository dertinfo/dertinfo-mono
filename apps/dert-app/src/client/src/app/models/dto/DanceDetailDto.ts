import { DanceMarkingSheetDto } from './DanceMarkingSheetDto';
import { DanceScoreDto } from './DanceScoreDto';

export interface DanceDetailDto {
    danceId: number;
    competitionId: number;
    competitionName: string;
    teamId: number;
    teamName: string;
    teamPictureUrl: string;
    venueId: number;
    venueName: string;
    hasScoresEntered: boolean;
    hasScoresChecked: boolean;
    scoresEnteredBy: string;
    danceMarkingSheets: DanceMarkingSheetDto[];
    danceScores: DanceScoreDto[];
    overrun: boolean;
}
