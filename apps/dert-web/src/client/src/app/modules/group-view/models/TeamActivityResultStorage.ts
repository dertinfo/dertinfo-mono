import { DanceDetailDto } from 'app/models/dto';

    export interface TeamActivityResultStorage {
        teamId: number;
        activityId: number;
        danceResults: Array<DanceDetailDto>;
    }
