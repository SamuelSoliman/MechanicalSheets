import { Component, Input } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { SHEET_STATUS_LABELS, SheetStatus } from '../../models/sheet-status';

// Vue: filtro `statusLabel` + classe CSS. Qui: piccolo componente con mat-chip.
// Mappa vanilla `badge(status)` → label italiana + colore per stato.
@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [MatChipsModule],
  template: `<mat-chip [class]="'status-' + statusClass">{{ label }}</mat-chip>`,
  styles: [
    `
      .status-draft {
        background: #f0f0f0;
        color: #666;
      }
      .status-submitted {
        background: #fff3cd;
        color: #856404;
      }
      .status-approved {
        background: #d4edda;
        color: #155724;
      }
      .status-rejected {
        background: #f8d7da;
        color: #721c24;
      }
      .status-closed {
        background: #d1ecf1;
        color: #0c5460;
      }
    `,
  ],
})
export class StatusBadge {
  @Input({ required: true }) status!: SheetStatus;

  get label(): string {
    return SHEET_STATUS_LABELS[this.status] ?? this.status;
  }

  get statusClass(): string {
    return (this.status ?? '').toLowerCase();
  }
}
