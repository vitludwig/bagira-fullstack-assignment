import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  EMPTY,
  finalize,
  Subject,
  switchMap,
} from 'rxjs';
import { IApiError } from '../../../common/types/IApiError';
import { EmptyStateComponent } from '../../../common/components/empty-state/empty-state.component';
import { ErrorStateComponent } from '../../../common/components/error-state/error-state.component';
import { LoadingStateComponent } from '../../../common/components/loading-state/loading-state.component';
import { ScenarioFiltersComponent } from './components/scenario-filters/scenario-filters.component';
import { ScenarioTableComponent } from './components/scenario-table/scenario-table.component';
import { ScenarioListService } from './services/scenario-list.service';
import { IScenarioListItem } from './types/IScenarioListItem';
import { IScenarioListQuery } from './types/IScenarioListQuery';
import { EScenarioSortOption } from './types/EScenarioSortOption';

const SORT_QUERIES: Record<
  EScenarioSortOption,
  Pick<IScenarioListQuery, 'sortBy' | 'sortDirection'>
> = {
  [EScenarioSortOption.RecentlyUpdated]: { sortBy: 'updatedAt', sortDirection: 'desc' },
  [EScenarioSortOption.NameAscending]: { sortBy: 'name', sortDirection: 'asc' },
  [EScenarioSortOption.NameDescending]: { sortBy: 'name', sortDirection: 'desc' },
  [EScenarioSortOption.MostEntities]: { sortBy: 'entityCount', sortDirection: 'desc' },
};

@Component({
  selector: 'app-scenario-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    MatButtonModule,
    MatPaginatorModule,
    EmptyStateComponent,
    ErrorStateComponent,
    LoadingStateComponent,
    ScenarioFiltersComponent,
    ScenarioTableComponent,
  ],
  providers: [ScenarioListService],
  templateUrl: './scenario-list.component.html',
  styleUrl: './scenario-list.component.scss',
})
export class ScenarioListComponent {
  private readonly service = inject(ScenarioListService);
  private readonly searchChanges = new Subject<string>();
  private readonly loadRequests = new Subject<boolean>();
  readonly scenarios = signal<IScenarioListItem[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly query = signal('');
  readonly sort = signal(EScenarioSortOption.RecentlyUpdated);
  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly totalCount = signal(0);

  constructor() {
    this.loadRequests
      .pipe(
        switchMap((showLoading) => {
          if (showLoading) {
            this.loading.set(true);
          }
          this.error.set(null);

          return this.service.getAll(this.createQuery()).pipe(
            catchError((error: IApiError) => {
              this.error.set(error.message);
              return EMPTY;
            }),
            finalize(() => showLoading && this.loading.set(false)),
          );
        }),
        takeUntilDestroyed(),
      )
      .subscribe((response) => {
        this.scenarios.set(response.items);
        this.totalCount.set(response.totalCount);
      });

    this.searchChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.page.set(1);
        this.load(false);
      });
    this.load();
  }

  load(showLoading = true): void {
    this.loadRequests.next(showLoading);
  }

  search(value: string): void {
    this.query.set(value);
    this.searchChanges.next(value);
  }

  changeSort(value: EScenarioSortOption): void {
    this.sort.set(value);
    this.page.set(1);
    this.load(false);
  }

  changePage(event: PageEvent): void {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.load(false);
  }

  private createQuery(): IScenarioListQuery {
    return {
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.query(),
      ...SORT_QUERIES[this.sort()],
    };
  }
}
