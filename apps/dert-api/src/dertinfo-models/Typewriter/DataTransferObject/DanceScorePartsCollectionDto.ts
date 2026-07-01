
    export interface DanceScorePartsCollectionDto {
        danceId: number;
        danceScoreParts: DanceScorePartDto[];
    }
    export interface DanceScorePartDto {
        danceScorePartId: number;
        danceScoreId: number;
        judgeSlotId: number;
        scoreGiven: number;
        scoreCategoryTag: string;
        judgeName: string;
    }
