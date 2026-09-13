import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateDefectItemDto, DefectItemResponse } from '../models/defect-item';

// DefectItemsController @ api/sheets/{sheetId}/defects [mechanic].
// Nota: il vanilla JS invia anche `gravity`, ma CreateDefectItemDto non lo ha —
// la gravità in risposta viene dal catalogo. Strict parity: non lo inviamo.
@Injectable({ providedIn: 'root' })
export class DefectItemService {
  private readonly base = `${environment.apiUrl}/sheets`;

  constructor(private http: HttpClient) {}

  addDefect(sheetId: number, dto: CreateDefectItemDto): Observable<DefectItemResponse> {
    return this.http.post<DefectItemResponse>(`${this.base}/${sheetId}/defects`, dto);
  }

  deleteDefect(sheetId: number, itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${sheetId}/defects/${itemId}`);
  }
}
