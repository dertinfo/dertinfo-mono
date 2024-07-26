
    export interface NotificationSummaryDto {
        notificationAudienceLogId: number;
        title: string;
        summary: string;
        canOpen: boolean;
        canDismiss: boolean;
        hasBeenOpened: boolean;
        hasBeenSeen: boolean;
        hasBeenDeleted: boolean;
        hasBeenAcknowledged: boolean;
        mustAcknowledge: boolean;
        severity: number;
        messageType: number;
        dateCreated: Date;
    }
