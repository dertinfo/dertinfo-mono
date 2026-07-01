import { DanceScorePartDto } from './DanceScorePartDto';

export interface JudgeSlotInformationDto {
    judgeSlotId: number;
    judgeId: number;
    judgeName: string;
    scoreParts: DanceScorePartDto[];
}
