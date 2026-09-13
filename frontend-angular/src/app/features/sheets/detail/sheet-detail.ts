import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AttachmentService } from '../../../core/attachment.service';
import { AuthService } from '../../../core/auth.service';
import { DefectItemService } from '../../../core/defect-item.service';
import { SheetService } from '../../../core/sheet.service';
import { AttachmentResponse } from '../../../models/attachment';
import { DefectItemResponse } from '../../../models/defect-item';
import { SheetResponse } from '../../../models/sheet';
import { isEditable, isReviewable } from '../../../models/sheet-status';
import { StatusBadge } from '../../../shared/badge-status/status-badge';
import { DefectDialog } from '../dialogs/defect-dialog';
import { PhotoDialog } from '../dialogs/photo-dialog';
import { RejectDialog } from '../dialogs/reject-dialog';

// Foto caricata come object URL locale (vanilla loadPhotos: blob → URL.createObjectURL).
interface PhotoView {
  open: boolean;
  loading: boolean;
  items: { attachment: AttachmentResponse; url: string }[];
}

// Vue: view-detail in index.html (openDetail: info card + difetti + manager block).
// Qui: route /sheets/:id, ricarica dopo ogni azione come il vanilla.
@Component({
  selector: 'app-sheet-detail',
  standalone: true,
  imports: [
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    RouterLink,
    StatusBadge,
  ],
  templateUrl: './sheet-detail.html',
  styleUrl: './sheet-detail.scss',
})
export class SheetDetail implements OnInit, OnDestroy {
  readonly sheet = signal<SheetResponse | null>(null);
  readonly loading = signal(true);
  readonly loadError = signal<string | null>(null);
  readonly photos = signal<Record<number, PhotoView>>({});

  private sheetId = 0;

  constructor(
    public auth: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private sheetsApi: SheetService,
    private defectsApi: DefectItemService,
    private attachmentsApi: AttachmentService,
    private dialog: MatDialog,
    private snack: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.sheetId = Number(this.route.snapshot.paramMap.get('id'));
    this.reload();
  }

  ngOnDestroy(): void {
    // Evita leak degli object URL creati per le foto.
    for (const view of Object.values(this.photos())) {
      for (const item of view.items) URL.revokeObjectURL(item.url);
    }
  }

  reload(): void {
    this.loading.set(true);
    this.loadError.set(null);
    this.sheetsApi.getSheet(this.sheetId).subscribe({
      next: (s) => {
        this.sheet.set(s);
        this.photos.set({});
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set('Errore caricamento scheda');
        this.loading.set(false);
      },
    });
  }

  canEdit(): boolean {
    const s = this.sheet();
    return !!s && this.auth.isMechanic() && isEditable(s.status);
  }

  canReview(): boolean {
    const s = this.sheet();
    return !!s && this.auth.isManager() && isReviewable(s.status);
  }

  photoView(defectId: number): PhotoView | undefined {
    return this.photos()[defectId];
  }

  // Toggle "Vedi foto": prima apertura → lista allegati + download blob (come vanilla).
  togglePhotos(defect: DefectItemResponse): void {
    const current = this.photos()[defect.id];
    if (current?.open) {
      this.photos.set({ ...this.photos(), [defect.id]: { ...current, open: false } });
      return;
    }
    if (current && !current.open) {
      this.photos.set({ ...this.photos(), [defect.id]: { ...current, open: true } });
      return;
    }
    this.photos.set({ ...this.photos(), [defect.id]: { open: true, loading: true, items: [] } });
    this.attachmentsApi.listAttachments(this.sheetId, defect.id).subscribe({
      next: (list) => {
        if (list.length === 0) {
          this.photos.set({
            ...this.photos(),
            [defect.id]: { open: true, loading: false, items: [] },
          });
          return;
        }
        const downloads = list.map((a) =>
          this.attachmentsApi.downloadAsObjectUrl(a.url).pipe(
            catchError(() => of(null)),
          ),
        );
        forkJoin(downloads).subscribe((urls) => {
          const items = list
            .map((attachment, i) => ({ attachment, url: urls[i] }))
            .filter((x): x is { attachment: AttachmentResponse; url: string } => x.url !== null);
          this.photos.set({
            ...this.photos(),
            [defect.id]: { open: true, loading: false, items },
          });
        });
      },
      error: () => {
        this.photos.set({
          ...this.photos(),
          [defect.id]: { open: true, loading: false, items: [] },
        });
        this.snack.open('Errore caricamento foto', 'Chiudi', { duration: 3000 });
      },
    });
  }

  openDefectDialog(): void {
    this.dialog
      .open(DefectDialog, { width: '560px' })
      .afterClosed()
      .subscribe((dto) => {
        if (!dto) return;
        this.defectsApi.addDefect(this.sheetId, dto).subscribe({
          next: () => {
            this.snack.open('Difetto aggiunto', 'Chiudi', { duration: 2500 });
            this.reload();
          },
        });
      });
  }

  deleteDefect(itemId: number): void {
    if (!confirm('Eliminare questo difetto?')) return;
    this.defectsApi.deleteDefect(this.sheetId, itemId).subscribe({
      next: () => {
        this.snack.open('Difetto eliminato', 'Chiudi', { duration: 2500 });
        this.reload();
      },
    });
  }

  openPhotoDialog(defectId: number): void {
    this.dialog
      .open(PhotoDialog, { width: '440px' })
      .afterClosed()
      .subscribe((file: File | undefined) => {
        if (!file) return;
        this.attachmentsApi.uploadAttachment(this.sheetId, defectId, file).subscribe({
          next: () => {
            this.snack.open('Foto caricata', 'Chiudi', { duration: 2500 });
            this.reload();
          },
        });
      });
  }

  submit(): void {
    if (!confirm('Inviare la scheda per revisione?')) return;
    this.sheetsApi.submitSheet(this.sheetId).subscribe({
      next: () => {
        this.snack.open('Scheda inviata', 'Chiudi', { duration: 2500 });
        void this.router.navigate(['/sheets']);
      },
    });
  }

  approve(): void {
    if (!confirm('Approvare questa scheda?')) return;
    this.sheetsApi.approveSheet(this.sheetId).subscribe({
      next: () => {
        this.snack.open('Scheda approvata ✓', 'Chiudi', { duration: 2500 });
        void this.router.navigate(['/sheets']);
      },
    });
  }

  openRejectDialog(): void {
    this.dialog
      .open(RejectDialog, { width: '480px' })
      .afterClosed()
      .subscribe((result: { rejectionNote: string } | undefined) => {
        if (!result) return;
        this.sheetsApi.rejectSheet(this.sheetId, result).subscribe({
          next: () => {
            this.snack.open('Scheda rifiutata', 'Chiudi', { duration: 2500 });
            void this.router.navigate(['/sheets']);
          },
        });
      });
  }

  technicianNames(): string {
    return (this.sheet()?.technicians ?? []).map((t) => t.name).join(', ');
  }

  extentLabel(d: DefectItemResponse): string {    if (d.extentLow) return 'Ext:0.2';
    if (d.extentMedium) return 'Ext:0.5';
    if (d.extentHigh) return 'Ext:1';
    return '';
  }

  intensityLabel(d: DefectItemResponse): string {
    if (d.intensityLow) return 'Int:0.2';
    if (d.intensityMedium) return 'Int:0.5';
    if (d.intensityHigh) return 'Int:1';
    return '';
  }
}
