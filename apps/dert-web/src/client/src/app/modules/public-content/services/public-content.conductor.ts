import { Injectable } from '@angular/core';
import { PublicContentRepository } from 'app/modules/repositories';

import { CompetitionResultDto } from 'app/models/dto';
import { PublicContentTracker } from './public-content.tracker';

@Injectable()
export class PublicContentConductor {

    constructor(

        private _resultsRepo: PublicContentRepository,
        private _publicContentTracker: PublicContentTracker
    ) {

        this._publicContentTracker.reset();
    }

    public clear() {

        this._publicContentTracker.reset();
    }

    public initResultLookup() {

        if (!this._publicContentTracker.hasLoadedResultsLookup()) {
            const subs = this._resultsRepo.getResultsLookup().subscribe((resultsLookup) => {
                subs.unsubscribe();
                this._publicContentTracker.resultLookup = resultsLookup;
            });
        }
    }

    public initResults(competitionId: number, resultType: string) {

        if (!this._publicContentTracker.hasLoadedResult(competitionId, resultType)) {
            const subs = this._resultsRepo.getResultsByCompetitionAndActivity(
                competitionId,
                resultType).subscribe(
                    (result: CompetitionResultDto) => {
                    subs.unsubscribe();
                    this._publicContentTracker.addResult(result);
                });
        } else {
            this._publicContentTracker.notifyCurrentResults();
        }
    }

}
