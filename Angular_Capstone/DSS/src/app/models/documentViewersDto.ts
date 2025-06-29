export class DocumentViewersDto{
  constructor(
    public docViewId:string="",
    public viewerName:string="",
    public fileName:string="",
    public viewedAt:Date
  ){}
}
