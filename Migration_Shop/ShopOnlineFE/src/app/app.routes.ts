import { Routes } from '@angular/router';
import { LayoutComponent } from './Components/layout-component/layout-component';
import { ColorComponent } from './Components/color-component/color-component';
import { ProductDetailComponent } from './Components/product-detail-component/product-detail-component';
import { ProductListComponent } from './Components/product-list-component/product-list-component';
import { CartComponent } from './Components/cart-component/cart-component';
import { OrderListComponent } from './Components/order-list-component/order-list-component';
import { OrderDetailComponent } from './Components/order-detail-component/order-detail-component';
import { ContactUsComponent } from './Components/contact-us-component/contact-us-component';
import { NewsManagementComponent } from './Components/news-management-component/news-management-component';
import { NewsComponent } from './Components/news-component/news-component';
import { PaymentSuccessComponent } from './Components/payment-success/payment-success';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      // { path: 'home', component: HomeComponent },
      { path: 'news', component: NewsComponent },
      { path: 'products', component: ProductListComponent },
      { path: 'product/:id', component: ProductDetailComponent },
      { path: 'contact', component: ContactUsComponent },
      { path: 'colors', component: ColorComponent },
      { path: 'news-management', component: NewsManagementComponent },
      { path: 'cart', component: CartComponent },
      { path: 'order', component: OrderListComponent },
      { path: 'orders/:id', component: OrderDetailComponent },
      { path: 'payment-success', component: PaymentSuccessComponent },
      { path: '', redirectTo: 'home', pathMatch: 'full' }
    ]
  }
];
