import { ChangeDetectionStrategy, Component, effect, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { IScenarioFormValue } from '../../types/IScenarioFormValue';
import { IApiError } from '../../../../../common/types/IApiError';
import { requiredTrimmedValidator } from '../../../../../common/validators/required-trimmed.validator';
import { applyServerValidationErrors } from '../../../../../common/validators/apply-server-validation-errors';

@Component({
  selector: 'app-scenario-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './scenario-form.component.html',
  styleUrl: './scenario-form.component.scss',
})
export class ScenarioFormComponent {
  readonly submitting = input(false);
  readonly serverError = input<IApiError | null>(null);
  readonly submitted = output<IScenarioFormValue>();
  readonly cancelled = output<void>();
  readonly form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [requiredTrimmedValidator, Validators.maxLength(200)],
    }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [Validators.maxLength(1000)],
    }),
  });

  constructor() {
    effect(() => applyServerValidationErrors(this.form, this.serverError()));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.submitted.emit({ name: value.name.trim(), description: value.description.trim() });
  }
}
