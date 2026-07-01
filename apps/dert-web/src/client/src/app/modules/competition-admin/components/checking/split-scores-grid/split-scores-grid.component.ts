import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

import { DanceDetailDto, DanceScorePartDto, JudgeSlotInformationDto } from 'app/models/dto';
import { Subscription } from 'rxjs';

class DataTableDataElement {
    public judgeName: string;
    // plus dynamic columns
}

@Component({
    selector: 'app-competition-checking-split-scores-grid',
    templateUrl: './split-scores-grid.component.html',
    styleUrls: ['./split-scores-grid.component.css']
})
export class SplitScoresGridComponent implements OnInit, OnChanges, OnDestroy, AfterViewInit {

    private _subscriptions: Subscription[] = [];

    @Input() judgeSlotInformation: Array<JudgeSlotInformationDto>;

    @Input() danceResult: DanceDetailDto;

    @Output() changed = new EventEmitter<DanceScorePartDto>();

    @ViewChild(MatSort, { static: true }) sort: MatSort;

    public collatedColumns = [];
    public collatedDisplayedColumns: string[] = ['judgeName'];
    public dancesSplitScoresDataSource = new MatTableDataSource<DataTableDataElement>();
    public splitScoresValid: boolean = false;

    constructor(

    ) { }

    ngOnInit() {
        this.loadOrRefreshComponent();
    }

    ngAfterViewInit() {
        this.dancesSplitScoresDataSource.sort = this.sort;
    }

    ngOnChanges() {
        this.loadOrRefreshComponent();
    }

    public onScorePartChanged(judgeSlotId: number, danceScoreId: number, $event: any) {

        const judgeSlot = this.judgeSlotInformation.find(js => js.judgeSlotId === judgeSlotId);
        const scorePart = judgeSlot.scoreParts.find(sp => sp.danceScoreId === danceScoreId);
        scorePart.scoreGiven = parseFloat($event.target.value);

        this.changed.emit(scorePart);

        this.splitScoresValid = this.checkValidityAgainstSuppliedScores();
    }

    ngOnDestroy() {
        this._subscriptions.map((sub) => { sub.unsubscribe(); });
        this._subscriptions = []; // For good measure
    }

    private checkValidityAgainstSuppliedScores(): boolean {

        let valid = true;
        this.danceResult.danceScores.forEach(ds => {

            const constuantScoreParts: Array<DanceScorePartDto> = [];
            this.judgeSlotInformation.map((jsi) => {
                const part = jsi.scoreParts.filter(sp => sp.scoreCategoryTag === ds.scoreCategoryTag);
                if (part && part.length === 1 && part[0].isPartOfScoreSet) { constuantScoreParts.push(part[0]); }
            });

            let constuantScorePartsTotal = 0;
            constuantScoreParts.map((part) => {
                constuantScorePartsTotal = constuantScorePartsTotal + Math.round(part.scoreGiven * 100) / 100;
            });

            const partsAverage = (constuantScorePartsTotal / constuantScoreParts.length);
            if (ds.markGiven !== Math.round(partsAverage * 100) / 100) {
                valid = false;
            }
        });

        return valid;
    }

    private loadOrRefreshComponent() {

        if (this.judgeSlotInformation) {

            const flattenedData = this.processData(this.judgeSlotInformation);

            this.collatedDisplayedColumns = ['judgeName', ...this.collatedColumns.map(c => c.columnDef)];

            this.dancesSplitScoresDataSource.sort = this.sort;
        }
    }

    private processData(results: Array<JudgeSlotInformationDto>) {

        const collatedColumns = this.processCollatedColumns(results[0].scoreParts);

        // Apply the dynamic columns
        this.collatedColumns = collatedColumns;

        const flattenedData = this.flattenDataSet(results, collatedColumns);

        this.dancesSplitScoresDataSource = new MatTableDataSource<DataTableDataElement>(flattenedData);
    }

    private flattenDataSet(results: JudgeSlotInformationDto[], dynamicColumns: any): any {

        // Iterate over the results
        const flattenedData = results.map((result) => {

            const flatResult = {
                judgeSlotId: result.judgeSlotId,
                judgeName: result.judgeName,
            };

            // Pin columns to it
            dynamicColumns.map((col) => {
                const info = result.scoreParts.find(sp => sp.scoreCategoryTag === col.columnDef);

                info.partsMatchScore = info.partsMatchScore || false;

                flatResult[col.header] = {
                    judgeSlotId: info.judgeSlotId,
                    danceScoreId: info.danceScoreId,
                    markGiven: info.scoreGiven,
                    isVisible: info.isPartOfScoreSet,
                    match: info.partsMatchScore
                };
            });

            return flatResult;
        });

        return flattenedData;
    }

    private processCollatedColumns(scoreParts: Array<DanceScorePartDto>) {

        const tagsSeen = [];
        const columnDefinitions = scoreParts.map((ds) => {

            if (!tagsSeen.some(seen => seen === ds.scoreCategoryTag)) {

                tagsSeen.push(ds.scoreCategoryTag);

                return {
                    columnDef: ds.scoreCategoryTag, header: ds.scoreCategoryTag, cell: (element: any) => {
                        return `${element[ds.scoreCategoryTag]}`;
                    }
                };
            }

            return null;
        });

        return columnDefinitions.filter(cd => cd != null);
    }
}
