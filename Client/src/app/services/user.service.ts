import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable } from 'rxjs'

import { User } from '../Models/user';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private http: HttpClient,
  ) { }
  
  //TODO: change to actual path
  private apiUrl = "http://localhost:60000/api/";

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(this.apiUrl+"user")
    //.pipe(catchError(this.handleError('', {}) //implement an error handler
  }

  register(user: User) {
    return this.http.post(`${this.apiUrl}users/`+user.id, user);
  }
  
  /* error handerl
  private handleError(???)
  */
}