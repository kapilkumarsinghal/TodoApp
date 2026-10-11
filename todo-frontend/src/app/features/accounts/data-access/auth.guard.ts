import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from './account.service';

export const accountGuard: CanActivateFn = (_route, state) => {
  const accounts = inject(AccountService);
  return accounts.isAuthenticated || inject(Router).createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};
