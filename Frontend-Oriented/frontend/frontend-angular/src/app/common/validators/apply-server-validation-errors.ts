import { FormGroup } from '@angular/forms';
import { IApiError } from '../types/IApiError';

export function applyServerValidationErrors(form: FormGroup, error: IApiError | null): void {
  for (const [field, messages] of Object.entries(error?.errors ?? {})) {
    const controlName = Object.keys(form.controls).find(
      (candidate) => candidate.toLowerCase() === field.toLowerCase(),
    );
    const control = controlName ? form.get(controlName) : null;
    if (control && messages[0]) {
      control.setErrors({ ...control.errors, server: messages[0] });
    }
  }
}
