export class DashBoardResponseDto{
  constructor(
    public totalUsers:number=0,
    public totalAdmin:number=0,
    public totalUserRole:number=0,
    public totalDocuments:number=0,
    public totalShared:number=0,
    public totalViews:number=0,
  ){}
}
export interface DocumentDateCountDto {
  date: string;
  count: number;
}
export interface ApiResponse<T> {
  data: {
    $values: T[];
  };
  success: boolean;
}

export interface UserCountDto{
  activeCount:number;
  inactiveCount:number;
}

export interface TopSharedDocumentDto {
  documentId: string;
  fileName: string;
  owner:string;
  shareCount: number;
}

export interface DocumentTypeCountDto {
  contentType: string;
  count: number;
}
