import { Component, EventEmitter, Input, Output } from '@angular/core';
import { GroupInvoiceDto } from 'app/models/dto';

@Component({
    selector: 'app-invoices-listing',
    templateUrl: './invoices-listing.component.html',
    styleUrls: ['./invoices-listing.component.css']
})
export class InvoiceListingComponent {

    @Input() invoices: GroupInvoiceDto[] = [];

    @Output() invoiceSelected: EventEmitter<GroupInvoiceDto> = new EventEmitter();

    constructor() {
    }

    public OnInvoiceRowClick(invoice: GroupInvoiceDto) {

        this.invoiceSelected.emit(invoice);
    }
}
