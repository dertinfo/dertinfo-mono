import { EmailBaseDto } from './EmailBaseDto';

    export interface EmailRegistrationConfirmationDataDto extends EmailBaseDto {
        groupName: string;
        contactName: string;
        contactNumber: string;
        eventName: string;
        eventStartDate: Date;
        eventEndDate: Date;
        eventRegistrationCloseDate: Date;
        paymentDueDate: Date;
        siteLink: string;
        year: string;
    }
