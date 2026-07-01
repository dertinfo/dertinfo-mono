import { Injectable } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, NavigationEnd, Params, PRIMARY_OUTLET, Router } from '@angular/router';

interface IRoutePart {
  title: string;
  breadcrumb: string;
  breadcrumbIcon: string;
  params?: Params;
  url: string;
  urlSegments: any[];
}

@Injectable({ providedIn: 'root' })
export class RoutePartsService {
  public routeParts: IRoutePart[];
  constructor(private router: Router) {}

  generateRouteParts(snapshot: ActivatedRouteSnapshot): IRoutePart[] {
    let routeParts = <IRoutePart[]>[];
    if (snapshot) {
      if (snapshot.firstChild) {
        routeParts = routeParts.concat(this.generateRouteParts(snapshot.firstChild));
      }
      if (snapshot.data['title'] && snapshot.url.length) {
        routeParts.push({
          title: snapshot.data['title'],
          breadcrumb: snapshot.data['breadcrumb'],
          breadcrumbIcon: snapshot.data['breadcrumbIcon'],
          url: snapshot.url[0].path,
          urlSegments: snapshot.url,
          params: snapshot.params
        });
      }
    }
    return routeParts;
  }
}
