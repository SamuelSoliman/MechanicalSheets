// Specchio di UserSummaryDto.cs + claim JWT.
// Ruoli = stringhe semplici 'mechanic' | 'manager' (scelta intenzionale, vedi PROJECT.md).
export type UserRole = 'mechanic' | 'manager';

export interface UserSummary {
  id: number;
  name: string;
  email: string;
  role: UserRole;
}
