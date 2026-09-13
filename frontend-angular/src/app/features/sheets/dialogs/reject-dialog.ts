import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

// Vue: modal-reject in index.html (reject-note obbligatoria → confirmReject).
// Qui: MatDialog che restituisce { rejectionNote }. Sostituisce il prompt() dello step 5.
export interface RejectResult {
  rejectionNote: string;
}

@Component({
  selector: 'app-reject-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './reject-dialog.html',
})
export class RejectDialog {
  private readonly fb = new FormBuilder().nonNullable;

  readonly form = this.fb.group({
    rejectionNote: ['', [Validators.required]],
  });
  readonly error = signal<string | null>(null);

  constructor(private dialogRef: MatDialogRef<RejectDialog, RejectResult>) {}

  cancel(): void {
    this.dialogRef.close();
  }

  confirm(): void {
    const note = this.form.getRawValue().rejectionNote.trim();
    if (!note) {
      this.error.set('La nota è obbligatoria');
      return;
    }
    this.dialogRef.close({ rejectionNote: note });
  }
}
