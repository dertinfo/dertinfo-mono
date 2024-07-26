import { EmailActivityLineItemDto } from './EmailActivityLineItemDto';

export interface EmailIndividualAttendanceLineItemDto {
    fullName: string;
    clasificationName: string;
    clasificationPrice: number;
    activities: EmailActivityLineItemDto[];
}
