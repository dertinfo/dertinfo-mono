import { Injectable } from '@angular/core';
import { CompetitionLookupDto, CompetitionResultDto } from 'app/models/dto';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class PublicContentTracker {

    get resultLookup$() { return this._resultsLookup.asObservable(); }
    get cachedResults$() { return this._cachedResults.asObservable(); }

    public set resultLookup(overview: CompetitionLookupDto) {
        this._memoryStore.resultsLookup = overview;
        this._resultsLookup.next(this._memoryStore.resultsLookup);
    }

    private _resultsLookup = new BehaviorSubject<CompetitionLookupDto>(null);
    private _cachedResults = new BehaviorSubject<{ [key: string]: CompetitionResultDto }>(null);

    private _memoryStore: {
        resultsLookup: CompetitionLookupDto,
        cachedResults: { [key: string]: CompetitionResultDto },
    };

    constructor() {
        this.reset();
    }

    public reset(): void {
        this._memoryStore = {
            resultsLookup: null,
            cachedResults: {},
        };
    }

    public hasLoadedResultsLookup(): boolean {
        return this._memoryStore.resultsLookup !== null;
    }

    public hasLoadedResult(competitionId: number, resultType: string): any {
        if (this._memoryStore.cachedResults) {
            const key = `${competitionId}_${resultType}`;
            return null !== this._memoryStore.cachedResults[key] && undefined !== this._memoryStore.cachedResults[key];
        } else {
            return false;
        }
    }

    public addResult(result: CompetitionResultDto): any {

        const key = `${result.competitionId}_${result.resultType}`;
        this._memoryStore.cachedResults[key] = result;
        this._cachedResults.next(this._memoryStore.cachedResults);
    }

    public notifyCurrentResults(): any {
        this._cachedResults.next(this._memoryStore.cachedResults);
    }
}
