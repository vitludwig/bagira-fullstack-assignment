import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { IScenarioListItem } from '../../types/IScenarioListItem';

@Component({
  selector: 'app-scenario-table',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, RouterLink, MatButtonModule, MatTableModule],
  templateUrl: './scenario-table.component.html',
  styleUrl: './scenario-table.component.scss',
})
export class ScenarioTableComponent {
  readonly scenarios = input.required<IScenarioListItem[]>();
  readonly columns = ['name', 'description', 'entities', 'updated', 'action'];
}
