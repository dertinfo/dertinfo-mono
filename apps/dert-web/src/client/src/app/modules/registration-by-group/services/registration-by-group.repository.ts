import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { GroupMemberDto } from 'app/models/dto/GroupMemberDto';
import { GroupTeamDto } from 'app/models/dto/GroupTeamDto';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

@Injectable()
export class RegistrationByGroupRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    members(groupId: number): Observable<GroupMemberDto[]> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/members';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => this.groupMemberDtoMap(data)),
            catchError((err) => this.processApiError(err)));
    }

    teams(groupId: number): Observable<GroupTeamDto[]> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/teams';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    private groupMemberDtoMap(data: any) {
        const groupMemberDtos: GroupMemberDto[] = data;

        return groupMemberDtos.map(gm => {
            gm['photo'] = '/assets/images/face-1.jpg';
            return gm;
        });
    }
}
