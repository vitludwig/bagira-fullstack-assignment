import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { IEntity } from '../../types/IEntity';

@Component({
  selector: 'app-entity-table',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatTableModule],
  templateUrl: './entity-table.component.html',
  styleUrl: './entity-table.component.scss',
})
export class EntityTableComponent {
  readonly entities = input.required<IEntity[]>();
  readonly columns = ['name', 'type', 'taskForce', 'latitude', 'longitude'];
}
