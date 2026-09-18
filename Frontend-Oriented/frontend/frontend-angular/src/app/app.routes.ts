import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'scenarios',
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./pages/scenarios/scenario-list/scenario-list.component').then(
            (module) => module.ScenarioListComponent,
          ),
      },
      {
        path: 'create',
        loadComponent: () =>
          import('./pages/scenarios/scenario-create/scenario-create.component').then(
            (module) => module.ScenarioCreateComponent,
          ),
      },
      {
        path: ':scenarioId/entities/create',
        loadComponent: () =>
          import('./pages/entities/entity-create/entity-create.component').then(
            (module) => module.EntityCreateComponent,
          ),
      },
      {
        path: ':scenarioId',
        loadComponent: () =>
          import('./pages/scenarios/scenario-details/scenario-details.component').then(
            (module) => module.ScenarioDetailsComponent,
          ),
      },
    ],
  },
  { path: '**', redirectTo: 'scenarios' },
];
