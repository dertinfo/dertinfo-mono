import { AfterViewInit, Component, Input, OnChanges, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';

import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CompetitionEntryAttributeDto, DanceDetailDto, DanceScoreDto } from 'app/models/dto';

class DataTableDataElement {
    public teamName: string;
    public venueName: string;
    // plus dynamic columns

}
@Component({
    selector: 'app-groupview-registration-dances-grid',
    templateUrl: './dances-grid.component.html',
    styleUrls: ['./dances-grid.component.css']
})
export class DancesGridComponent implements OnInit, OnChanges, OnDestroy, AfterViewInit {

    private _subscriptions: Subscription[] = [];

    @Input() danceResults: Array<DanceDetailDto>;
    @ViewChild(MatSort, { static: true }) sort: MatSort;

    public teamFilter = null;
    public venueFilter = null;
    public teamFilterItems = [];
    public venueFilterItems = [];
    public entryAttributeFilterData: Array<CompetitionEntryAttributeDto> = [];
    public collatedColumns = [];
    public collatedDisplayedColumns: string[] = ['venueName'];
    public dancesDataSource = new MatTableDataSource<DataTableDataElement>();

    constructor() { }

    ngOnInit() {
        this.loadOrRefreshComponent();
    }

    ngAfterViewInit() {
        this.dancesDataSource.sort = this.sort;
    }

    ngOnChanges() {
        this.loadOrRefreshComponent();
    }

    ngOnDestroy() {
        this._subscriptions.map((sub) => { sub.unsubscribe(); });
        this._subscriptions = []; // For good measure
    }

    onAttributeFilterChange($event) {

        const filterId = $event.value.toString();
        if (filterId > 0) {
            this.dancesDataSource.filter = filterId;
        } else {
            this.dancesDataSource.filter = null;
        }
    }

    private loadOrRefreshComponent() {

        if (this.danceResults && this.danceResults.length > 0) {

            this.processData(this.danceResults);

            this.dancesDataSource.filterPredicate = (data: DataTableDataElement, filter: string) => {

                return data.teamName === filter || data.venueName === filter || (data.teamName + data.venueName) === filter;
            };

            this.collatedDisplayedColumns = ['venueName', ...this.collatedColumns.map(c => c.columnDef), 'scoreTotal', 'timeScoresEntered'];

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
