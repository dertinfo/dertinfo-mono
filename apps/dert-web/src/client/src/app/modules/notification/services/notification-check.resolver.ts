import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { NotificationThumbnailInfoDto } from '../models/NotificationThumbnailInfoDto';
import { NotificationConductor } from './notification.conductor';

/**
 * Note that currently we only load the settings when entering the dert of derts
 * functionality in order to prevent api hits on the homepage
 */
@Injectable()
export class NotificationCheckResolver implements Resolve<NotificationThumbnailInfoDto> {
    constructor(private conductor: NotificationConductor) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<NotificationThumbnailInfoDto> {
        return this.conductor.checkForMessages();
    }
}
