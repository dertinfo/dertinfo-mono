import { DanceMarkingSheetDto, DanceScoreDto } from '.';

    export interface DanceDetailDto {
        danceId: number;
        competitionId: number;
        competitionName: string;
        teamId: number;
        teamName: string;
        teamPictureUrl: string;
        venueId: number;
        venueName: string;
        dateScoresEntered: Date;
        hasScoresEntered: boolean;
        hasScoresChecked: boolean;
        hasScoresLocked: boolean;
        scoresEnteredBy: string;
        overrun: boolean;
        danceMarkingSheets: DanceMarkingSheetDto[];
        danceScores: DanceScoreDto[];
    }
