import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-markingcriteriasidebar',
  templateUrl: './markingcriteriasidebar.component.html',
  styleUrls: ['./markingcriteriasidebar.component.scss']
})
export class MarkingCriteriaSideBarComponent implements OnInit {

  public panelOpenState = [true, false, false, false, false, false, false];

  constructor() { }

  ngOnInit() {
  }

  public openPanel(panelIndex: number) {
    for (let index = 0; index < this.panelOpenState.length; index++) {
      // Close all others
      index === panelIndex ? this.panelOpenState[index] = true : this.panelOpenState[index] = false;
    }
  }

}
