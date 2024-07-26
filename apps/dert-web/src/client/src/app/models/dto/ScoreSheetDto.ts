import { CompetitionEntryAttributeDto, EventDto, ScoreCategoryDto } from '.';

    export interface ScoreSheetDto {
        danceId: string;
        judgeName: string;
        teamName: string;
        venueName: string;
        competitionName: string;
        competitionEntryAttributes: CompetitionEntryAttributeDto[];
        scoreCategories: ScoreCategoryDto[];
        event: EventDto;
    }
