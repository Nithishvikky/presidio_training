export class UserModel
{

  constructor(
    public id:number= 0,
    public username:string="",
    public email:string= "",
    public firstName:string= "",
    public lastName:string= "",
    public gender:string= "",
    public image:string= ""
  ){}

  static fromForm(
    data:{
      id:number,
      username:string,
      email:string,
      firstName:string,
      lastName:string,
      gender:string,
      image: string
    }
  )
  {
    return new UserModel(data.id,data.username,data.email,data.firstName,data.lastName,data.gender,data.image);
  }
}
