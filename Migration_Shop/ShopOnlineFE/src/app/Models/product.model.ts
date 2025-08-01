import { Category } from "./category.model";
import { Color } from "./color.model";
import { Model } from "./model.model";
import { User } from "./user.model";

export interface Product {
  productId: number;
  productName: string;
  image: string;
  price: number;
  userId: number;
  categoryId: number;
  colorId: number;
  modelId: number;
  storageId: number;
  sellStartDate: string;
  sellEndDate: string;
  isNew: number;
  category?: Category;
  color?: Color;
  model?: Model;
  user?: User;
}
