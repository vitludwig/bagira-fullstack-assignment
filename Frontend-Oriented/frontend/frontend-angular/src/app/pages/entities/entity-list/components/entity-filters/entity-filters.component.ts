import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { EEntityType } from '../../../../../common/types/EEntityType';
import { ETaskForce } from '../../../../../common/types/ETaskForce';
import { EEntitySortOption } from '../../types/EEntitySortOption';

@Component({
  selector: 'app-entity-filters',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './entity-filters.component.html',
  styleUrl: './entity-filters.component.scss',
})
export class EntityFiltersComponent {
  readonly query = input.required<string>();
  readonly type = input.required<string>();
  readonly taskForce = input.required<string>();
  readonly sort = input.required<EEntitySortOption>();
  readonly queryChange = output<string>();
  readonly typeChange = output<string>();
  readonly taskForceChange = output<string>();
  readonly sortChange = output<EEntitySortOption>();
  readonly entityTypes = Object.values(EEntityType);
  readonly taskForces = Object.values(ETaskForce);
  readonly sortOptions = EEntitySortOption;
}
