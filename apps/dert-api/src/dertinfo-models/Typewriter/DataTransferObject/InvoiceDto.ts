
    export interface InvoiceDto {
        invoiceId: number;
        invoiceCode: string;
        registrationId: number;
        invoiceToName: string;
        invoiceToTeamName: string;
        invoiceToEmail: string;
        invoiceTotal: number;
        invoiceLineItemNotes: string;
        invoiceEntryNotes: string;
        hasPaid: boolean;
        title: string;
        imageResourceUri: string;
        hasStructuredNotes: boolean;
        dateCreated: Date;
    }
