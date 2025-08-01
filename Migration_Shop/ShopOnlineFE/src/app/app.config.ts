import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { CategoryService } from './Services/Category.service';
import { ColorService } from './Services/Color.service';
import { CartService } from './Services/Cart.service';
import { OrderService } from './Services/Order.service';
import { PaymentService } from './Services/payment.service';
import { ContactUsComponent } from './Components/contact-us-component/contact-us-component';
import { ContactUsService } from './Services/ContactUs.service';
import { NewsManagementComponent } from './Components/news-management-component/news-management-component';
import { NewsManagementService } from './Services/NewsManagement.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    CategoryService,
    ColorService,
    CartService,
    OrderService,
    PaymentService,
    ContactUsService,
    NewsManagementService
  ]
};
