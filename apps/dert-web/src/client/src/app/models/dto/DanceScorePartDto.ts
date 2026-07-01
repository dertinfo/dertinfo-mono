
    export interface DanceScorePartDto {
        danceScorePartId: number;
        danceScoreId: number;
        judgeSlotId: number;
        scoreGiven: number;
        scoreCategoryTag: string;
        sortOrder: number;
        isPartOfScoreSet: boolean;
        /* Client property  */
        partsMatchScore: boolean;
    }
