import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: '[maintain-original]'
})
export class MaintainOriginalDirective {

    private _preservedVal: string;

    constructor(private el: ElementRef) {

    }

    @HostListener('focus') onFocus() {
        this.focus();
    }

    @HostListener('focusout') onFocusOut() {
        this.focusOut();
    }

    private focus() {
        this._preservedVal = this.el.nativeElement.value;
        this.el.nativeElement.value = '';
    }

    private focusOut() {
        this.el.nativeElement.style.backgroundColor = null;
        if (this.el.nativeElement.value === '') {
            this.el.nativeElement.value = this._preservedVal;
        }
    }
}
