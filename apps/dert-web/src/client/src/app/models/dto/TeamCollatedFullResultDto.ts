import { CompetitionEntryAttributeDto, ScoreGroupResultDto } from '.';

    export interface TeamCollatedFullResultDto {
        teamName: string;
        teamEntryAttributes: CompetitionEntryAttributeDto[];
        scoreGroupResults: { [key: string]: ScoreGroupResultDto; };
        danceEnteredCount: number;
        danceCheckedCount: number;
        danceTotalCount: number;
    }
