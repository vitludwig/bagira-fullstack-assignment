import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { IEntity } from '../types/IEntity';

@Injectable()
export class EntityListService {
  private readonly http = inject(HttpClient);

  getByScenario(scenarioId: string): Observable<IEntity[]> {
    return this.http.get<IEntity[]>(`${environment.apiUrl}/api/scenarios/${scenarioId}/entities`);
  }
}
