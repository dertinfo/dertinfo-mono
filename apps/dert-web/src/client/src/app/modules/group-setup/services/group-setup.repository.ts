import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RepositoryBase } from 'app/core/base/repository.base';
import { AppInsightsService } from 'app/core/logging/appinsights.service';
import { ConfigurationService } from 'app/core/services/configuration.service';
import { GroupConfigurationSubmissionDto } from 'app/models/dto/GroupConfigurationSubmissionDto';
import { GroupImageDto } from 'app/models/dto/GroupImageDto';
import { GroupOverviewDto } from 'app/models/dto/GroupOverviewDto';
import { Observable } from 'rxjs';
import { catchError, map, share } from 'rxjs/operators';

@Injectable()
export class GroupSetupRepository extends RepositoryBase {

    constructor(http: HttpClient, configurationService: ConfigurationService, appInsightsService: AppInsightsService) {
        super(http, configurationService, appInsightsService);
    }

    overview(groupId: number): Observable<GroupOverviewDto> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/overview';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => this.groupOverviewDtoMap(data)),
            catchError((err) => this.processApiError(err)));
    }

    images(groupId: number): Observable<GroupImageDto[]> {
        const url = this.ApiUri + '/group/' + groupId.toString() + '/images';  // URL to web API

        return this.http.get(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    setPrimaryImage(groupImage: GroupImageDto) {
        const url = `${this.ApiUri}/group/${groupImage.groupId.toString()}/groupimage/${groupImage.groupImageId}/setprimary`;

        return this.http.put(url, groupImage).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    applyConfiguration(groupId: number, groupConfigurationSubmissionDto: GroupConfigurationSubmissionDto) {
        const url = this.ApiUri + '/group/' + groupId + '/configure';  // URL to web API

        return this.http.post(url, groupConfigurationSubmissionDto).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    abandonConfiguration(groupId: number) {
        const url = this.ApiUri + '/group/' + groupId + '/abandonconfig';  // URL to web API

        return this.http.delete(url).pipe(
            share(),
            map((data) => data),
            catchError((err) => this.processApiError(err)));
    }

    private groupOverviewDtoMap(data: any) {
        const groupOverviewDto: GroupOverviewDto = data;

        // Manipulate the image paths to gain the correct sizes - this should be done using a pipe.

        return groupOverviewDto;
    }
}
