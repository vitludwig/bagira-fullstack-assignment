import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { IScenarioDetails } from '../../scenario-details/types/IScenarioDetails';
import { ICreateScenarioRequest } from '../types/ICreateScenarioRequest';

@Injectable()
export class ScenarioCreateService {
  private readonly http = inject(HttpClient);

  create(request: ICreateScenarioRequest): Observable<IScenarioDetails> {
    return this.http.post<IScenarioDetails>(`${environment.apiUrl}/api/scenarios`, request);
  }
}
