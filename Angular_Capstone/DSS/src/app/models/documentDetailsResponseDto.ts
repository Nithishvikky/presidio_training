export class DocumentDetailsResponseDto{
  constructor(
    public docId:string="",
    public fileName:string="",
    public size:number,
    public uploadedAt:Date,
    public contentType:string="",
    public lastViewerName:string="",
    public uploaderEmail:string ="",
    public uploaderUsername:string="",
    public status:string="Active",
    public archivedAt?:Date,
    public fileData?:any,
    public scheduledRearchiveAt:string=""
  ){}
}
