import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { IScenarioDetails } from '../types/IScenarioDetails';

@Injectable()
export class ScenarioDetailsService {
  private readonly http = inject(HttpClient);

  getById(scenarioId: string): Observable<IScenarioDetails> {
    return this.http.get<IScenarioDetails>(`${environment.apiUrl}/api/scenarios/${scenarioId}`);
  }
}
