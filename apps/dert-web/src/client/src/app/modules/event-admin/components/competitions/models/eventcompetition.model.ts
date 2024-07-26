export interface EventCompetitionModel {
    id: number;
    name: string;
    status: StatusBlockModel;
    entries: StatusBlockModel;
    judges: StatusBlockModel;
    venues: StatusBlockModel;
}

export interface StatusBlockModel {
    icon: string;
    iconClass: string;
    heading: string;
    subtext: string;

}
