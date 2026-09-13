import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthUser, LoginDto, LoginResponse, RegisterDto } from '../models/auth';
import { UserRole } from '../models/user';

// Claim URI usati da .NET JWT (vedi frontend/index.html login()).
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname';

// Vue: Pinia store + Laravel AuthService. Qui: service root con signal() (~ref()).
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'ms_token';

  private readonly _token = signal<string | null>(localStorage.getItem(this.tokenKey));
  private readonly _user = signal<AuthUser | null>(null);

  readonly token = this._token.asReadonly();
  readonly user = this._user.asReadonly();
  readonly isLoggedIn = computed(() => this._token() !== null && this._user() !== null);
  readonly isMechanic = computed(() => this._user()?.role === 'mechanic');
  readonly isManager = computed(() => this._user()?.role === 'manager');

  constructor(private http: HttpClient) {
    // Ripristina sessione come fa il vanilla JS con `token` globale.
    const saved = this._token();
    if (saved) {
      const parsed = this.parseUser(saved);
      if (parsed) this._user.set(parsed);
      else localStorage.removeItem(this.tokenKey);
    }
  }

  login(dto: LoginDto): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, dto).pipe(
      tap((res) => this.setSession(res.token, dto.email)),
    );
  }

  register(dto: RegisterDto): Observable<unknown> {
    return this.http.post(`${environment.apiUrl}/auth/register`, dto);
  }

  logout(): void {
    this._token.set(null);
    this._user.set(null);
    localStorage.removeItem(this.tokenKey);
  }

  private setSession(token: string, fallbackEmail: string): void {
    const user = this.parseUser(token, fallbackEmail);
    if (!user) return;
    this._token.set(token);
    this._user.set(user);
    localStorage.setItem(this.tokenKey, token);
  }

  private parseUser(token: string, fallbackEmail = ''): AuthUser | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const role = String(payload[ROLE_CLAIM] ?? '').toLowerCase() as UserRole;
      if (role !== 'mechanic' && role !== 'manager') return null;
      return {
        id: parseInt(payload.sub, 10),
        email: payload[EMAIL_CLAIM] ?? payload.email ?? fallbackEmail,
        name: payload[NAME_CLAIM] ?? payload.name ?? fallbackEmail,
        role,
      };
    } catch {
      return null;
    }
  }
}
