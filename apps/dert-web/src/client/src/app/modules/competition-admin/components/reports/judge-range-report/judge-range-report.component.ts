import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { JudgeDto, RangeReportDto } from 'app/models/dto';
import { CompetitionRepository } from 'app/modules/repositories';
import { CompetitionAdminConductor } from 'app/modules/competition-admin/services/competition-admin.conductor';
import { CompetitionAdminTracker } from 'app/modules/competition-admin/services/competition-admin.tracker';
import { SubscriptionLike } from 'rxjs';
import { JudgeRangeReportDialogComponent } from '../judge-range-report-dialog/judge-range-report-dialog.component';

@Component({
    selector: 'app-competition-judge-range-report',
    templateUrl: './judge-range-report.component.html',
    styleUrls: ['./judge-range-report.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class JudgeRangeReportComponent implements OnInit {

  private _subscriptions: Array<SubscriptionLike> = [];

  public competitionJudges: Array<JudgeDto> = [];

  public dataAvailable: boolean = false;
  public rangeReportJudgeSelected: JudgeDto;
  public rangeReportChartData: Array<any> = [];
  public rangeReportChartLabels: Array<any> = [];

  sharedChartOptions: any = {
    responsive: true,
    plugins: {
      legend: {
        display: true,
        position: 'bottom'
      }
    }
  };

  rangeReportChartOptions: any = Object.assign({
    scales: {
      x: {
        title: {
          display: true,
          text: 'mark given'
        },
        grid: {
          color: 'rgba(0,0,0,0.02)'
        }
      },
      y1: {
        type: 'linear',
        position: 'left',
        title: {
          display: true,
          text: 'number of occurrences of mark'
        },
        grid: {
          color: 'rgba(0,0,0,0.02)'
        },
        beginAtZero: true,
        suggestedMax: 9
      },
      y2: {
        type: 'linear',
        position: 'right',
        title: {
          display: true,
          text: 'number of occurrences of mark (combined)'
        },
        beginAtZero: true,
        suggestedMax: 20,
        grid: {
          color: 'rgba(0,0,0,0)',
          drawOnChartArea: false
        }
      }
    }
  }, this.sharedChartOptions);

  public rangeReportChartLegend: boolean = true;
  public rangeReportChartType: string = 'line';

  constructor(
    private _competitionRepo: CompetitionRepository,
    private _tracker: CompetitionAdminTracker,
    private _conductor: CompetitionAdminConductor,
    public composeDialog: MatDialog,
  ) { }

  ngOnInit() {

    this._subscriptions.push(this._tracker.judges$.subscribe((judges: Array<JudgeDto>) => {
      this.competitionJudges = judges;
    }));

    this._conductor.initJudges(this._tracker.competitionId);
  }

  public onRangeReportJudgeSelectorChanged() {

    // Clear the data available
    if (this.rangeReportJudgeSelected) {
      this.getReportData(this._tracker.competitionId, this.rangeReportJudgeSelected.id);
    } else {
      this.dataAvailable = false;
    }
  }

  public onRangeReportInfoClick() {
    const eventDialogRef = this.composeDialog.open(JudgeRangeReportDialogComponent);
    const dialogueSubscription = eventDialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  private getReportData(competitionId: number, judgeId: number) {
    const subs = this._competitionRepo.getJudgeRangeReport(competitionId, judgeId).subscribe((rangeReportDto: RangeReportDto) => {
      subs.unsubscribe();

      if (rangeReportDto.scoreRanges && rangeReportDto.scoreRanges.length > 0) {
        this.buildRangeReportLabelData();
        this.buildRangeReportData(rangeReportDto);
        this.dataAvailable = true;
      } else {
        this.dataAvailable = false;
      }

    });
  }

  private buildRangeReportLabelData(): void {
    this.rangeReportChartLabels = [];
    for (let i = 0; i < 16; i++) {
      this.rangeReportChartLabels.push(i.toString());
    }
  }

  private buildRangeReportData(rangeReportDto: RangeReportDto): void {
    this.rangeReportChartData = [];
    const rangeReportCumulativeRangeValues: Array<number> = [];

    rangeReportDto.scoreRanges.map((scoreRange) => {

      const noResults = this.checkIfAllZero(scoreRange.rangeValues);

      if (!noResults) {
        rangeReportCumulativeRangeValues.push(...scoreRange.rangeValues);

        this.rangeReportChartData.push({
          data: this.collateScoreRange(scoreRange.rangeValues),
          label: scoreRange.rangeTag,
          borderWidth: 1,
          yAxisID: 'y1'
        });
      }
    });

    if (rangeReportCumulativeRangeValues.length > 0) {
      this.rangeReportChartData.push({
        data: this.collateScoreRange(rangeReportCumulativeRangeValues),
        label: 'Combined',
        borderWidth: 1,
        borderColor: 'rgba(0,0,0,0)',
        yAxisID: 'y2',
        type: 'line',
        fill: false,
        tension: 0,
        pointStyle: 'rect',
        pointBorderColor: '#000',
        pointBackgroundColor: '#000'
      });
    }
  }

  private checkIfAllZero(scoresGiven: Array<number>): boolean {
    const sum = scoresGiven.reduce((total, value) => { return total + value; });
    return sum === 0;
  }

  private collateScoreRange(scoresGiven: Array<number>): Array<number> {

    const outputArray = [];

    const countTally = scoresGiven.reduce((tally, value) => {
      tally[value] = (tally[value] || 0) + 1;
      return tally;
    }, {});

    for (let i = 0; i < 16; i++) {
      const instancesOf = countTally[i] ? countTally[i] : 0;
      outputArray.push(instancesOf);
    }

    return outputArray;
  }

}
