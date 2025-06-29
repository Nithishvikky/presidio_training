export class PasswordChangeDto{
  constructor(
    public OldPassword:string="",
    public NewPassword:string=""
  ){}
}
