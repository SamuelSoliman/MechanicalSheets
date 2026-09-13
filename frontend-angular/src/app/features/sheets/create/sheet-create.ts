import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { SheetService } from '../../../core/sheet.service';

// Vue: view-create in index.html (new-code*, new-brand, new-vehicle, new-date* → createSheet).
// TechnicianIds non ha UI nel vanilla → inviato [] come lui.
@Component({
  selector: 'app-sheet-create',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    RouterLink,
  ],
  templateUrl: './sheet-create.html',
  styleUrl: './sheet-create.scss',
})
export class SheetCreate {
  private readonly fb = new FormBuilder().nonNullable;

  readonly form = this.fb.group({
    code: ['', [Validators.required]],
    brand: [''],
    vehicle: [''],
    inspectionDate: ['', [Validators.required]],
  });
  readonly saving = signal(false);

  constructor(
    private sheetsApi: SheetService,
    private router: Router,
    private snack: MatSnackBar,
  ) {}

  save(): void {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);
    const v = this.form.getRawValue();
    this.sheetsApi
      .createSheet({
        code: v.code.trim(),
        brand: v.brand.trim() ? v.brand.trim() : null,
        vehicle: v.vehicle.trim() ? v.vehicle.trim() : null,
        inspectionDate: v.inspectionDate,
        technicianIds: [],
      })
      .subscribe({
        next: (created) => {
          this.saving.set(false);
          this.snack.open('Scheda creata', 'Chiudi', { duration: 2500 });
          void this.router.navigate(['/sheets', created.id]);
        },
        error: () => this.saving.set(false),
      });
  }
}
