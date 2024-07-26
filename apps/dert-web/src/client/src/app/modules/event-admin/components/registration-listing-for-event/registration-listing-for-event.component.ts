import { Component, Input } from '@angular/core';
import { EventRegistrationDto } from 'app/models/dto';

@Component({
    selector: 'app-registration-listing-for-event',
    templateUrl: './registration-listing-for-event.component.html',
    styleUrls: ['./registration-listing-for-event.component.css']
})
export class RegistrationsListingForEventComponent {

    @Input() eventRegistrationOverviews: EventRegistrationDto[] = [];

    constructor() {
    }
}
