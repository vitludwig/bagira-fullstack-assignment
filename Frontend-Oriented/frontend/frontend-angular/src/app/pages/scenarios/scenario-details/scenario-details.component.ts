import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged, finalize, forkJoin, Subject } from 'rxjs';
import { EmptyStateComponent } from '../../../common/components/empty-state/empty-state.component';
import { ErrorStateComponent } from '../../../common/components/error-state/error-state.component';
import { LoadingStateComponent } from '../../../common/components/loading-state/loading-state.component';
import { IApiError } from '../../../common/types/IApiError';
import { EntityFiltersComponent } from '../../entities/entity-list/components/entity-filters/entity-filters.component';
import { EntityTableComponent } from '../../entities/entity-list/components/entity-table/entity-table.component';
import { EntityMapComponent } from '../../entities/entity-list/components/entity-map/entity-map.component';
import { EntityListService } from '../../entities/entity-list/services/entity-list.service';
import { ScenarioHeaderComponent } from './components/scenario-header/scenario-header.component';
import { ScenarioDetailsService } from './services/scenario-details.service';
import { IEntity } from '../../entities/entity-list/types/IEntity';
import { IEntityListQuery } from '../../entities/entity-list/types/IEntityListQuery';
import { EEntitySortOption } from '../../entities/entity-list/types/EEntitySortOption';
import { EEntityViewMode } from '../../entities/entity-list/types/EEntityViewMode';
import { IScenarioDetails } from './types/IScenarioDetails';

const ENTITY_SORT_QUERIES: Record<
  EEntitySortOption,
  Pick<IEntityListQuery, 'sortBy' | 'sortDirection'>
> = {
  [EEntitySortOption.NameAscending]: { sortBy: 'name', sortDirection: 'asc' },
  [EEntitySortOption.NameDescending]: { sortBy: 'name', sortDirection: 'desc' },
  [EEntitySortOption.Type]: { sortBy: 'type', sortDirection: 'asc' },
  [EEntitySortOption.TaskForce]: { sortBy: 'taskForce', sortDirection: 'asc' },
};

@Component({
  selector: 'app-scenario-details',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    MatButtonModule,
    MatButtonToggleModule,
    MatPaginatorModule,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent,
    EntityFiltersComponent,
    EntityTableComponent,
    EntityMapComponent,
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
  private readonly searchChanges = new Subject<string>();

  readonly scenarioId = this.route.snapshot.paramMap.get('scenarioId') ?? '';
  readonly scenario = signal<IScenarioDetails | null>(null);
  readonly entities = signal<IEntity[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly query = signal('');
  readonly type = signal('');
  readonly taskForce = signal('');
  readonly sort = signal(EEntitySortOption.NameAscending);
  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly totalCount = signal(0);
  readonly viewMode = signal(EEntityViewMode.Table);
  readonly viewModes = EEntityViewMode;

  constructor() {
    this.searchChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => this.reloadEntities());
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      scenario: this.service.getById(this.scenarioId),
      entities: this.entityListService.getByScenario(this.scenarioId, this.createEntityQuery()),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => {
          this.scenario.set(result.scenario);
          this.entities.set(result.entities.items);
          this.totalCount.set(result.entities.totalCount);
        },
        error: (error: IApiError) => this.error.set(error.message),
      });
  }

  search(value: string): void {
    this.query.set(value);
    this.searchChanges.next(value);
  }

  changeType(value: string): void {
    this.type.set(value);
    this.reloadEntities();
  }

  changeTaskForce(value: string): void {
    this.taskForce.set(value);
    this.reloadEntities();
  }

  changeSort(value: EEntitySortOption): void {
    this.sort.set(value);
    this.reloadEntities();
  }

  changePage(event: PageEvent): void {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadEntities(false);
  }

  private reloadEntities(): void {
    this.page.set(1);
    this.loadEntities(false);
  }

  private loadEntities(showLoading = true): void {
    if (showLoading) {
      this.loading.set(true);
    }
    this.error.set(null);
    this.entityListService
      .getByScenario(this.scenarioId, this.createEntityQuery())
      .pipe(finalize(() => showLoading && this.loading.set(false)))
      .subscribe({
        next: (response) => {
          this.entities.set(response.items);
          this.totalCount.set(response.totalCount);
        },
        error: (error: IApiError) => this.error.set(error.message),
      });
  }

  private createEntityQuery(): IEntityListQuery {
    return {
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.query(),
      type: this.type(),
      taskForce: this.taskForce(),
      ...ENTITY_SORT_QUERIES[this.sort()],
    };
  }
}
