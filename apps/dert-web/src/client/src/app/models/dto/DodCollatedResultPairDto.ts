import { DodTeamCollatedResultDto } from './DodTeamCollatedResultDto';

    export interface DodTeamCollatedResultPairDto {
        collatedOfficialResults: DodTeamCollatedResultDto[];
        collatedPublicResults: DodTeamCollatedResultDto[];
    }
