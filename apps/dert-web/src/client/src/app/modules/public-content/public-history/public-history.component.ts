import { Component, OnInit } from '@angular/core';
import { EventShowcaseDto } from 'app/models/dto';
import { ShowcaseRepository } from '../../repositories';

@Component({
  selector: 'app-public-history',
  templateUrl: './public-history.component.html',
  styleUrls: ['./public-history.component.css']
})
export class PublicHistoryComponent implements OnInit {

  public events: EventShowcaseDto;

  constructor(private _showcaseRepository: ShowcaseRepository) { }

  ngOnInit() {
    const subs = this._showcaseRepository.getEvents().subscribe((events) => {
      subs.unsubscribe();
      this.events = events;
    });
  }

  public getPublicEventDetailRouteLink(event: EventShowcaseDto) {
    return ['event/' + encodeURIComponent(event.name) + '/' + event.id + ''];
  }

}
