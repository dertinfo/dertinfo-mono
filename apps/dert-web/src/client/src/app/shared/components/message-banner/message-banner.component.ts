import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
    selector: 'app-message-banner',
    templateUrl: './message-banner.component.html',
    styleUrls: ['./message-banner.component.css']
})
export class MessageBannerComponent implements OnInit {

    @Output() action: EventEmitter<any> = new EventEmitter();
    @Input() active = false;
    @Input() hideButton = false;
    @Input() buttonText = 'Confirm';

    constructor() { }

    ngOnInit() { }

    public onButtonClick($event) {
        this.action.emit($event);
    }

}
