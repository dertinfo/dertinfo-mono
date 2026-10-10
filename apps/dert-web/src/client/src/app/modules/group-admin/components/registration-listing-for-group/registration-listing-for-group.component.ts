import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { GroupRegistrationDto } from 'app/models/dto';

@Component({
    selector: 'app-registration-listing-for-group',
    templateUrl: './registration-listing-for-group.component.html',
    styleUrls: ['./registration-listing-for-group.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class RegistrationsListingForGroupComponent {

    @Input() groupRegistrationOverviews: GroupRegistrationDto[] = [];

    constructor() {
    }
}
