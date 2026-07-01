import { EmailActivityLineItemDto } from './EmailActivityLineItemDto';

    export interface EmailTeamAttendanceLineItemDto {
        teamName: string;
        activities: EmailActivityLineItemDto[];
    }
