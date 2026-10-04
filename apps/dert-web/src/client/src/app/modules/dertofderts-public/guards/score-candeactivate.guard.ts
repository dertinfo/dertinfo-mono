import { Location } from '@angular/common';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot } from '@angular/router';
import { AppConfirmService } from 'app/services/app-confirm/app-confirm.service';
import { Observable, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ScoreComponent } from '../components/score/score.component';

@Injectable()
export class CanDeactivateScoreGuard implements CanDeactivate<ScoreComponent> {
    constructor(
        private location: Location,
        private appConfirmService: AppConfirmService
    ) {
    }

    canDeactivate(
        component: ScoreComponent,
        currentRoute: ActivatedRouteSnapshot,
        currentState: RouterStateSnapshot,
        nextState: RouterStateSnapshot
    ): Observable<boolean> {

        if (component.form.dirty) {
            this.location.go(currentState.url);
            return this.appConfirmService.confirm('Leave page?', 'It looks like you entered some information. Are you sure you want to leave?', 'dod-dialog-panel');
        }

        return of(true);
    }
}
