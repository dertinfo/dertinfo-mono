import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-action-confirm-navigate',
  templateUrl: './action-confirm-navigate.component.html',
  styleUrls: ['./action-confirm-navigate.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ActionConfirmNavigateComponent implements OnInit, OnChanges {

  private _postCompleteTimeout = 2000;

  public isNormal: boolean = true;
  public isConfirming: boolean = false;
  public isActioning: boolean = false;
  public isActioned: boolean = false;
  public selectedOption: string = null;
  public get titleClass() { return `mat-bg-${this.colour} m-0`; }

  @Input() title: string;
  @Input() actionName: string;
  @Input() complete: boolean = false;
  @Input() navigateTo: string;
  @Input() colour: string = 'warn';
  @Input() options: Array<string>;

  @Output() confirmed: EventEmitter<any> = new EventEmitter();

  constructor(private router: Router) { }

  ngOnInit() {
    if (this.options != null) { this.selectedOption = this.options[0]; }
  }

  ngOnChanges() {

    if (this.complete) {
      this.actionCompleted();
    }
  }

  onActionClick() {
    this.isNormal = false;
    this.isConfirming = true;
  }

  onConfirmClick() {
    this.isConfirming = false;
    this.isActioning = true;

    this.confirmed.emit(this.selectedOption);
  }

  onBackClick() {
    this.isNormal = true;
    this.isConfirming = false;
  }

  private actionCompleted() {
    this.isActioning = false;
    this.isActioned = true;

    setTimeout(() => {
      this.navigateAway();
    }, this._postCompleteTimeout);
  }

  private navigateAway() {
    if (this.navigateTo !== '' && this.navigateTo != null) {
      this.router.navigate([this.navigateTo]);
    }
  }
}
