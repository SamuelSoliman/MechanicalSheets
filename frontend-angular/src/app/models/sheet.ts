import { SheetStatus } from './sheet-status';
import { DefectItemResponse } from './defect-item';
import { UserSummary } from './user';

// Specchio di SheetResponseDto.cs. InspectionDate = DateOnly → string 'YYYY-MM-DD'.
export interface SheetResponse {
  id: number;
  code: string;
  brand?: string | null;
  vehicle?: string | null;
  inspectionDate: string;
  status: SheetStatus;
  rejectionNote?: string | null;
  submittedAt?: string | null;
  reviewedAt?: string | null;
  createdAt: string;
  createdBy: UserSummary;
  reviewedBy?: UserSummary | null;
  technicians: UserSummary[];
  defectItems: DefectItemResponse[];
}

// Specchio di CreateSheetDto.cs — TechnicianIds obbligatorio (anche vuoto).
export interface CreateSheetDto {
  code: string;
  brand?: string | null;
  vehicle?: string | null;
  inspectionDate: string;
  technicianIds: number[];
}

// Specchio di UpdateSheetDto.cs — tutto opzionale, solo Draft/Rejected.
export interface UpdateSheetDto {
  code?: string;
  brand?: string | null;
  vehicle?: string | null;
  inspectionDate?: string;
  technicianIds?: number[];
}

// Specchio di RejectSheetDto.cs — nota obbligatoria lato UI.
export interface RejectSheetDto {
  rejectionNote: string;
}
