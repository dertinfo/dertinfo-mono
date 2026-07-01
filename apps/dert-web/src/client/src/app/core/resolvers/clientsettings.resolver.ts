import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { ClientSettingsDto } from 'app/models/dto/ClientSettingsDto';
import { ClientSettingsService } from 'app/core/services/clientsettings.service';
import { Observable } from 'rxjs';

/**
 * Note that currently we only load the settings when entering the dert of derts
 * functionality in order to prevent api hits on the homepage
 */
@Injectable({ providedIn: 'root' })
export class ClientSettingsResolver implements Resolve<ClientSettingsDto> {
    constructor(private clientSettingsService: ClientSettingsService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<ClientSettingsDto> {

        if (!this.clientSettingsService.loaded()) {
            return this.clientSettingsService.loadSettings();
        } else {
            return this.clientSettingsService.getSettings();
        }
    }
}
