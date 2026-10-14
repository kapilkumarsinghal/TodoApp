import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from './account.service';
import { apiBaseUrl } from '../../../core/api-config';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(AccountService).token;
  if (!token || !request.url.startsWith(`${apiBaseUrl}/`)) return next(request);
  return next(request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
};
