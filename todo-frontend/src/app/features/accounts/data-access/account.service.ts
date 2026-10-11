import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AccountRequest, AccountResponse } from '../models/account.models';

const tokenKey = 'todoapp.accessToken';
const emailKey = 'todoapp.email';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = 'http://localhost:5160/api/accounts';
  readonly email = signal(localStorage.getItem(emailKey) ?? '');

  get token(): string | null {
    const token = localStorage.getItem(tokenKey);
    if (!token || this.isExpired(token)) { this.logout(); return null; }
    return token;
  }
  get isAuthenticated(): boolean { return this.token !== null; }

  async login(request: AccountRequest): Promise<void> {
    this.save(await firstValueFrom(this.http.post<AccountResponse>(`${this.endpoint}/login`, request)));
  }
  async register(request: AccountRequest): Promise<void> {
    this.save(await firstValueFrom(this.http.post<AccountResponse>(`${this.endpoint}/register`, request)));
  }
  logout(): void { localStorage.removeItem(tokenKey); localStorage.removeItem(emailKey); this.email.set(''); }

  private save(response: AccountResponse): void {
    localStorage.setItem(tokenKey, response.accessToken);
    localStorage.setItem(emailKey, response.email);
    this.email.set(response.email);
  }
  private isExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/'))) as { exp?: number };
      return !payload.exp || payload.exp * 1000 <= Date.now();
    } catch { return true; }
  }
}
