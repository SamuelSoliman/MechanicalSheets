import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';

// Vue: <Nav.vue> con store user + $router. Qui: toolbar Material con signal() user.
// Visibile solo se loggati (App la mostra con @if) — come il vanilla `#app > nav`.
@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [MatToolbarModule, MatButtonModule],
  templateUrl: './nav.html',
  styleUrl: './nav.scss',
})
export class Nav {
  constructor(
    public auth: AuthService,
    private router: Router,
  ) {}

  logout(): void {
    this.auth.logout();
    void this.router.navigate(['/login']);
  }
}
