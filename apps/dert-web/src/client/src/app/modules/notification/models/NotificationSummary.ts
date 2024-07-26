export interface NotificationSummary {
    id: number;
    title: string;
    summary: string;
    canOpen: boolean;
    canDismiss: boolean;
    showAsDeleted: boolean;
    showAsNew: boolean;
    icon: string;
    time: string;
    severity: number;
}
