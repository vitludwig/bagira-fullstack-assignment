import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { finalize, forkJoin } from 'rxjs';
import { EmptyStateComponent } from '../../../common/components/empty-state/empty-state.component';
import { ErrorStateComponent } from '../../../common/components/error-state/error-state.component';
import { LoadingStateComponent } from '../../../common/components/loading-state/loading-state.component';
import { IApiError } from '../../../common/types/IApiError';
import { EntityFiltersComponent } from '../../entities/entity-list/components/entity-filters/entity-filters.component';
import { EntityTableComponent } from '../../entities/entity-list/components/entity-table/entity-table.component';
import { EntityListService } from '../../entities/entity-list/services/entity-list.service';
import { ScenarioHeaderComponent } from './components/scenario-header/scenario-header.component';
import { ScenarioDetailsService } from './services/scenario-details.service';
import { IEntity } from '../../entities/entity-list/types/IEntity';
import { IScenarioDetails } from './types/IScenarioDetails';

@Component({
  selector: 'app-scenario-details',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    MatButtonModule,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent,
    EntityFiltersComponent,
    EntityTableComponent,
    ScenarioHeaderComponent,
  ],
  providers: [ScenarioDetailsService, EntityListService],
  templateUrl: './scenario-details.component.html',
  styleUrl: './scenario-details.component.scss',
})
export class ScenarioDetailsComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(ScenarioDetailsService);
  private readonly entityListService = inject(EntityListService);

  readonly scenarioId = this.route.snapshot.paramMap.get('scenarioId') ?? '';
  readonly scenario = signal<IScenarioDetails | null>(null);
  readonly entities = signal<IEntity[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly query = signal('');
  readonly type = signal('');
  readonly taskForce = signal('');
  readonly sort = signal('name-asc');
  readonly visibleEntities = computed(() => {
    const query = this.query().trim().toLowerCase();
    const type = this.type();
    const force = this.taskForce();
    const result = this.entities().filter(
      (entity) =>
        (!query || entity.name.toLowerCase().includes(query)) &&
        (!type || entity.type === type) &&
        (!force || entity.taskForce === force),
    );
    return [...result].sort((a, b) => {
      switch (this.sort()) {
        case 'name-desc':
          return b.name.localeCompare(a.name);
        case 'type-asc':
          return a.type.localeCompare(b.type) || a.name.localeCompare(b.name);
        case 'force-asc':
          return a.taskForce.localeCompare(b.taskForce) || a.name.localeCompare(b.name);
        default:
          return a.name.localeCompare(b.name);
      }
    });
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      scenario: this.service.getById(this.scenarioId),
      entities: this.entityListService.getByScenario(this.scenarioId),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => {
          this.scenario.set(result.scenario);
          this.entities.set(result.entities);
        },
        error: (error: IApiError) => this.error.set(error.message),
      });
  }
}
