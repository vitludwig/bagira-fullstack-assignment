import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { IEntity } from '../../entity-list/types/IEntity';
import { ICreateEntityRequest } from '../types/ICreateEntityRequest';

@Injectable()
export class EntityCreateService {
  private readonly http = inject(HttpClient);

  create(scenarioId: string, request: ICreateEntityRequest): Observable<IEntity> {
    return this.http.post<IEntity>(
      `${environment.apiUrl}/api/scenarios/${scenarioId}/entities`,
      request,
    );
  }
}
