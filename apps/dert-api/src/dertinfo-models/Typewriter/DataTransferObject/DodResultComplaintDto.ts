
    export interface DodResultComplaintDto {
        id: number;
        dodResultId: number;
        dodSubmissionId: number;
        forScores: boolean;
        forComments: boolean;
        isResolved: boolean;
        isValidated: boolean;
        isRejected: boolean;
        notes: string;
        createdBy: string;
        dateCreated: Date;
        scoreCard: DodGroupResultsScoreCardDto;
    }
