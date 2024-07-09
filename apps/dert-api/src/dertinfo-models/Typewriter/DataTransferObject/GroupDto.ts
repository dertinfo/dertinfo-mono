
    export interface GroupDto {
        id: number;
        groupName: string;
        groupPictureUrl: string;
        groupBio: string;
        userAccessContext: GroupAccessContext;
        isConfigured: boolean;
    }
