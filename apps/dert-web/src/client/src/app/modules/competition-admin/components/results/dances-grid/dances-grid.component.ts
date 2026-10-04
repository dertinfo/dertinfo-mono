import { AfterViewInit, Component, Input, OnChanges, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AngularCsv } from 'angular-csv-ext/dist/Angular-csv';
import { Subscription } from 'rxjs';

import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CompetitionEntryAttributeDto, DanceDetailDto, DanceScoreDto, TeamCollatedFullResultDto } from 'app/models/dto';

class DataTableDataElement {
    public teamName: string;
    public venueName: string;
    // plus dynamic columns

}
@Component({
    selector: 'app-competition-results-dances-grid',
    templateUrl: './dances-grid.component.html',
    styleUrls: ['./dances-grid.component.css']
})
export class DancesGridComponent implements OnInit, OnChanges, OnDestroy, AfterViewInit {

    private _subscriptions: Subscription[] = [];

    @Input() danceResults: Array<DanceDetailDto>;

    @Input() competitionName: string;

    @ViewChild(MatSort, { static: true }) sort: MatSort;

    public teamFilter = null;
    public venueFilter = null;

    public teamFilterItems = [];
    public venueFilterItems = [];

    public entryAttributeFilterData: Array<CompetitionEntryAttributeDto> = [];

    public collatedColumns = [];
    public collatedDisplayedColumns: string[] = ['teamName'];
    public dancesDataSource = new MatTableDataSource<DataTableDataElement>();

    constructor(

    ) { }

    ngOnInit() {
        this.loadOrRefreshComponent();
    }

    ngAfterViewInit() {
        this.dancesDataSource.sort = this.sort;
    }

    ngOnChanges() {
        this.loadOrRefreshComponent();
    }

    public onTeamFilterChanged() {
        this.applyFilter();
    }

    public onVenueFilterChanged() {
        this.applyFilter();
    }

    public applyFilter() {
        if (this.teamFilter || this.venueFilter) {
            const teamFilterStr = this.teamFilter ? this.teamFilter : '';
            const venueFilterStr = this.venueFilter ? this.venueFilter : '';
            this.dancesDataSource.filter = teamFilterStr + venueFilterStr;
        } else {
            this.dancesDataSource.filter = null;
        }
    }

    ngOnDestroy() {
        this._subscriptions.map((sub) => { sub.unsubscribe(); });
        this._subscriptions = []; // For good measure
    }

    public onAttributeFilterChange($event) {

        const filterId = $event.value.toString();
        if (filterId > 0) {
            this.dancesDataSource.filter = filterId;
        } else {
            this.dancesDataSource.filter = null;
        }
    }

    downloadCSV() {

        const headers = ['Team Name'];

        const csvOptions  = {
            fieldSeparator: ',',
            quoteStrings: '"',
            decimalseparator: '.',
            showLabels: true,
            showTitle: false,
            title: '',
            useBom: true,
            noDownload: false,
            headers: headers
        };

        const flattened = [];
        if ( this.danceResults.length > 0 ) {
            // Add the score category headers
            for (const tag of this.danceResults[0].danceScores.map(ds => ds.scoreCategoryTag)) {
                headers.push(tag);
            }

            headers.push('all');
            headers.push('timeEntered');
        }

        this.danceResults.forEach(danceResult => {

            const sumOfScores = danceResult.danceScores
            .map(ds => ds.markGiven)
            .reduce(function (a, b) {
                return a + b;
              }, 0);

            const row = {
                teamName: danceResult.teamName,
            };

            for (const score of danceResult.danceScores) {
                row[score.scoreCategoryTag] = score.markGiven;
            }

            row['all'] = sumOfScores;
            row['timeEntered'] = this.extractTime(danceResult.dateScoresEntered)

            flattened.push(row);
        });

        const downloadName = this.competitionName != null ? `${this.competitionName}Results` : 'Results';
        const csv = new AngularCsv(flattened, downloadName, csvOptions);
        return csv;
    }

    private loadOrRefreshComponent() {

        if (this.danceResults) {

            const flattenedData = this.processData(this.danceResults);

            this.dancesDataSource.filterPredicate = (data: DataTableDataElement, filter: string) => {

                return data.teamName === filter || data.venueName === filter || (data.teamName + data.venueName) === filter;
            };

            this.collatedDisplayedColumns = ['teamName', ...this.collatedColumns.map(c => c.columnDef), 'scoreTotal', 'timeScoresEntered'];

            this.dancesDataSource.sort = this.sort;
        }
    }

    private processData(results: Array<DanceDetailDto>) {

        const collatedColumns = this.processCollatedColumns(results[0].danceScores);

        // Apply the dynamic columns
        this.collatedColumns = collatedColumns;

        const flattenedData = this.flattenDataSet(results, collatedColumns);

        this.dancesDataSource = new MatTableDataSource<DataTableDataElement>(flattenedData);
    }

    private flattenDataSet(results: DanceDetailDto[], dynamicColumns: any): any {

        // Iterate over the results
        const flattenedData = results.map((result) => {

            // Build The Team & Venue Filter Items
            if (!this.teamFilterItems.some(tfi => tfi.teamId === result.teamId)) {
                this.teamFilterItems.push({ teamId: result.teamId, teamName: result.teamName });
            }

            if (!this.venueFilterItems.some(vfi => vfi.venueId === result.venueId)) {
                this.venueFilterItems.push({ venueId: result.venueId, venueName: result.venueName });
            }

            // Produce the flatened result
            const myReducer = (accumulator: number, currentValue: DanceScoreDto) => { return accumulator + currentValue.markGiven; };
            const flatResult = {
                teamName: result.teamName,
                venueName: result.venueName,
                scoreTotal: result.danceScores.reduce(myReducer, 0),
                timeScoresEntered: this.extractTime(result.dateScoresEntered)
            };

            // Pin the dynamic categories to the flattened result
            dynamicColumns.map((col) => {
                flatResult[col.header] = result.danceScores.find(ds => ds.scoreCategoryTag === col.columnDef).markGiven;
            });

            // Return the flattened result
            return flatResult;
        });

        return flattenedData;
    }

    private extractTime(date: Date): string {
        if (date) {
            const myDate = new Date(date);
            const hours = myDate.getHours();
            const mins = myDate.getMinutes();
            const paddedHours = hours.toString().length === 2 ? hours.toString() : '0' + hours.toString();
            const paddedMins = mins.toString().length === 2 ? mins.toString() : '0' + mins.toString();
            return `${paddedHours}:${paddedMins}`;
        } else {
            return '';
        }

    }

    private processCollatedColumns(danceScores: DanceScoreDto[]) {

        return danceScores.map((ds) => {
            return {
                columnDef: ds.scoreCategoryTag, header: ds.scoreCategoryTag, cell: (element: any) => {
                    return `${element[ds.scoreCategoryTag]}`;
                }
            };
        });
    }
}
