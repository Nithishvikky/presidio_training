export interface UserDocDetailDto {
  id: string;
  userId: string;
  documentId: string;
  documentName: string;
  documentType: string;
  fileSize: number;
  uploadDate: string; 
  lastModified: string;
  status: string; 
  isArchived: boolean;
  archivedAt?: string; 
  accessExpiresAt?: string; 
  canRequestAccess: boolean; 
  hasActiveRequest: boolean; 
} 