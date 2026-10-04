import { Directive, ElementRef, EventEmitter, HostBinding, HostListener, Input, OnInit, Output } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import * as domHelper from '../../helpers/dom.helper';

@Directive({
    selector: '[sideNavAccordion]',
    standalone: false
})
export class SideNavAccordionDirective implements OnInit {
  constructor(private el: ElementRef) {
  }
  ngOnInit() {
    const self = this;
    setTimeout(() => {
      if (self.el.nativeElement.querySelector('.sub-menu')) {
        self.el.nativeElement.classList.add('has-submenu');
      }
    });
    const isCollapsed = domHelper.hasClass(document.body, 'collapsed-menu');

    // remove open class that is added my router
    if (isCollapsed) {
      setTimeout(() => {
        domHelper.removeClass(self.el.nativeElement, 'open');
      });
    }
  }

  @HostListener('click', ['$event'])
  onClick($event) {
    const parentLi = $event.target && $event.target.closest
      ? $event.target.closest('mat-list-item')
      : null;
    if (!parentLi || !parentLi.querySelector('.sub-menu')) {
      // PREVENTS CLOSING PARENT ITEM
      return;
    }
    this.toggleOpen();
  }

  // For collapsed sidebar
  @HostListener('mouseenter', ['$event'])
  onMouseEnter($event) {
    const elem = this.el.nativeElement;
    const isCollapsed = domHelper.hasClass(document.body, 'collapsed-menu');
    if (!isCollapsed) {
      return;
    }
    domHelper.addClass(elem, 'open');
  }
  @HostListener('mouseleave', ['$event'])
  onMouseLeave($event) {
    const elem = this.el.nativeElement;
    const isCollapsed = domHelper.hasClass(document.body, 'collapsed-menu');
    if (!isCollapsed) {
      return;
    }
    domHelper.removeClass(elem, 'open');
  }

  private toggleOpen() {
    const elem = this.el.nativeElement;
    const parenMenuItems = document.getElementsByClassName('has-submenu');

    if (domHelper.hasClass(elem, 'open')) {
      domHelper.removeClass(parenMenuItems, 'open');

    } else {
      domHelper.removeClass(parenMenuItems, 'open');
      domHelper.addClass(elem, 'open');
    }
  }

}
