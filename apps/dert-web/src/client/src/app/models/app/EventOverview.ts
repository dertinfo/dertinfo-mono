import { EventOverviewDto } from '../dto';

export interface EventOverview extends EventOverviewDto {
    isClosing: boolean;
    isClosed: boolean;
    isCancelling: boolean;
    isCancelled: boolean;
    isDeleting: boolean;
    isDeleted: boolean;
}
