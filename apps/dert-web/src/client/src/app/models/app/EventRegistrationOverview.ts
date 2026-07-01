import { EventRegistration } from './EventRegistration';

export interface EventRegistrationOverview extends EventRegistration {
    invoicedTotal: number;
    currentTotal: number;
    isDeleting: boolean;
    isDeleted: boolean;
}
