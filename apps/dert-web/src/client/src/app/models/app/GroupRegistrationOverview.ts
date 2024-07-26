
import { GroupRegistration } from './GroupRegistration';

export interface GroupRegistrationOverview extends GroupRegistration {
    invoicedTotal: number;
    currentTotal: number;
    isEventDeleted: boolean;
    isDeleting: boolean;
    isDeleted: boolean;
}
