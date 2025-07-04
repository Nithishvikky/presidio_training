export class NotificationSharedResponseDto{
  constructor(
    public fileName:string="",
    public email:string="",
    public userName:string="",
    public grantedAt:Date
  ){}
}
