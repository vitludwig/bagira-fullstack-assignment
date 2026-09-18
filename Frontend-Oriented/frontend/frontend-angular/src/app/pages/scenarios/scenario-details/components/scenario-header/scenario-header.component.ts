import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { IScenarioDetails } from '../../types/IScenarioDetails';

@Component({
  selector: 'app-scenario-header',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, RouterLink, MatButtonModule],
  templateUrl: './scenario-header.component.html',
  styleUrl: './scenario-header.component.scss',
})
export class ScenarioHeaderComponent {
  readonly scenario = input.required<IScenarioDetails>();
}
