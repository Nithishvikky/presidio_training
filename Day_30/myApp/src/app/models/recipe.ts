export class RecipeModel
{
  constructor(
    public id:number=0,
    public name:string="",
    public cuisine:string="",
    public cookTimeMinutes:number=0,
    public image:string="",
    public ingredients:string[] | null= null
  ) {}
}
