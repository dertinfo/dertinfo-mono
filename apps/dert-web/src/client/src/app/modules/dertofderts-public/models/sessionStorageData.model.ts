export interface SessionStorageData {
    name: string;
    email: string;
    interestedInJudging: boolean;
    officialJudge: boolean;
    agreeToTermsAndConditions: boolean;
    dancesJudged: Array<number>;
    userGuid: string;
}
