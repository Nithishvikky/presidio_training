
export class UserResponseDto{
  constructor(
    public userId:string = "",
    public email:string="",
    public username:string="",
    public role:string="",
    public registeredAt?:Date,
    public updatedAt?:Date,
    public documentCount:number = 0,
  ){}
}

