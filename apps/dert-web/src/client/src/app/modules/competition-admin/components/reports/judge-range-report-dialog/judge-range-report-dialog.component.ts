import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { DashboardConductor } from 'app/modules/dashboard/services/dashboard.conductor';

@Component({
  selector: 'app-judge-range-report-dialog',
  templateUrl: './judge-range-report-dialog.component.html'
})
export class JudgeRangeReportDialogComponent {

  constructor(
    private composeDialog: MatDialog,
  ) {
  }

}
