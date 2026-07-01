import { Flag } from 'app/models/app/Enumerations/Flags';

export interface StatusBlockDto {
    flag: Flag;
    flagText: string;
    title: string;
    subText: string;
    detailItems: string[];
}
