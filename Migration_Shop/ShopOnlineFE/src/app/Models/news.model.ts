export interface News {
  newsId?: number;
  userId: number;
  title: string;
  shortDescription: string;
  imageUrl: string; // used for create
  image?: string;   // returned in response
  content: string;
  createdDate: string;
  status: number;
  user?: {
    userId: number;
    username: string;
    password?: string;
    news?: any[];
    products?: any[];
  };
}

export interface PaginatedNews {
  items: News[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
