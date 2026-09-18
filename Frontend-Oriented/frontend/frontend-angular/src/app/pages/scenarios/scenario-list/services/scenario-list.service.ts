import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { IScenarioListItem } from '../types/IScenarioListItem';

@Injectable()
export class ScenarioListService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<IScenarioListItem[]> {
    return this.http.get<IScenarioListItem[]>(`${environment.apiUrl}/api/scenarios`);
  }
}
