import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { NotificationService } from '../../../common/services/notification.service';
import { IApiError } from '../../../common/types/IApiError';
import { ScenarioFormComponent } from './components/scenario-form/scenario-form.component';
import { ScenarioCreateService } from './services/scenario-create.service';
import { IScenarioFormValue } from './types/IScenarioFormValue';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-scenario-create',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, ScenarioFormComponent, MatButton],
  providers: [ScenarioCreateService],
  templateUrl: './scenario-create.component.html',
  styleUrl: './scenario-create.component.scss',
})
export class ScenarioCreateComponent {
  private readonly service = inject(ScenarioCreateService);
  private readonly notification = inject(NotificationService);
  protected readonly router = inject(Router);
  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);

  create(value: IScenarioFormValue): void {
    this.submitting.set(true);
    this.error.set(null);
    this.service
      .create({ name: value.name, description: value.description || null })
      .pipe(finalize(() => this.submitting.set(false)))
      .subscribe({
        next: (scenario) => {
          this.notification.success('Scenario created successfully.');
          void this.router.navigate(['/scenarios', scenario.id]);
        },
        error: (error: IApiError) => {
          this.error.set(error.message);
          this.notification.error('Scenario could not be created.');
        },
      });
  }
}
