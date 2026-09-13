import { Component, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';

// Vue: modal-foto in index.html (file input + submitPhoto con FormData).
// Qui: MatDialog che restituisce il File scelto (upload nel componente chiamante).
const ACCEPTED = ['image/jpeg', 'image/png', 'image/webp'];
const MAX_BYTES = 5 * 1024 * 1024;

@Component({
  selector: 'app-photo-dialog',
  standalone: true,
  imports: [MatButtonModule, MatDialogModule],
  templateUrl: './photo-dialog.html',
})
export class PhotoDialog {
  readonly fileName = signal<string | null>(null);
  readonly error = signal<string | null>(null);
  private file: File | null = null;

  constructor(private dialogRef: MatDialogRef<PhotoDialog, File>) {}

  onFile(event: Event): void {
    this.error.set(null);
    const input = event.target as HTMLInputElement;
    const picked = input.files?.[0] ?? null;
    if (!picked) return;
    if (!ACCEPTED.includes(picked.type)) {
      this.error.set('Formato non valido: solo JPEG, PNG, WEBP.');
      return;
    }
    if (picked.size > MAX_BYTES) {
      this.error.set('File troppo grande: max 5MB.');
      return;
    }
    this.file = picked;
    this.fileName.set(picked.name);
  }

  cancel(): void {
    this.dialogRef.close();
  }

  upload(): void {
    if (this.file) this.dialogRef.close(this.file);
  }
}
