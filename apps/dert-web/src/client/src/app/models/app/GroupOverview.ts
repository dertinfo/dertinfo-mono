import { GroupOverviewDto } from '../dto';

export interface GroupOverview extends GroupOverviewDto {
    isDeleting: boolean;
    isDeleted: boolean;
}
