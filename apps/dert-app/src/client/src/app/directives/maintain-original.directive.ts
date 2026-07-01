import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: '[maintain-original]'
})
export class MaintainOriginalDirective {

    private _preservedVal: string;

    constructor(private el: ElementRef) {

    }

    @HostListener('focusin') onFocus() {
        this.focus();
    }

    @HostListener('focusout') onFocusOut() {
        this.focusOut();
    }

    private focus() {
        this.el.nativeElement.style.backgroundColor = '#8FBE36';
        this._preservedVal = this.el.nativeElement.children[0].value;
        this.el.nativeElement.children[0].value = '';
    }

    private focusOut() {
        this.el.nativeElement.style.backgroundColor = null;
        if (this.el.nativeElement.children[0].value === '') {
            this.el.nativeElement.children[0].value = this._preservedVal;
        }
    }
}
