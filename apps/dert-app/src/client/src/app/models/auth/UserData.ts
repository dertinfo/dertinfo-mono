export interface UserData {
    email: string;
    name: string;
    nickname: string;
    picture: string;
    superAdmin: boolean;
    groupAdmin: number[];
    eventAdmin: number[];
    venueAdmin: number[];
    groupMember: number[];
}
