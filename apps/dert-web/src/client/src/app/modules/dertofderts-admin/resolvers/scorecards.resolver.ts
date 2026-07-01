import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';
import { Repository } from '../services/dertofderts-admin.repository';

@Injectable()
export class ScorecardsResolver {

  constructor(
    private _repository: Repository,
  ) { }

  resolve(activatedRoute: ActivatedRouteSnapshot) {

    const judgeId = activatedRoute.paramMap.get('judgeId');
    const submissionId = activatedRoute.paramMap.get('submissionId');

    if (judgeId != null) {
      return this._repository.getScoreCardsByJudge(+judgeId);
    }

    if (submissionId != null) {
      return this._repository.getScoreCardsBySubmission(+submissionId);
    }
  }
}
