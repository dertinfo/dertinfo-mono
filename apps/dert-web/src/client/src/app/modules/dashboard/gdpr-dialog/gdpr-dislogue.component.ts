import { Component, OnDestroy, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DashboardConductor } from 'app/modules/dashboard/services/dashboard.conductor';

@Component({
    selector: 'app-gdpr-dialog',
    templateUrl: './gdpr-dialog.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class GdprDialogComponent implements OnInit, OnDestroy {

  constructor(
    private composeDialog: MatDialog,
    private dashboardConductor: DashboardConductor,
  ) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  onGdprDialogOk() {
    this.dashboardConductor.gdprAcceptanceGained();
    this.composeDialog.closeAll();
  }
}
