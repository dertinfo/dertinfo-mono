import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output } from '@angular/core';
import { CompetitionEntryAttributeDto } from 'app/models/dto';

@Component({
    selector: 'app-competition-entrants-attributeselector',
    templateUrl: './attribute-selector.component.html',
    styleUrls: ['./attribute-selector.component.css']
})
export class AttributeSelectorComponent<T> implements OnInit {

    @Input() fullSet: Array<CompetitionEntryAttributeDto>;

    @Input() selectedSet: Array<CompetitionEntryAttributeDto>;

    @Input() disabled: boolean;

    @Output() selectionChanged = new EventEmitter();

    public selectItems: Array<any> = [];

    constructor() { }

    ngOnInit(): void {
        this.refresh();
    }

    refresh() {
        this.selectItems = this.fullSet.map((entryAttribute) => {

            const isSelected = this.selectedSet.some(cea => cea.id === entryAttribute.id);

            return {
                id: entryAttribute.id,
                name: entryAttribute.name,
                tag: entryAttribute.tag,
                isSelected: isSelected
            };
        });
    }

    public onItemClick(entryAttribute: any) {

        if (!entryAttribute.disabled) {

            const isSelected = this.selectedSet.some(ssi => ssi.id === entryAttribute.id);

            if (isSelected) {
                this.selectionChanged.emit({
                    toggle: 'off',
                    entryAttribute: entryAttribute
                });

            } else {
                this.selectionChanged.emit({
                    toggle: 'on',
                    entryAttribute: entryAttribute
                });
            }

            entryAttribute.isSelected = !entryAttribute.isSelected;
        }
    }

}
