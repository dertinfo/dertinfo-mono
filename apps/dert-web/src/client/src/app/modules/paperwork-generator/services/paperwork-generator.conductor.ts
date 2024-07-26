import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ScoreSheetDto } from 'app/models/dto';
import { PaperworkGeneratorRepository } from 'app/modules/paperwork-generator/services/paperwork-generator.repository';

@Injectable()
export class PaperworkGeneratorConductor {

    constructor(

        private _paperworkRepo: PaperworkGeneratorRepository
    ) { }

    public getScoresheetPopulatedData(competitionId: number): Observable<Array<ScoreSheetDto>> {
        return this._paperworkRepo.scoresheetsPopulated(competitionId);
    }

    public getScoresheetSparesData(competitionId: number): any {
        return this._paperworkRepo.scoresheetsSpares(competitionId);
    }

    public getSignInSheetPopulatedData(eventId: number): any {
        return this._paperworkRepo.signInSheets(eventId);
    }
}
