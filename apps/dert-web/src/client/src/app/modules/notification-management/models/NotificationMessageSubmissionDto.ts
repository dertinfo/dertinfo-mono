
    export interface NotificationMessageSubmissionDto {
        messageHeader: string;
        messageSummary: string;
        messageBody: string;
        hasDetails: boolean;
        requiresOpening: boolean;
        requiresAcknowledgement: boolean;
        severity: number;
        blocksUser: boolean;
    }
