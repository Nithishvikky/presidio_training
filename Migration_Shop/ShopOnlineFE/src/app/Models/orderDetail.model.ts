import { Product } from "./product.model";

export interface OrderDetail {
  orderID: number;
  productID: number;
  quantity: number;
  price: number;
  product?: Product;
}
