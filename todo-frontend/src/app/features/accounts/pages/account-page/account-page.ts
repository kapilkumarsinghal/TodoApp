import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../../data-access/account.service';

@Component({
  selector: 'app-account-page', standalone: true, imports: [CommonModule, FormsModule],
  templateUrl: './account-page.html', styleUrl: './account-page.css',
})
export class AccountPageComponent implements OnInit {
  private readonly accounts = inject(AccountService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  readonly isRegister = this.route.snapshot.routeConfig?.path === 'register';
  readonly email = signal('');
  readonly password = signal('');
  readonly error = signal('');
  readonly busy = signal(false);

  ngOnInit(): void { if (this.accounts.isAuthenticated) void this.router.navigateByUrl('/habits'); }
  async submit(): Promise<void> {
    this.error.set(''); this.busy.set(true);
    try {
      const request = { email: this.email().trim(), password: this.password() };
      if (this.isRegister) await this.accounts.register(request); else await this.accounts.login(request);
      const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
      await this.router.navigateByUrl(returnUrl?.startsWith('/') ? returnUrl : '/');
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        const message = typeof error.error === 'string' ? error.error : error.error?.title;
        this.error.set(message || (error.status === 0
          ? 'The API is unavailable. Start TodoApp.Api and make sure SQL Server is running.'
          : this.isRegister ? 'Could not create account. Check the email and password requirements.' : 'Email or password is incorrect.'));
      } else this.error.set('Something went wrong. Please try again.');
    } finally { this.busy.set(false); }
  }
}
