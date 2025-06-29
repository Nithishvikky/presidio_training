export class NotificationResponseDto{
  constructor(
    public docViewId:string="",
    public viewerName:string="",
    public viewerEmail:string="",
    public fileName:string="",
    public viewedAt:Date
  ){}
}
