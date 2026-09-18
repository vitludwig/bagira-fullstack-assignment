import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { NotificationService } from '../../../common/services/notification.service';
import { IApiError } from '../../../common/types/IApiError';
import { EntityFormComponent } from './components/entity-form/entity-form.component';
import { EntityCreateService } from './services/entity-create.service';
import { ICreateEntityRequest } from './types/ICreateEntityRequest';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-entity-create',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, EntityFormComponent, MatButton],
  providers: [EntityCreateService],
  templateUrl: './entity-create.component.html',
  styleUrl: './entity-create.component.scss',
})
export class EntityCreateComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly entityCreateService = inject(EntityCreateService);
  private readonly notification = inject(NotificationService);
  protected readonly router = inject(Router);
  readonly scenarioId = this.route.snapshot.paramMap.get('scenarioId') ?? '';
  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);

  create(value: ICreateEntityRequest): void {
    this.submitting.set(true);
    this.error.set(null);

    this.entityCreateService
      .create(this.scenarioId, value)
      .pipe(finalize(() => this.submitting.set(false)))
      .subscribe({
        next: () => {
          this.notification.success('Entity created successfully.');
          void this.router.navigate(['/scenarios', this.scenarioId]);
        },
        error: (error: IApiError) => {
          this.error.set(error.message);
          this.notification.error('Entity could not be created.');
        },
      });
  }
}
