import { Component, EventEmitter, Input, Output } from '@angular/core';
import { EventInvoiceDto } from 'app/models/dto';

@Component({
    selector: 'app-event-invoices-listing',
    templateUrl: './event-invoices-listing.component.html',
    styleUrls: ['./event-invoices-listing.component.css']
})
export class EventInvoiceListingComponent {

    @Input() invoices: EventInvoiceDto[] = [];
    @Input() selectedInvoice: EventInvoiceDto = null;
    @Input() compareEnabled: boolean = false;
    @Input() changeStateEnabled: boolean = false;

    @Output() invoiceSelected: EventEmitter<EventInvoiceDto> = new EventEmitter();

    @Output() invoicePaidToggled: EventEmitter<number> = new EventEmitter();

    constructor() {
    }

    public OnInvoiceRowClick(invoice: EventInvoiceDto) {

        this.invoiceSelected.emit(invoice);
    }

    public OnHasPaidClick(invoiceId: number) {

        this.invoicePaidToggled.emit(invoiceId);
    }
}
