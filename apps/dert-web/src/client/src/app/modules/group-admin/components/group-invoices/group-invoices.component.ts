import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { GroupInvoiceDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { GroupAdminConductor } from '../../services/group-admin.conductor';
import { GroupAdminTracker } from '../../services/group-admin.tracker';

@Component({
  selector: 'app-group-invoices',
  templateUrl: './group-invoices.component.html',
  styleUrls: ['./group-invoices.component.css']
})
export class GroupInvoicesComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public groupInvoices$;

  public selectedInvoice: GroupInvoiceDto = null;

  public structuredNotes: any = null;

  public structuredLines: any = null;

  public collatedLines: any[] = [];

  constructor(

    public composeDialog: MatDialog,
    private router: ActivatedRoute,
    private _conductor: GroupAdminConductor,
    private _tracker: GroupAdminTracker
  ) { }

  ngOnInit() {

    this.groupInvoices$ = this._tracker.groupInvoices$;

    this._subscriptions.push(this.router.parent.params.subscribe(params => {
      this._conductor.initInvoices(params['id']);
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public OnInvoiceSelected(invoice: GroupInvoiceDto) {

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

    this.selectedInvoice = invoice;
  }
}
