import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { EventInvoiceDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';

import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
  selector: 'app-event-invoices',
  templateUrl: './event-invoices.component.html',
  styleUrls: ['./event-invoices.component.css']
})
export class EventInvoicesComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public eventInvoices$;

  public invoiceHistory$;

  public selectedInvoice: EventInvoiceDto = null;

  public selectedInvoiceHistory: EventInvoiceDto = null;

  public invoiceDetailToDisplay: EventInvoiceDto = null;

  public structuredNotes: any = null;

  public structuredLines: any = null;

  public collatedLines: any[] = [];

  constructor(
    public composeDialog: MatDialog,
    private router: ActivatedRoute,
    private _conductor: EventAdminConductor,
    private _tracker: EventAdminTracker
  ) { }

  ngOnInit() {

    this.eventInvoices$ = this._tracker.eventInvoices$;
    this.invoiceHistory$ = this._tracker.focussedInvoiceHistory$;

    this._subscriptions.push(this.router.parent.params.subscribe(params => {
      this._conductor.initInvoices(params['id']);
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public OnInvoicePaidToggled(invoiceId: number) {
    this._conductor.toggleInvoicePaid(invoiceId);
  }

  public OnInvoiceSelected(invoice: EventInvoiceDto) {
    this._conductor.initFocussedInvoice(invoice.invoiceId);

    this.selectedInvoiceHistory = null;
    this.InvoiceSelected(invoice);
  }

  public OnInvoiceHistoryItemSelected(invoice: EventInvoiceDto) {
    this.InvoiceHistorySelected(invoice);
  }

  private InvoiceHistorySelected(invoice: EventInvoiceDto) {
    this.selectedInvoiceHistory = invoice;

    this.processInvoiceDetailDisplay(invoice);

  }

  private InvoiceSelected(invoice: EventInvoiceDto) {
    this.selectedInvoice = invoice;

    this.processInvoiceDetailDisplay(invoice);
  }

  private processInvoiceDetailDisplay(invoice: EventInvoiceDto) {

    this.invoiceDetailToDisplay = invoice;

    if (invoice.hasStructuredNotes) {
      this.structuredNotes = JSON.parse(invoice.invoiceEntryNotes);
      this.structuredLines = JSON.parse(invoice.invoiceLineItemNotes);

      const lines = [];
      if (
        this.structuredLines.LinesDetailStorage.collatedIndividualLineItems
        && this.structuredLines.LinesDetailStorage.collatedIndividualLineItems.FlatLine.length > 0
      ) {
        this.structuredLines.LinesDetailStorage.collatedIndividualLineItems.FlatLine.map((iLine) => {
          lines.push(iLine);
        });
      } else {
        if (
          this.structuredLines.LinesDetailStorage.collatedIndividualLineItems &&
          this.structuredLines.LinesDetailStorage.collatedIndividualLineItems.FlatLine
        ) {
          lines.push(this.structuredLines.LinesDetailStorage.collatedIndividualLineItems.FlatLine);
        }
      }

      if (
        this.structuredLines.LinesDetailStorage.collatedTeamLineItems &&
        this.structuredLines.LinesDetailStorage.collatedTeamLineItems.FlatLine.length > 0
      ) {
        this.structuredLines.LinesDetailStorage.collatedTeamLineItems.FlatLine.map((iLine) => {
          lines.push(iLine);
        });
      } else {
        if (
          this.structuredLines.LinesDetailStorage.collatedTeamLineItems &&
          this.structuredLines.LinesDetailStorage.collatedTeamLineItems.FlatLine
        ) {
          lines.push(this.structuredLines.LinesDetailStorage.collatedTeamLineItems.FlatLine);
        }
      }

      this.collatedLines = lines;
    }
  }
}
