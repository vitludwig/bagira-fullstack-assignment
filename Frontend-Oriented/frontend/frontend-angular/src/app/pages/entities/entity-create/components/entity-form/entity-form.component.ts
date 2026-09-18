import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { EEntityType } from '../../../../../common/types/EEntityType';
import { ETaskForce } from '../../../../../common/types/ETaskForce';
import { ICreateEntityRequest } from '../../types/ICreateEntityRequest';

@Component({
  selector: 'app-entity-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './entity-form.component.html',
  styleUrl: './entity-form.component.scss',
})
export class EntityFormComponent {
  readonly submitting = input(false);
  readonly serverError = input<string | null>(null);
  readonly submitted = output<ICreateEntityRequest>();
  readonly cancelled = output<void>();
  readonly entityTypes = Object.values(EEntityType);
  readonly taskForces = Object.values(ETaskForce);
  readonly form = new FormGroup({
    type: new FormControl<EEntityType | null>(null, [Validators.required]),
    taskForce: new FormControl<ETaskForce | null>(null, [Validators.required]),
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
    latitude: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(-90),
      Validators.max(90),
    ]),
    longitude: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(-180),
      Validators.max(180),
    ]),
  });

  submit(): void {
    const value = this.form.getRawValue();

    if (this.form.invalid || !isICreateEntityRequest(value)) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitted.emit({
      type: value.type,
      taskForce: value.taskForce,
      name: value.name.trim(),
      latitude: value.latitude,
      longitude: value.longitude,
    });
  }
}

function isICreateEntityRequest(
  value: ReturnType<EntityFormComponent['form']['getRawValue']>,
): value is ICreateEntityRequest {
  return (
    value.type !== null &&
    value.taskForce !== null &&
    value.latitude !== null &&
    value.longitude !== null
  );
}

