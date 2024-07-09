
    export interface RangeReportDto {
        judgeId: number;
        judgeName: string;
        scoreRanges: RangeReportSubRangeDto[];
    }
    export interface RangeReportSubRangeDto {
        rangeName: string;
        rangeTag: string;
        rangeValues: number[];
    }
