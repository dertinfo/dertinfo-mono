
    export interface NotificationDetailDto {
        notificationAudienceLogId: number;
        title: string;
        body: string;
        isRead: boolean;
        canClear: boolean;
        requestAcknowledgement: boolean;
        dateCreated: Date;
    }
