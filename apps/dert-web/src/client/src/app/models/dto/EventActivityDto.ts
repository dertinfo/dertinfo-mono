import { ActivityDto } from '.';

    export interface EventActivityDto extends ActivityDto {
        attendanceCount: number;
        valueOfSales: number;
    }
