import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

// Mappa ExceptionHandlingMiddleware → toast:
// KeyNotFound→404, Unauthorized→401, InvalidOperation→400 (vedi AGENTS.md).
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snack = inject(MatSnackBar);
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      const msg =
        (err.error as { message?: string })?.message ??
        (err.status === 401 ? 'Non autorizzato — rieffettua il login' : 'Errore di rete');
      // Non mostrare toast sul login fallito: lo gestisce il form.
      if (!req.url.includes('/auth/login')) {
        snack.open(msg, 'Chiudi', { duration: 3500 });
      }
      return throwError(() => err);
    }),
  );
};
