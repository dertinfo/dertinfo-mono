import { AfterViewInit, Component, Input, OnChanges, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AngularCsv } from 'angular-csv-ext/dist/Angular-csv';
import { Subscription } from 'rxjs';

import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CompetitionEntryAttributeDto, TeamCollatedFullResultDto } from 'app/models/dto';

class DataTableDataElement {
    public teamName: string;
    public counts: any;
    public attributes: Array<CompetitionEntryAttributeDto>;

}
@Component({
    selector: 'app-competition-results-collated-grid',
    templateUrl: './collated-grid.component.html',
    styleUrls: ['./collated-grid.component.css']
})
export class CollatedGridComponent implements OnInit, OnChanges, OnDestroy, AfterViewInit {

    private _subscriptions: Subscription[] = [];

    @Input() collatedResults: Array<TeamCollatedFullResultDto>;

    @Input() competitionName: string;

    @ViewChild(MatSort, { static: true }) sort: MatSort;

    public awardFilter: number;
    public entryAttributeFilterData: Array<CompetitionEntryAttributeDto> = [];

    public collatedColumns = [];
    public collatedDisplayedColumns: string[] = ['teamName'];
    public collatedDataSource = new MatTableDataSource<DataTableDataElement>();

    constructor(

    ) { }

    ngOnInit() {
        this.loadOrRefreshComponent();
    }

    ngOnChanges() {
        this.loadOrRefreshComponent();
    }

    ngAfterViewInit() {

        this.collatedDataSource.sort = this.sort;
    }

    ngOnDestroy() {
        this._subscriptions.map((sub) => { sub.unsubscribe(); });
        this._subscriptions = []; // For good measure
    }

    public onAttributeFilterChange() {

        if (this.awardFilter > 0) {
            this.collatedDataSource.filter = this.awardFilter.toString();
        } else {
            this.collatedDataSource.filter = null;
        }
    }

    downloadCSV() {

        const headers = ['Team Name', 'Attributes'];

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
        if ( this.collatedResults.length > 0 ) {
            // Add the score category headers
            for (const key of Object.keys(this.collatedResults[0].scoreGroupResults)) {
                headers.push(key);
            }
            headers.push('counts');
        }

        this.collatedResults.forEach(collatedResult => {

            const row = {
                teamName: collatedResult.teamName,
                teamEntryAttributes: collatedResult.teamEntryAttributes.map(tea => tea.tag).join(','),
            };

            for (const key of Object.keys(collatedResult.scoreGroupResults)) {
                row[key] = collatedResult.scoreGroupResults[key].collatedScore;
            }

            row['counts'] = `${collatedResult.danceEnteredCount} of ${collatedResult.danceTotalCount}`;

            flattened.push(row);
        });

        const downloadName = this.competitionName != null ? `${this.competitionName}Results` : 'Results';
        const csv = new AngularCsv(flattened, downloadName, csvOptions);
        return csv;
    }

    private loadOrRefreshComponent() {

        if (this.collatedResults) {

            const flattenedData = this.processCollatedResults(this.collatedResults);

            this.collatedDataSource.filterPredicate = (data: DataTableDataElement, filter: string) =>
                data.attributes.findIndex(x => x.id === parseInt(filter, 10)) !== -1;

            this.collatedDisplayedColumns = ['teamName', ...this.collatedColumns.map(c => c.columnDef), 'counts'];

            this.collatedDataSource.sort = this.sort;
        }
    }

    private processCollatedResults(results: Array<TeamCollatedFullResultDto>) {

        const collatedColumns = this.processCollatedColumns(results[0].scoreGroupResults);

        // Apply the dynamic columns
        this.collatedColumns = collatedColumns;

        const flattenedData = this.flattenCollatedData(results, collatedColumns);

        this.collatedDataSource = new MatTableDataSource<any>(flattenedData);
    }

    private flattenCollatedData(results: TeamCollatedFullResultDto[], collatedColumns): any {

        let distinctEntryAttributes = [];

        // Iterate over the results
        const flattenedData = results.map((result) => {

            // Get the attributes for the team to build up the filter
            distinctEntryAttributes = this.processEntryAttributes(result.teamEntryAttributes, distinctEntryAttributes);

            // Produce the flatened result
            const flatResult = {
                teamName: result.teamName,
                counts: {
                    danceEnteredCount: result.danceEnteredCount,
                    danceTotalCount: result.danceTotalCount
                },
                attributes: result.teamEntryAttributes
            };

            // Pin the dynamic categories to the flattened result
            collatedColumns.map((col) => {
                flatResult[col.header] = result.scoreGroupResults[col.header].collatedScore;
            });

            // Return the flattened result
            return flatResult;
        });

        // Apply the filters
        this.entryAttributeFilterData = [...distinctEntryAttributes];

        return flattenedData;
    }

    private processEntryAttributes(
        teamEntryAttributes: CompetitionEntryAttributeDto[],
        currentAttributeSet: CompetitionEntryAttributeDto[]
    ): CompetitionEntryAttributeDto[] {

        teamEntryAttributes.map((att) => {
            const index = currentAttributeSet.findIndex(x => x.id === att.id);
            if (index === -1) {
                currentAttributeSet.push(att);
            }
        });

        return currentAttributeSet;
    }

    private processCollatedColumns(resultEntry) {

        const keys = Object.keys(resultEntry);
        const allEntryAttributes: Array<CompetitionEntryAttributeDto> = [];

        return keys.map((key) => {
            return {
                columnDef: key, header: key, cell: (element: any) => {
                    return `${element[key]}`;
                }
            };
        });
    }
}
