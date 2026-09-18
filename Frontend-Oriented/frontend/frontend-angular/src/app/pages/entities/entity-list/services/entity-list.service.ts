import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { IPagedResponse } from '../../../../common/types/IPagedResponse';
import { IEntity } from '../types/IEntity';
import { IEntityListQuery } from '../types/IEntityListQuery';

@Injectable()
export class EntityListService {
  private readonly http = inject(HttpClient);

  getByScenario(scenarioId: string, query: IEntityListQuery): Observable<IPagedResponse<IEntity>> {
    const params = new HttpParams()
      .set('page', query.page)
      .set('pageSize', query.pageSize)
      .set('search', query.search)
      .set('type', query.type)
      .set('taskForce', query.taskForce)
      .set('sortBy', query.sortBy)
      .set('sortDirection', query.sortDirection);

    return this.http.get<IPagedResponse<IEntity>>(
      `${environment.apiUrl}/api/scenarios/${scenarioId}/entities`,
      { params },
    );
  }
}
