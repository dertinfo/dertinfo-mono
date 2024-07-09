
    export interface EmailBaseDto {
        toAddresses: string[];
        ccAddresses: string[];
        bccAddresses: string[];
        fromAddress: string;
        fromName: string;
        subject: string;
        attachments: KeyValuePair<string, number[]>[];
        emailTemplateId: number;
    }
