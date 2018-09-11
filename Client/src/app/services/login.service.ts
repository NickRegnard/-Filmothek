<<<<<<< HEAD
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { User } from '../Models/user';
import { Observable } from 'rxjs';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private http: HttpClient) { }

  //TODO: change to actual path
  private apiUrl = "http://localhost:50000/api";


  //remember to kill fakeDB
  //TODO: change to actual path  
  login(username: string, password: string) {
     return this.http.post<any>(`${this.apiUrl}/Login`, { username: username, password: password }, httpOptions)
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
=======
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
>>>>>>> 999c4bcb684bc37ad08f0802959b7a8a3a6bf9f1
 }