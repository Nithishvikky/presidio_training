export class UserModel{
  constructor(
    public username:string = "",
    public age:number = 0,
    public gender:string = "",
    public email:string = "",
    public password:string = "",
    public role:string = "",
  ){}
}
