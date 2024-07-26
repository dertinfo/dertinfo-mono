
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import {filter} from 'rxjs/operators';
import { RoutePartsService } from '../../../core/services/route-parts.service';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit {
  routeParts: any[];
  public isEnabled: boolean = true;
  constructor(
    private router: Router,
    private routePartsService: RoutePartsService,
    private activeRoute: ActivatedRoute
  ) {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe((routeChange) => {
      this.routeParts = this.routePartsService.generateRouteParts(this.activeRoute.snapshot);
      // generate url from parts
      this.routeParts.reverse().map((item, i) => {
        item.breadcrumb = this.parseText(item);
        item.breadcrumb = decodeURIComponent(item.breadcrumb);
        item.urlSegments.forEach((urlSegment, j) => {
          if (j === 0) {
            return item.url = `${urlSegment.path}`;
          }
          item.url += `/${urlSegment.path}`;
        });
        if (i === 0) {
          return item;
        }
        // prepend previous part to current part
        item.url = `${this.routeParts[i - 1].url}/${item.url}`;
        return item;
      });
    });
  }

  ngOnInit() {}

  parseText(part) {
    part.breadcrumb = part.breadcrumb.replace(/{{([^{}]*)}}/g, function (a, b) {
      const r = part.params[b];
      return typeof r === 'string' ? r : a;
    });
    return part.breadcrumb;
  }

}
