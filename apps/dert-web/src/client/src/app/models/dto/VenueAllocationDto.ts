import { JudgeSlotDto } from '.';

export interface VenueAllocationDto {
    id: number;
    name: string;
    judgesAllocated: boolean;
    judgeSlots: JudgeSlotDto[];
}
