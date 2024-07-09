
    export interface NotificationMessageSubmissionDto {
        messageHeader: string;
        messageSummary: string;
        messageBody: string;
        hasDetails: boolean;
        requiresOpening: boolean;
        requiresAcknowledgement: boolean;
        severity: NotificationSeverity;
        notificationType: NotificationType;
        blocksUser: boolean;
    }
