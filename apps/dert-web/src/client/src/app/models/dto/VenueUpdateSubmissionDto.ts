import { JudgeSlotJudgeUpdateSubmissionDto } from '.';

export interface VenueUpdateSubmissionDto {
    name: string;
    judgeSlotUpdates: JudgeSlotJudgeUpdateSubmissionDto[];
}
