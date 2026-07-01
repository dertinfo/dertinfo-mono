import { InvoiceDto } from './InvoiceDto';

    export interface EventInvoiceDto extends InvoiceDto {
        currentRegistrationTotal: number;
    }
