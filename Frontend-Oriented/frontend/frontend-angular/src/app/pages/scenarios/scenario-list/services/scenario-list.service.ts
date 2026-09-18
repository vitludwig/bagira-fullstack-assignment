import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { IScenarioListItem } from '../types/IScenarioListItem';
import { IPagedResponse } from '../../../../common/types/IPagedResponse';
import { IScenarioListQuery } from '../types/IScenarioListQuery';

@Injectable()
export class ScenarioListService {
  private readonly http = inject(HttpClient);

  getAll(query: IScenarioListQuery): Observable<IPagedResponse<IScenarioListItem>> {
    const params = new HttpParams()
      .set('page', query.page)
      .set('pageSize', query.pageSize)
      .set('search', query.search)
      .set('sortBy', query.sortBy)
      .set('sortDirection', query.sortDirection);
    return this.http.get<IPagedResponse<IScenarioListItem>>(`${environment.apiUrl}/api/scenarios`, { params });
  }
}
