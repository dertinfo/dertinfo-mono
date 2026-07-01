import { GroupDto } from './';

export interface GroupOverviewDto extends GroupDto {
    groupEmail: string;
    contactTelephone: string;
    contactName: string;
    outstandingBalance: number;
    originTown: string;
    originPostcode: string;
    teamsCount: number;
    registrationsCount: number;
    membersCount: number;
    unpaidInvoicesCount: number;
    groupMemberJoiningPinCode: string;
    visibility: number;
}
