import { EventDto } from 'app/models/dto';

    export interface EventOverviewDto extends EventDto {
        contactName: string;
        contactEmail: string;
        contactTelephone: string;
        registrationsCount: number;
        visibility: number;
        sentEmailsBcc: string;
        membersAndGuestsCount: number;
        teamsCount: number;
    }
