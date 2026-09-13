import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DefectCatalogService } from '../../../core/defect-catalog.service';
import { DefectCatalog } from '../../../models/defect-catalog';
import { CreateDefectItemDto } from '../../../models/defect-item';

// Vue: modal-difetto in index.html (select catalogo + checkbox extent/intensity/flag + note).
// Qui: MatDialog che restituisce CreateDefectItemDto.
// Nota: il vanilla invia anche `gravity`, ma CreateDefectItemDto non lo prevede
// (la gravità in risposta viene dal catalogo) — campo omesso di proposito.
@Component({
  selector: 'app-defect-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCheckboxModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './defect-dialog.html',
})
export class DefectDialog implements OnInit {
  private readonly fb = new FormBuilder().nonNullable;

  readonly form = this.fb.group({
    defectCatalogId: [0, [Validators.required, Validators.min(1)]],
    isSeen: [false],
    extentLow: [false],
    extentMedium: [false],
    extentHigh: [false],
    intensityLow: [false],
    intensityMedium: [false],
    intensityHigh: [false],
    isPs: [false],
    isNa: [false],
    isNr: [false],
    isNp: [false],
    notes: [''],
  });

  readonly catalog = signal<DefectCatalog[]>([]);
  readonly loadingCatalog = signal(true);

  constructor(
    private dialogRef: MatDialogRef<DefectDialog, CreateDefectItemDto>,
    private catalogApi: DefectCatalogService,
  ) {}

  ngOnInit(): void {
    this.catalogApi.getCatalog().subscribe({
      next: (list) => {
        this.catalog.set(list);
        this.loadingCatalog.set(false);
      },
      error: () => this.loadingCatalog.set(false),
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }

  save(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    this.dialogRef.close({
      defectCatalogId: v.defectCatalogId,
      isSeen: v.isSeen,
      extentLow: v.extentLow,
      extentMedium: v.extentMedium,
      extentHigh: v.extentHigh,
      intensityLow: v.intensityLow,
      intensityMedium: v.intensityMedium,
      intensityHigh: v.intensityHigh,
      isPs: v.isPs,
      isNa: v.isNa,
      isNr: v.isNr,
      isNp: v.isNp,
      notes: v.notes.trim() ? v.notes.trim() : null,
    });
  }
}
