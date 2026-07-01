import { GroupAccessContext } from '../app/Enumerations/GroupAccessContext';

export interface GroupDto {
    id: number;
    groupName: string;
    groupPictureUrl: string;
    groupBio: string;
    userAccessContext: GroupAccessContext;
    isConfigured: boolean;
    // the following properties are only set on the client object
    teamsCount: number;
    registrationsCount: number;
    membersCount: number;
}
