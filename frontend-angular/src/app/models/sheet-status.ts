// Specchio di SheetStatusEnum.cs — serializzato come stringa dal backend
// (JsonStringEnumConverter + HasConversion<string>).
export type SheetStatus = 'Draft' | 'Submitted' | 'Approved' | 'Rejected' | 'Closed';

export const SHEET_STATUS_LABELS: Record<SheetStatus, string> = {
  Draft: 'Bozza',
  Submitted: 'Inviata',
  Approved: 'Approvata',
  Rejected: 'Rifiutata',
  Closed: 'Chiusa',
};

// Regole workflow (stesse di index.html + SheetService.cs) — usate per mostrare/nascondere bottoni.
export function isEditable(status: SheetStatus | string): boolean {
  const s = (status ?? '').toLowerCase();
  return s === 'draft' || s === 'rejected';
}

export function isReviewable(status: SheetStatus | string): boolean {
  return (status ?? '').toLowerCase() === 'submitted';
}
