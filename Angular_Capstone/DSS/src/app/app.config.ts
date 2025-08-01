import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withHashLocation } from '@angular/router';

import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { UserService } from './services/user.service';
import { AuthGuard } from './guards/auth-guard';
import { DocumentService } from './services/document.service';
import { AuthInterceptor } from './interceptors/auth-interceptor';
import { DocumentAccessService } from './services/documentAccess.service';
import { DocumentViewerService } from './services/documentView.service';
import { LoaderInterceptor } from './interceptors/loader-interceptor';
import { NotificationService } from './services/notification.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    // Use hash routing to avoid server-side routing issues
    provideRouter(routes, withHashLocation()),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoaderInterceptor,
      multi: true
    },
    UserService,
    DocumentService,
    DocumentAccessService,
    DocumentViewerService,
    NotificationService,
    AuthGuard
  ]
};
