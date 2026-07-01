import { AfterViewInit, Component, ElementRef, Input, OnChanges, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-fold',
  templateUrl: './fold.component.html',
  styleUrls: ['./fold.component.scss']
})
export class FoldComponent implements OnInit, AfterViewInit, OnChanges {

  private _baseHeight: number = 200;
  private _appliedHeight: number = null;
  private _expandTimeout: number = 1000; // Must match transition.
  private _foldTextContracted = 'view more';
  private _foldTextExpanded = 'view less';

  /**
   * We accept an input of a content hash in order that when the conponent is displayed on the page and the
   * ngContnet changes the inputs of the desired height wouldn't change. This hash change forces the ngOnChanges to fire
   * and threfore appropraitely evaluate if the expand content is needed.
   */
  @Input() contentHash = null;
  @Input() foldHeight = 100;
  @ViewChild('content') elementView: ElementRef;

  public isExpanded = false;
  public foldText = this._foldTextContracted;
  public allowFold = false;

  public get foldStyle(): string {
    return this._appliedHeight ? `max-height:${this._appliedHeight}px` : '';
  }

  constructor() { }

  public ngOnInit() {
    if (this.foldHeight) {
      this._appliedHeight = this.foldHeight;
      this._baseHeight = this.foldHeight;
    }
  }

  public ngAfterViewInit() {
    this.checkHeights();
  }

  public ngOnChanges() {
    this.checkHeights();
  }

  public onViewMoreClick() {

    if (!this.isExpanded) {
      this.expandContent();
    } else {
      this.contractContent();
    }
  }

  private checkHeights() {
    setTimeout(() => {
      const contentHeight = this.elementView.nativeElement.clientHeight;
      this.allowFold = contentHeight > this._baseHeight;
    }, 0);
  }

  private expandContent() {
    const contentHeight = this.elementView.nativeElement.clientHeight;
    this._appliedHeight = contentHeight;
    setTimeout(() => {
      this.isExpanded = true;
      this.foldText = this._foldTextExpanded;
    }, this._expandTimeout);
  }

  private contractContent() {
    this._appliedHeight = this.foldHeight; // Shrink

    setTimeout(() => {
      this.isExpanded = false;
      this.foldText = this._foldTextContracted;
    }, this._expandTimeout);
  }
}
