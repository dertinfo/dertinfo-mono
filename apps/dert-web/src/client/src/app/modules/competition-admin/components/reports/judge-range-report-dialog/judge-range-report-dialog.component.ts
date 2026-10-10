import { Component, OnDestroy, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DashboardConductor } from 'app/modules/dashboard/services/dashboard.conductor';

@Component({
    selector: 'app-judge-range-report-dialog',
    templateUrl: './judge-range-report-dialog.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class JudgeRangeReportDialogComponent {

  constructor(
    private composeDialog: MatDialog,
  ) {
  }

}
