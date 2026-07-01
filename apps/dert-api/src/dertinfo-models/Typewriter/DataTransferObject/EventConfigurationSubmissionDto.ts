
    export interface EventConfigurationSubmissionDto {
        eventSynopsis: string;
        contactName: string;
        contactEmail: string;
        contactTelephone: string;
        registrationOpenDate: Date;
        registrationCloseDate: Date;
        eventStartDate: Date;
        eventEndDate: Date;
        visibilityType: number;
        templateType: number;
        locationTown: string;
        locationPostcode: string;
        agreeToTermsAndConditions: boolean;
    }
