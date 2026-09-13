import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CreateSheetDto,
  RejectSheetDto,
  SheetResponse,
  UpdateSheetDto,
} from '../models/sheet';

// Vue: modulo API `sheets.js` con fetch(). Qui: service root con HttpClient
// (JWT aggiunto da auth.interceptor, errori da error.interceptor).
// Rotte: SheetsController — GET lista (manager=tutte, mechanic=solo proprie),
// GET dettaglio (404 se di altro mechanic), POST/PUT mechanic, submit mechanic,
// approve/reject manager. Strict vanilla parity: niente PUT-edit UI qui, solo chiamata.
@Injectable({ providedIn: 'root' })
export class SheetService {
  private readonly base = `${environment.apiUrl}/sheets`;

  constructor(private http: HttpClient) {}

  getSheets(): Observable<SheetResponse[]> {
    return this.http.get<SheetResponse[]>(this.base);
  }

  getSheet(id: number): Observable<SheetResponse> {
    return this.http.get<SheetResponse>(`${this.base}/${id}`);
  }

  createSheet(dto: CreateSheetDto): Observable<SheetResponse> {
    return this.http.post<SheetResponse>(this.base, dto);
  }

  updateSheet(id: number, dto: UpdateSheetDto): Observable<SheetResponse> {
    return this.http.put<SheetResponse>(`${this.base}/${id}`, dto);
  }

  submitSheet(id: number): Observable<SheetResponse> {
    return this.http.post<SheetResponse>(`${this.base}/${id}/submit`, {});
  }

  approveSheet(id: number): Observable<SheetResponse> {
    return this.http.post<SheetResponse>(`${this.base}/${id}/approve`, {});
  }

  rejectSheet(id: number, dto: RejectSheetDto): Observable<SheetResponse> {
    return this.http.post<SheetResponse>(`${this.base}/${id}/reject`, dto);
  }
}
