import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, Observer } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';


/**
 * This class adds the auth token to the header of requests made by the application.
 */
@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(
    private authService: AuthService,
    private router: Router    
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // The interceptor recieves the request and forwards it in the next HttpHandler. 
    // The HttpHandler is a class that manages the chaining of interceptors.

    if (request.url.endsWith("/api/login")) {
      // If the login is requested dont add auth header
      return next.handle(request);
    }   



    // Send token with request if present
    const token = localStorage.getItem(this.authService.tokenName);
    if (token) {
      const modifiedRequest = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });

      console.log("Auth interceptor called.", request);

      // Creating an error event handler object
      const eventObject: Partial<Observer<HttpEvent<any>>> = {
        error: (err: HttpErrorResponse) => {
          console.log("Auth intercepter received error response: ", err.status);
          if (err.status === 401 || err.status === 403) {
            this.authService.logout();
            this.router.navigateByUrl("/login");

            console.log("Auth interceptor is routing back to login due to 401 or 403.");
          }
        }
      }

      const observable = next.handle(modifiedRequest);
      observable.subscribe(eventObject);
      return observable;
    }

    else {
      // If there is no token also go back to login
      this.router.navigateByUrl("/login");
      console.log("Auth interceptor is routing back to login due to missing token.");
      return new Observable<HttpEvent<any>>(); // Dont forward the request
    }
  }
}
