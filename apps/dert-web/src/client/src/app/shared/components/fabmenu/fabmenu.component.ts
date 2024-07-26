import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NavigationService } from '../../../core/services/navigation.service';

@Component({
  selector: 'app-fabmenu',
  templateUrl: './fabmenu.component.html',
  styleUrls: ['./fabmenu.component.css']
})
export class FabMenuComponent implements OnInit {
  isFlyoutOpen = false;

  @Input() menuItems;
  @Output() menuItemClicked = new EventEmitter<string>();

  constructor(private navService: NavigationService) { }

  ngOnInit() { }

  toggleFlyout() {
    this.isFlyoutOpen = !this.isFlyoutOpen;
  }

  itemClicked(name: string) {
    this.menuItemClicked.emit(name);
    this.isFlyoutOpen = false;
  }

}
