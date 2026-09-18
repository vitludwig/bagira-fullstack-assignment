import { AbstractControl, ValidationErrors } from '@angular/forms';

export function requiredTrimmedValidator(
  control: AbstractControl<string>,
): ValidationErrors | null {
  return control.value.trim().length > 0 ? null : { required: true };
}
