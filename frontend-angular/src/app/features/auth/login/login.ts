import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth.service';

// Vue: <Login.vue> con v-model + $router.push. Qui: ReactiveForms + signal() per loading/errore.
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly fb = new FormBuilder().nonNullable;

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  constructor(
    private auth: AuthService,
    private router: Router,
    private snack: MatSnackBar,
  ) {}

  fill(email: string): void {
    this.form.setValue({ email, password: 'password123' });
  }

  submit(): void {
    if (this.form.invalid || this.loading()) return;
    this.loading.set(true);
    this.error.set(null);
    const { email, password } = this.form.getRawValue();
    this.auth.login({ email: email.trim(), password }).subscribe({
      next: () => {
        this.loading.set(false);
        this.snack.open('Accesso effettuato', 'Chiudi', { duration: 2500 });
        void this.router.navigate(['/sheets']);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Credenziali non valide');
      },
    });
  }
}
