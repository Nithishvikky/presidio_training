import { UserResponseDto } from "./userResponseDto";

export class PagedResponseDto{
  constructor(
    public pageNumber:number=1,
    public pageSize:number=0,
    public totalCount:number=0,
    public totalPages:number=0,
    public Users:UserResponseDto[] | null = null
  ){}
}
