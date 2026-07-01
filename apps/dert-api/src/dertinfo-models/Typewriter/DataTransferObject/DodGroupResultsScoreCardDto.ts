
    export interface DodGroupResultsScoreCardDto {
        dodResultId: number;
        dodSubmissionId: number;
        scoreCategories: DodGroupResultsScoreCategoryDto[];
        comments: string;
        isOfficial: boolean;
    }
