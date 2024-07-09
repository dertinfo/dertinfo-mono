
    export interface DodResultSubmissionDto {
        submissionId: number;
        userGuid: string;
        userName: string;
        userEmail: string;
        interestedInJudging: boolean;
        officialJudge: boolean;
        officialJudgePassword: string;
        agreeToTermsAndConditions: boolean;
        musicScore: number;
        musicComments: string;
        steppingScore: number;
        steppingComments: string;
        swordHandlingScore: number;
        swordHandlingComments: string;
        danceTechniqueScore: number;
        danceTechniqueComments: string;
        presentationScore: number;
        presentationComments: string;
        buzzScore: number;
        buzzComments: string;
        charactersScore: number;
        charactersComments: string;
        overallComments: string;
    }
