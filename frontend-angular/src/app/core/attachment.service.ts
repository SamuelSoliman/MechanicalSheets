import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AttachmentResponse } from '../models/attachment';

// AttachmentsController + FilesController.
// - Lista/upload/delete: /api/sheets/{s}/defects/{d}/attachments (JWT via interceptor)
// - Download: GET AttachmentResponse.url (/api/files/{stored}) con JWT → blob.
// Vanilla: fetch(url, {Bearer}) → blob → URL.createObjectURL. Qui: HttpClient
// fa lo stesso (responseType blob), l'interceptor aggiunge già il Bearer.
@Injectable({ providedIn: 'root' })
export class AttachmentService {
  private readonly sheetsBase = `${environment.apiUrl}/sheets`;

  constructor(private http: HttpClient) {}

  listAttachments(sheetId: number, itemId: number): Observable<AttachmentResponse[]> {
    return this.http.get<AttachmentResponse[]>(
      `${this.sheetsBase}/${sheetId}/defects/${itemId}/attachments`,
    );
  }

  // Solo image/jpeg,png,webp, max 5MB — validato dal backend; FormData senza Content-Type.
  uploadAttachment(
    sheetId: number,
    itemId: number,
    file: File,
  ): Observable<AttachmentResponse> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<AttachmentResponse>(
      `${this.sheetsBase}/${sheetId}/defects/${itemId}/attachments`,
      form,
    );
  }

  deleteAttachment(sheetId: number, itemId: number, attachmentId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.sheetsBase}/${sheetId}/defects/${itemId}/attachments/${attachmentId}`,
    );
  }

  // Scarica il file come object URL locale per <img> / window.open (come vanilla loadPhotos).
  // `url` arriva già come '/api/files/...' → usabile direttamente (stessa origin + proxy).
  downloadAsObjectUrl(url: string): Observable<string> {
    return this.http.get(url, { responseType: 'blob' }).pipe(
      map((blob) => URL.createObjectURL(blob)),
    );
  }
}
