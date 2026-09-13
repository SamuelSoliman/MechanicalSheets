import { Component, OnInit, computed, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth.service';
import { SheetService } from '../../../core/sheet.service';
import { SheetResponse } from '../../../models/sheet';
import { SheetStatus, isEditable, isReviewable } from '../../../models/sheet-status';
import { StatusBadge } from '../../../shared/badge-status/status-badge';
import { RejectDialog } from '../dialogs/reject-dialog';

// Vue: <SheetsList.vue> con Pinia + v-for + v-if per ruolo/stato.
// Filtri client-side come vanilla renderSheets(): status esatto o 'all'.
type Filter = 'all' | SheetStatus;

const FILTER_LABELS: Record<Filter, string> = {
  all: 'Tutte',
  Draft: 'Bozze',
  Submitted: 'Inviate',
  Approved: 'Approvate',
  Rejected: 'Rifiutate',
  Closed: 'Chiuse',
};

@Component({
  selector: 'app-sheets-list',
  standalone: true,
  imports: [
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    StatusBadge,
  ],
  templateUrl: './sheets-list.html',
  styleUrl: './sheets-list.scss',
})
export class SheetsList implements OnInit {
  readonly sheets = signal<SheetResponse[]>([]);
  readonly loading = signal(true);
  readonly loadError = signal<string | null>(null);
  readonly filter = signal<Filter>('all');
  readonly filterLabels = FILTER_LABELS;

  // Tab disponibili per ruolo (stessi del vanilla buildTabs).
  readonly tabs = computed<Filter[]>(() =>
    this.auth.isManager()
      ? ['all', 'Submitted', 'Approved', 'Rejected', 'Closed']
      : ['all', 'Draft', 'Submitted', 'Rejected', 'Approved'],
  );

  readonly title = computed(() =>
    this.auth.isManager() ? 'Tutte le schede' : 'Le mie schede',
  );

  readonly filtered = computed(() => {
    const f = this.filter();
    const all = this.sheets();
    return f === 'all' ? all : all.filter((s) => s.status === f);
  });

  readonly pendingCount = computed(
    () => this.sheets().filter((s) => isReviewable(s.status)).length,
  );

  constructor(
    public auth: AuthService,
    private sheetsApi: SheetService,
    private router: Router,
    private snack: MatSnackBar,
    private dialog: MatDialog,
  ) {}

  ngOnInit(): void {
    this.reload();
  }

  setFilter(f: Filter): void {
    this.filter.set(f);
  }

  reload(): void {
    this.loading.set(true);
    this.loadError.set(null);
    this.sheetsApi.getSheets().subscribe({
      next: (list) => {
        this.sheets.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set('Errore caricamento schede');
        this.loading.set(false);
      },
    });
  }

  openDetail(id: number): void {
    void this.router.navigate(['/sheets', id]);
  }

  createNew(): void {
    void this.router.navigate(['/sheets/new']);
  }

  canEdit(s: SheetResponse): boolean {
    return this.auth.isMechanic() && isEditable(s.status);
  }

  canSubmit(s: SheetResponse): boolean {
    return this.auth.isMechanic() && isEditable(s.status);
  }

  canReview(s: SheetResponse): boolean {
    return this.auth.isManager() && isReviewable(s.status);
  }

  submit(id: number): void {
    if (!confirm('Inviare la scheda per revisione?')) return;
    this.sheetsApi.submitSheet(id).subscribe({
      next: () => {
        this.snack.open('Scheda inviata', 'Chiudi', { duration: 2500 });
        this.reload();
      },
    });
  }

  approve(id: number): void {
    if (!confirm('Approvare questa scheda?')) return;
    this.sheetsApi.approveSheet(id).subscribe({
      next: () => {
        this.snack.open('Scheda approvata ✓', 'Chiudi', { duration: 2500 });
        this.reload();
      },
    });
  }

  // Dialog Material (come modal-reject del vanilla) — sostituisce il prompt() iniziale.
  reject(id: number): void {
    this.dialog
      .open(RejectDialog, { width: '480px' })
      .afterClosed()
      .subscribe((result: { rejectionNote: string } | undefined) => {
        if (!result) return;
        this.sheetsApi.rejectSheet(id, result).subscribe({
          next: () => {
            this.snack.open('Scheda rifiutata', 'Chiudi', { duration: 2500 });
            this.reload();
          },
        });
      });
  }
}
