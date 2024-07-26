import { Injectable } from '@angular/core';
import { ClientSettingsDto } from 'app/models/dto/ClientSettingsDto';
import { ClientSettingRepository } from 'app/modules/repositories/repositories/clientsetting.repository';
import { observable, Observable, of } from 'rxjs';
import { share } from 'rxjs/operators';

/**
 * Note that currently we onlt load the settings when entering the dert of derts
 * functionality in order to prevent api hits on the homepage
 */
@Injectable({ providedIn: 'root' })
export class ClientSettingsService {

    private _clientSettings: ClientSettingsDto;

    public get dodOpenToPublic() {
        return this._clientSettings.dodOpenToPublic;
    }

    public get dodResultsPublished() {
        return this._clientSettings.dodResultsPublished;
    }

    public get dodPublicResultsForwarded() {
        return this._clientSettings.dodPublicResultsForwarded;
    }

    public get dodOfficialResultsForwarded() {
        return this._clientSettings.dodOfficialResultsForwarded;
    }

    constructor(private settingsRepository: ClientSettingRepository) {
    }

    public loaded() {
        return this._clientSettings != null;
    }

    public loadSettings() {
        const obs$ = this.settingsRepository.getAll();

        const subs = obs$.subscribe((settings: ClientSettingsDto) => {
            subs.unsubscribe();
            this._clientSettings = settings;
        });

        return obs$;
    }

    public getSettings() {
        return of(this._clientSettings);
    }

}
