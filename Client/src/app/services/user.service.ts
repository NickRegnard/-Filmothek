import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { User } from '../Models/user';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private http: HttpClient,
  ) { }

  //TODO: change to actual path
  private apiUrl = "api";

  //checks if user exists. Also gets PW for comparison to login
  getLogindata(username: string): Observable<User> {

    let user: Observable<User> = this.http.get<User>(this.apiUrl+"/"+username);
    return user;  
    
  }

}