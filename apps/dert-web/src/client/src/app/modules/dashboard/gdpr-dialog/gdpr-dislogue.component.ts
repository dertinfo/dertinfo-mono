import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { DashboardConductor } from 'app/modules/dashboard/services/dashboard.conductor';

@Component({
  selector: 'app-gdpr-dialog',
  templateUrl: './gdpr-dialog.component.html'
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
