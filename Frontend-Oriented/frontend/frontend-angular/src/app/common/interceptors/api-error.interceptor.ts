import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { IApiError, isApiError } from '../types/IApiError';

export const apiErrorInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(catchError((error: unknown) => throwError(() => normalizeApiError(error))));

function normalizeApiError(error: unknown): IApiError {
  if (!(error instanceof HttpErrorResponse)) {
    return { message: 'An unexpected error occurred.' };
  }

  if (isApiError(error.error)) {
    return error.error;
  }

  if (error.status === 0) {
    return { message: 'The API is unavailable. Check that the backend is running.' };
  }

  if (error.status === 404) {
    return { message: 'The requested resource was not found.' };
  }

  return { message: `The request failed (${error.status}).` };
}
