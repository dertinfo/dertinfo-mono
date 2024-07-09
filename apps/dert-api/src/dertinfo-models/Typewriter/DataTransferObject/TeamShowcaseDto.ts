
    export interface TeamShowcaseDto {
        id: number;
        teamName: string;
        teamPictureUrl: string;
        teamBio: string;
        attendedEvents: EventShowcaseDto[];
    }
