import { UserRequestDto } from "./userRequestDto";
import { UserResponseDto } from "./userResponseDto";

export interface PagedResponseDto<T> {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  data: {
    $values: T[];
  };
}
