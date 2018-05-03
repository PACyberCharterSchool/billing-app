import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { AuthenticationService } from '../services/authentication.service';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(public auth: AuthenticationService) {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    const authToken = this.auth.getAuthenticationToken();
    if (authToken) {
      const modified = request.clone({
        setHeaders: {
          Authorization: `Bearer ${this.auth.getAuthenticationToken()}`
        }
      });
      return next.handle(modified);
    } else {
      return next.handle(request);
    }
  }
}
