import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { Observable, catchError, tap, throwError } from 'rxjs';
import jwt_decode, { JwtDecodeOptions } from "jwt-decode";
import { JwtClaims } from '../models/JwtClaims';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(
    private http: HttpClient
  ) { }
  
  readonly tokenName: string = "AuthToken";
  

  public login(user: User): Observable<any> {
    const postObservable = this.http.post<any>("https://localhost:7171/api/login", user);

    // Could also have used.subscribe, but then you would need to assign an error handler here
    // which im not interested in since i already have an error handler in the login component.
    const pipedObservable = postObservable.pipe(
      tap((response) => {
        console.log("Response: ", response);
        localStorage.setItem(this.tokenName, response.token);
      })
    );      
    
    return pipedObservable
  }

  public logout() {
    localStorage.removeItem(this.tokenName)
  }

  public getToken(): string | null {
    return localStorage.getItem(this.tokenName);
  }

  public tokenIsPresent(): boolean {
    if (this.getToken()) return true;    
    return false;
  }

  public getUsername(): string {
    var token = this.getToken();
    if (token) {
      let lala: JwtDecodeOptions;      
      var decoded = jwt_decode<JwtClaims>(token);
      return decoded.username;
    }

    return "";
  }
}
