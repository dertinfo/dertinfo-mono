import { Directive, ElementRef, Input, OnChanges, Renderer2, SimpleChanges } from '@angular/core';

/**
 * This is an Attribute Directive. It augments the functionality of another element.
 * To use this use:
 * <span [price]="selectedInvoice.invoiceTotal" [tbc]="false">{{selectedInvoice.invoiceTotal}}</span>
 * To Register this:
 * Import the module that exports it.
 */
@Directive({
    selector: '[price]'
})
export class PriceDirective implements OnChanges {

    @Input() price: number;
    @Input() tbc: boolean;

    constructor(private el: ElementRef, private renderer: Renderer2) {
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.price) {
            if (this.tbc) {
                this.renderer.setProperty(this.el.nativeElement, 'innerHTML', 'TBC');
            }

            if (this.price && !this.tbc) {
                this.renderer.setProperty(this.el.nativeElement, 'innerHTML',  `£${this.price}`);
            }
        }
    }
}
