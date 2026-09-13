import { DefectCategory } from './defect-catalog';

// Specchio di DefectItemResponseDto.cs (SheetService.MapDefectItemToDto).
export interface DefectItemResponse {
  id: number;
  defectCatalogId: number;
  defectCode: string;
  defectCategory: DefectCategory;
  defectDescription: string;
  isSeen: boolean;
  gravity: number;
  extentLow: boolean;
  extentMedium: boolean;
  extentHigh: boolean;
  intensityLow: boolean;
  intensityMedium: boolean;
  intensityHigh: boolean;
  isPs: boolean;
  isNa: boolean;
  isNr: boolean;
  isNp: boolean;
  hasPhoto: boolean;
  notes?: string | null;
}

// Specchio di CreateDefectItemDto.cs — POST /api/sheets/{id}/defects (mechanic).
export interface CreateDefectItemDto {
  defectCatalogId: number;
  isSeen: boolean;
  extentLow?: boolean;
  extentMedium?: boolean;
  extentHigh?: boolean;
  intensityLow?: boolean;
  intensityMedium?: boolean;
  intensityHigh?: boolean;
  isPs?: boolean;
  isNa?: boolean;
  isNr?: boolean;
  isNp?: boolean;
  hasPhoto?: boolean;
  notes?: string | null;
}
