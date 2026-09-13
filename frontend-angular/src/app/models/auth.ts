import { UserRole } from './user';

// Specchio di LoginDto.cs / RegisterDto.cs
export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  name: string;
  email: string;
  password: string;
  role: UserRole;
}

// POST /api/auth/login ritorna { token }
export interface LoginResponse {
  token: string;
}

// Utente decodificato dal JWT (payload con claim MS: role, emailaddress, givenname).
// Equivalente di quanto fa index.html con parseJwt() + normalizeRole().
export interface AuthUser {
  id: number;
  email: string;
  name: string;
  role: UserRole;
}
