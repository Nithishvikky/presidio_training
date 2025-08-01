export interface CreateUserRequestDto {
  documentId: string;
  requestType: string;
  reason: string;
  accessDurationHours?: number;
}

export interface UserRequestDto {
  id: string;
  userId: string;
  userEmail: string;
  userUsername: string;
  documentId: string;
  documentFileName: string;
  requestType: string;
  status: string; // "Pending", "Approved", "Rejected"
  reason: string;
  requestedAt: string; // ISO date string
  processedAt?: string; // ISO date string
  accessGrantedAt?: string; // ISO date string
  accessExpiresAt?: string; // ISO date string
  accessDurationHours?: number;
}

export interface ApproveUserRequestDto {
  accessDurationHours?: number;
}

export interface RejectUserRequestDto {
  reason: string;
} 