import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { EventRegistrationDto } from 'app/models/dto';

@Component({
    selector: 'app-registration-listing-for-event',
    templateUrl: './registration-listing-for-event.component.html',
    styleUrls: ['./registration-listing-for-event.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class RegistrationsListingForEventComponent {

    @Input() eventRegistrationOverviews: EventRegistrationDto[] = [];

    constructor() {
    }
}
