/**
 * This score card model is used to present the results information for each of the scores
 * from dertofderts to the teams within the group area.
 */

export interface ScoreCard {
    dodResultId: number;
    dodSubmissionId: number;
    scoreCategories: Array<ScoreCardCategory>;
    comments: string;
    isOfficial: boolean;
}

export interface ScoreCardCategory {
    categoryName: string;
    maxMarks: number;
    score: number;
    comments: string;
}
