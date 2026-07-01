
export interface EmailBaseDto {
    toAddresses: string[];
    ccAddresses: string[];
    bccAddresses: string[];
    fromAddress: string;
    fromName: string;
    subject: string;
    attachments: any;
    emailTemplateId: number;
}
