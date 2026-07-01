
    export interface NotificationMessageDto {
        id: number;
        messageHeader: string;
        messageSummary: string;
        messageBody: string;
        hasDetails: boolean;
        isDismissable: boolean;
        blocksUser: boolean;
        severity: number;
        dateCreated: Date;
    }
