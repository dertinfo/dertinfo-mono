
    export interface DodSubmissionDto {
        id: number;
        groupId: number;
        groupName: string;
        groupPictureUrl: string;
        embedLink: string;
        embedOrigin: string;
        dertYearFrom: string;
        dertVenueFrom: string;
        hasAnyResults: boolean;
        numberOfResults: number;
        isPremier: boolean;
        isChampionship: boolean;
        isOpen: boolean;
    }
