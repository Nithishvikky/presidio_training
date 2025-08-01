# Frontend API Reference - Document Sharing System

## Base Configuration
- **Base URL**: `https://localhost:7001/api` (or your configured URL)
- **Authentication**: Bearer Token in Authorization header
- **Content-Type**: `application/json`

---

## 1. NOTIFICATION SYSTEM APIs

### 1.1 Get User Notifications
```http
GET /notifications/user
Authorization: Bearer {token}
```

**Response Model:**
```typescript
interface NotificationDto {
  id: Guid;
  title: string;
  message: string;
  type: string; // "Info", "Warning", "Error", "Success"
  isRead: boolean;
  createdAt: string; // ISO date string
  userId: Guid;
}

interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

### 1.2 Mark Notification as Read
```http
PUT /notifications/{id}/read
Authorization: Bearer {token}
```

**Response:**
```typescript
interface ApiResponseModel<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}
```

### 1.3 Mark All Notifications as Read
```http
PUT /notifications/user/read-all
Authorization: Bearer {token}
```

### 1.4 Delete Notification
```http
DELETE /notifications/{id}
Authorization: Bearer {token}
```

### 1.5 Get Unread Count
```http
GET /notifications/user/unread-count
Authorization: Bearer {token}
```

**Response:**
```typescript
interface UnreadCountDto {
  count: number;
}
```

---

## 2. USER ACTIVITY LOG APIs

### 2.1 Get User Activity Logs
```http
GET /user-activity-logs/user
Authorization: Bearer {token}
```

**Query Parameters:**
- `pageNumber` (int, optional): Default 1
- `pageSize` (int, optional): Default 10

**Response Model:**
```typescript
interface UserActivityLogDto {
  id: Guid;
  userId: Guid;
  activityType: string; // "Login", "Logout", "DocumentView", "DocumentShare", etc.
  description: string;
  timestamp: string; // ISO date string
  userEmail: string;
}

interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

### 2.2 Get User Dashboard Activity
```http
GET /user-activity-logs/user/dashboard
Authorization: Bearer {token}
```

**Response Model:**
```typescript
interface DashboardActivityDto {
  lastLoginDate: string; // ISO date string
  totalLogins: number;
  recentActivities: UserActivityLogDto[];
  isActive: boolean; // Based on 30-day activity
}
```

---

## 3. DOCUMENT REQUEST & ARCHIVE ACCESS APIs

### 3.1 Request Access to Archived Document
```http
POST /user-requests
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Model:**
```typescript
interface CreateUserRequestDto {
  userId: Guid;
  reason: string;
  accessDurationHours?: number; // Optional, defaults to 24
}
```

**Response Model:**
```typescript
interface UserRequestDto {
  id: Guid;
  userId: Guid;
  userEmail: string;
  status: string; // "Pending", "Approved", "Rejected"
  reason: string;
  requestedAt: string; // ISO date string
  processedAt?: string; // ISO date string
  accessGrantedAt?: string; // ISO date string
  accessExpiresAt?: string; // ISO date string
  accessDurationHours?: number;
}
```

### 3.2 Get User's Document Requests
```http
GET /user-requests/user
Authorization: Bearer {token}
```

**Query Parameters:**
- `pageNumber` (int, optional): Default 1
- `pageSize` (int, optional): Default 10
- `status` (string, optional): Filter by status

### 3.3 Get All Document Requests (Admin Only)
```http
GET /user-requests
Authorization: Bearer {token}
```

**Query Parameters:**
- `pageNumber` (int, optional): Default 1
- `pageSize` (int, optional): Default 10
- `status` (string, optional): Filter by status
- `userId` (Guid, optional): Filter by user

### 3.4 Approve Document Request (Admin Only)
```http
PUT /user-requests/{id}/approve
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Model:**
```typescript
interface ApproveUserRequestDto {
  accessDurationHours?: number; // Optional, defaults to 24
}
```

### 3.5 Reject Document Request (Admin Only)
```http
PUT /user-requests/{id}/reject
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Model:**
```typescript
interface RejectUserRequestDto {
  reason: string; // Required rejection reason
}
```

### 3.6 Get Request Details
```http
GET /user-requests/{id}
Authorization: Bearer {token}
```

---

## 4. ENHANCED DOCUMENT APIs

### 4.1 Get User Documents (Including Archive Status)
```http
GET /user-docs
Authorization: Bearer {token}
```

**Query Parameters:**
- `pageNumber` (int, optional): Default 1
- `pageSize` (int, optional): Default 10
- `status` (string, optional): "Active", "Archived", "TemporaryAccess"

**Enhanced Response Model:**
```typescript
interface UserDocDetailDto {
  id: Guid;
  userId: Guid;
  documentId: Guid;
  documentName: string;
  documentType: string;
  fileSize: number;
  uploadDate: string; // ISO date string
  lastModified: string; // ISO date string
  status: string; // "Active", "Archived", "TemporaryAccess"
  isArchived: boolean;
  archivedAt?: string; // ISO date string
  accessExpiresAt?: string; // ISO date string
  canRequestAccess: boolean; // True if archived and no pending request
  hasActiveRequest: boolean; // True if user has pending/approved request
}
```

### 4.2 Get Archived Documents
```http
GET /user-docs/archived
Authorization: Bearer {token}
```

**Query Parameters:**
- `pageNumber` (int, optional): Default 1
- `pageSize` (int, optional): Default 10

---

## 5. AUTHENTICATION APIs (Enhanced)

### 5.1 Login (Enhanced with Activity Logging)
```http
POST /auth/login
Content-Type: application/json
```

**Request Model:**
```typescript
interface LoginRequestDto {
  email: string;
  password: string;
}
```

**Enhanced Response Model:**
```typescript
interface AuthResponseDto {
  success: boolean;
  message: string;
  data?: {
    token: string;
    refreshToken: string;
    expiresIn: number;
    user: {
      id: Guid;
      email: string;
      firstName: string;
      lastName: string;
      role: string;
      lastLogin: string; // ISO date string - NEW FIELD
    };
  };
  errors?: string[];
}
```

### 5.2 Get User Profile (Enhanced)
```http
GET /users/profile
Authorization: Bearer {token}
```

**Enhanced Response Model:**
```typescript
interface UserProfileDto {
  id: Guid;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  lastLogin: string; // ISO date string - NEW FIELD
  isActive: boolean; // Based on 30-day activity
  totalDocuments: number;
  archivedDocuments: number;
  pendingRequests: number;
}
```

---

## 6. DASHBOARD APIs

### 6.1 Get Dashboard Data
```http
GET /users/dashboard
Authorization: Bearer {token}
```

**Response Model:**
```typescript
interface DashboardDto {
  user: {
    id: Guid;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
    lastLogin: string; // ISO date string
    isActive: boolean;
  };
  statistics: {
    totalDocuments: number;
    activeDocuments: number;
    archivedDocuments: number;
    pendingRequests: number;
    unreadNotifications: number;
  };
  recentActivity: UserActivityLogDto[];
  recentNotifications: NotificationDto[];
  upcomingExpirations: UserDocDetailDto[]; // Documents with temporary access expiring soon
}
```

---

## 7. FRONTEND DATA MODELS

### 7.1 Core Models
```typescript
// User Models
interface User {
  id: Guid;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  lastLogin: string;
  isActive: boolean;
}

interface UserProfile extends User {
  totalDocuments: number;
  archivedDocuments: number;
  pendingRequests: number;
}

// Document Models
interface Document {
  id: Guid;
  name: string;
  type: string;
  size: number;
  uploadDate: string;
  lastModified: string;
  status: 'Active' | 'Archived' | 'TemporaryAccess';
  isArchived: boolean;
  archivedAt?: string;
  accessExpiresAt?: string;
  canRequestAccess: boolean;
  hasActiveRequest: boolean;
}

// Notification Models
interface Notification {
  id: Guid;
  title: string;
  message: string;
  type: 'Info' | 'Warning' | 'Error' | 'Success';
  isRead: boolean;
  createdAt: string;
  userId: Guid;
}

// Activity Models
interface ActivityLog {
  id: Guid;
  userId: Guid;
  activityType: string;
  description: string;
  timestamp: string;
  userEmail: string;
}

// Request Models
interface DocumentRequest {
  id: Guid;
  userId: Guid;
  userEmail: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  reason: string;
  requestedAt: string;
  processedAt?: string;
  accessGrantedAt?: string;
  accessExpiresAt?: string;
  accessDurationHours?: number;
}

// Dashboard Models
interface Dashboard {
  user: UserProfile;
  statistics: {
    totalDocuments: number;
    activeDocuments: number;
    archivedDocuments: number;
    pendingRequests: number;
    unreadNotifications: number;
  };
  recentActivity: ActivityLog[];
  recentNotifications: Notification[];
  upcomingExpirations: Document[];
}
```

### 7.2 API Response Models
```typescript
interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

---

## 8. FRONTEND IMPLEMENTATION NOTES

### 8.1 Authentication Flow
1. Store JWT token in localStorage or secure storage
2. Include token in Authorization header for all API calls
3. Handle token expiration with refresh token
4. Redirect to login on authentication failure

### 8.2 Real-time Updates
- Implement WebSocket connection for real-time notifications
- Use SignalR hub connection for live updates
- Poll for updates every 30 seconds as fallback

### 8.3 Error Handling
- Handle 401 Unauthorized by redirecting to login
- Handle 403 Forbidden by showing access denied message
- Handle 500 errors with user-friendly messages
- Implement retry logic for network failures

### 8.4 State Management
- Use Redux, Zustand, or Context API for global state
- Cache user profile, notifications, and dashboard data
- Implement optimistic updates for better UX

### 8.5 UI Components Needed
- Notification bell with unread count
- Activity timeline component
- Document request modal/form
- Archive status indicators
- Dashboard widgets
- User activity chart/graph

### 8.6 Background Jobs (Frontend)
- No cron jobs needed on frontend
- Backend handles all scheduled tasks
- Frontend only needs to poll for updates or use WebSocket

---

## 9. EXAMPLE API CALLS

### 9.1 JavaScript/TypeScript Examples
```typescript
// Login
const login = async (email: string, password: string) => {
  const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  return response.json();
};

// Get notifications
const getNotifications = async (token: string) => {
  const response = await fetch('/api/notifications/user', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  return response.json();
};

// Request document access
const requestAccess = async (token: string, reason: string) => {
  const response = await fetch('/api/user-requests', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ reason })
  });
  return response.json();
};
```

### 9.2 React Hook Example
```typescript
const useNotifications = (token: string) => {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchNotifications = async () => {
      try {
        const response = await fetch('/api/notifications/user', {
          headers: { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        if (data.success) {
          setNotifications(data.data.items);
        }
      } catch (error) {
        console.error('Failed to fetch notifications:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchNotifications();
  }, [token]);

  return { notifications, loading };
};
```

---

This comprehensive API reference provides everything needed to implement the frontend for the notification system, user activity logging, and document request features. The backend is already set up with all the necessary endpoints and background services. 