export class UserModel{
  constructor(
    public firstName:string = "",
    public lastName:string = "",
    public age:number = 0,
    public gender:string = "",
    public email:string ="",
    public password:string = "",
    public role:string = "",
    public state:string = ""
  ){}
}
