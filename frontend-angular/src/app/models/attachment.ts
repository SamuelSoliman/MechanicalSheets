// Specchio di AttachmentResponseDto.cs
// Url = '/api/files/{storedName}' — il download richiede JWT (vedi FilesController).
export interface AttachmentResponse {
  id: number;
  fileName: string;
  mimeType: string;
  fileSize: number;
  uploadedAt: string;
  uploadedBy: string;
  url: string;
}
