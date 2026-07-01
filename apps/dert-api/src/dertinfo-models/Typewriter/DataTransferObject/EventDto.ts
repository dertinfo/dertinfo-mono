
    export interface EventDto {
        id: number;
        name: string;
        eventPictureUrl: string;
        eventSynopsis: string;
        registrationOpenDate: Date;
        registrationCloseDate: Date;
        eventStartDate: Date;
        eventEndDate: Date;
        locationTown: string;
        locationPostcode: string;
        isConfigured: boolean;
        isCancelled: boolean;
    }
