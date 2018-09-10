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

  register(user: User) {
    return this.http.post(`${this.apiUrl}/users/`+user.id, user);
  }
  
}