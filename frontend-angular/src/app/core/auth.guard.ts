import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

// Vue-router beforeEach / Laravel 'auth' middleware.
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return auth.isLoggedIn() ? true : router.createUrlTree(['/login']);
};

export const mechanicGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isLoggedIn()) return router.createUrlTree(['/login']);
  return auth.isMechanic() ? true : router.createUrlTree(['/sheets']);
};

export const managerGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isLoggedIn()) return router.createUrlTree(['/login']);
  return auth.isManager() ? true : router.createUrlTree(['/sheets']);
};
