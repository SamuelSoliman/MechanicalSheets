import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';
import { Nav } from './shared/nav/nav';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Nav],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  constructor(public auth: AuthService) {}
}
