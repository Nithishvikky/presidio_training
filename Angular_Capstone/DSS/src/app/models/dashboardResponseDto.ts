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
