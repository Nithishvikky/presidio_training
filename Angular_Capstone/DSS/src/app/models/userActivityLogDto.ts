export interface UserActivityLogDto {
  id: string;
  userId: string;
  userEmail: string;
  userUsername: string;
  activityType: string; // "Login", "Logout", "DocumentView", "DocumentShare", "DocumentUpload", etc.
  description: string;
  timestamp: string; // ISO date string
}

export interface UserActivitySummaryDto {
  userId: string;
  userEmail: string;
  totalActivities: number;
  lastActivity: string;
  isActive: boolean; // Based on 30-day activity
  recentActivities: UserActivityLogDto[];
  activityTypeBreakdown: ActivityTypeBreakdown[];
}

export interface ActivityTypeBreakdown {
  activityType: string;
  count: number;
}

export interface CreateActivityLogDto {
  activityType: string;
  description: string;
  userId?: string; // Optional, will be set from token if not provided
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface ErrorObjectDto {
  errorNumber: number;
  errorMessage: string;
}
