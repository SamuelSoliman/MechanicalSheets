import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

// Equivalente di apiFetch()/apiUpload() in index.html:
// aggiunge `Authorization: Bearer` a tutto tranne /auth/login e /auth/register.
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const isAuthCall = req.url.includes('/auth/login') || req.url.includes('/auth/register');
  const token = auth.token();

  if (isAuthCall || !token) return next(req);

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    }),
  );
};
