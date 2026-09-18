export interface IApiError {
  message: string;
  errors?: Record<string, string[]>;
}

export function isApiError(value: unknown): value is IApiError {
  if (!isRecord(value) || typeof value['message'] !== 'string') {
    return false;
  }

  return value['errors'] === undefined || isValidationErrors(value['errors']);
}

function isValidationErrors(value: unknown): value is Record<string, string[]> {
  return (
    isRecord(value) &&
    Object.values(value).every(
      (messages) =>
        Array.isArray(messages) && messages.every((message) => typeof message === 'string'),
    )
  );
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null && !Array.isArray(value);
}
