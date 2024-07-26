import { Component, Input } from '@angular/core';
import { GroupRegistrationDto } from 'app/models/dto';

@Component({
    selector: 'app-registration-listing-for-group',
    templateUrl: './registration-listing-for-group.component.html',
    styleUrls: ['./registration-listing-for-group.component.scss']
})
export class RegistrationsListingForGroupComponent {

    @Input() groupRegistrationOverviews: GroupRegistrationDto[] = [];

    constructor() {
    }
}
