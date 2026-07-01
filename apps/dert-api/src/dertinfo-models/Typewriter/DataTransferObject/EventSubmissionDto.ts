
    export interface EventSubmissionDto {
        id: number;
        eventName: string;
        eventSynopsis: string;
        registrationOpenDate: Date;
        registrationCloseDate: Date;
        eventStartDate: Date;
        eventEndDate: Date;
        templateSelection: string;
        base64StringImage: string;
        uploadImageExtension: string;
    }
