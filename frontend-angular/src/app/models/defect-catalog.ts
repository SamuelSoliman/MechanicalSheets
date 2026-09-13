// Specchio di DefectCategoryEnum.cs
export type DefectCategory = 'Bodywork' | 'Lighting' | 'Fasteners' | 'Frame';

// GET /api/defect-catalog → {Id, Code, Category, Description, Gravity}
export interface DefectCatalog {
  id: number;
  code: string;
  category: DefectCategory;
  description: string;
  gravity: number;
}
