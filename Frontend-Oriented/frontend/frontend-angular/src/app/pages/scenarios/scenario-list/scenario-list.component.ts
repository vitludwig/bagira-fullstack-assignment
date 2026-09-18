import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { finalize } from 'rxjs';
import { IApiError } from '../../../common/types/IApiError';
import { EmptyStateComponent } from '../../../common/components/empty-state/empty-state.component';
import { ErrorStateComponent } from '../../../common/components/error-state/error-state.component';
import { LoadingStateComponent } from '../../../common/components/loading-state/loading-state.component';
import { ScenarioFiltersComponent } from './components/scenario-filters/scenario-filters.component';
import { ScenarioTableComponent } from './components/scenario-table/scenario-table.component';
import { ScenarioListService } from './services/scenario-list.service';
import { IScenarioListItem } from './types/IScenarioListItem';

@Component({
  selector: 'app-scenario-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    MatButtonModule,
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
  readonly scenarios = signal<IScenarioListItem[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly query = signal('');
  readonly sort = signal('updated-desc');
  readonly visibleScenarios = computed(() => {
    const query = this.query().trim().toLowerCase();
    const result = this.scenarios().filter(
      (item) =>
        !query ||
        item.name.toLowerCase().includes(query) ||
        item.description?.toLowerCase().includes(query),
    );
    return [...result].sort((a, b) => {
      switch (this.sort()) {
        case 'name-asc':
          return a.name.localeCompare(b.name);
        case 'name-desc':
          return b.name.localeCompare(a.name);
        case 'entities-desc':
          return (b.entityCount ?? 0) - (a.entityCount ?? 0);
        default:
          return (b.updatedAt ?? '').localeCompare(a.updatedAt ?? '');
      }
    });
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.service
      .getAll()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (scenarios) => this.scenarios.set(scenarios),
        error: (error: IApiError) => this.error.set(error.message),
      });
  }
}
