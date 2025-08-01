import { Customer } from "./customer.model";
import { OrderDetail } from "./orderDetail.model";

export interface Order {
  orderID: number;
  orderName: string;
  orderDate: string;
  paymentType: string;
  status: string;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  customerAddress: string;
  orderDetails?: OrderDetail[];
}
