import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { EScenarioSortOption } from '../../types/EScenarioSortOption';

@Component({
  selector: 'app-scenario-filters',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './scenario-filters.component.html',
  styleUrl: './scenario-filters.component.scss',
})
export class ScenarioFiltersComponent {
  readonly query = input.required<string>();
  readonly sort = input.required<EScenarioSortOption>();
  readonly queryChange = output<string>();
  readonly sortChange = output<EScenarioSortOption>();
  readonly sortOptions = EScenarioSortOption;
}
