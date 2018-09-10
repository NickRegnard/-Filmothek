import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { User } from '../Models/user';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private http: HttpClient) { }

  //TODO: change to actual path
  private apiUrl = "";

  
  //TODO: change to actual path  
  login(username: string, password: string) {
     return this.http.post<any>(`${this.apiUrl}/users/authenticate`, { username: username, password: password })
       .pipe(map(user => {
        	if(user && user.token) {
            localStorage.setItem("currentUser", JSON.stringify(user));
          }
          return user;
      }));   
    
    }

    logout() {
      localStorage.removeItem("currentUser");
    }
 }