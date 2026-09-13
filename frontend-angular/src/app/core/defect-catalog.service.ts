import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { DefectCatalog } from '../models/defect-catalog';

// GET /api/defect-catalog — qualsiasi ruolo autenticato.
// Riempie la select "[code] description" del modal difetto (vanilla: openDefectModal).
@Injectable({ providedIn: 'root' })
export class DefectCatalogService {
  private readonly base = `${environment.apiUrl}/defect-catalog`;

  constructor(private http: HttpClient) {}

  getCatalog(): Observable<DefectCatalog[]> {
    return this.http.get<DefectCatalog[]>(this.base);
  }
}
