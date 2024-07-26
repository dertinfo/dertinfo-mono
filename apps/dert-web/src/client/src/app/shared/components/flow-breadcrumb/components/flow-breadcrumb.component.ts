import { Component, Input } from '@angular/core';
import { FlowBreadcrumbItem } from '../models/flow-breadcrumb-item.model';

@Component({
    selector: 'app-flow-breadcrumb',
    templateUrl: './flow-breadcrumb.component.html',
    styleUrls: ['./flow-breadcrumb.component.css']
})
export class FlowBreadcrumbComponent {

    @Input() states: FlowBreadcrumbItem[] = [];
    @Input() selectedState: number;
    @Input() additionalMessage: string;

    public get allStates(): FlowBreadcrumbItem[] {
        return this.states.map(s => {
            return {
                id: s.id,
                name: s.name,
                selected: s.id === this.selectedState,
                info: s.info
            };
        });
    }

    constructor() {
    }
}
