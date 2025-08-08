export interface NotificationDto {
  id: string;
  entityName: string;
  entityId: string;
  content: string;
  createdAt: string;
  isRead: boolean;
  readAt: string | null;
}

export interface NotificationResponse {
  $id: string;
  success: boolean;
  data: {
    $id: string;
    $values: NotificationDto[];
  };
  error: string | null;
}

export interface UnreadCountDto {
  count: number;
}

export interface ApiResponseModel<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
} 