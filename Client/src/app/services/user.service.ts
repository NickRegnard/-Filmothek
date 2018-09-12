import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { User } from '../Models/user';
import { AppSettings } from '../appSettings';


const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private http: HttpClient,
  ) { }
  

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(AppSettings.apiUrl+"user")
    //.pipe(catchError(this.handleError('', {}) //implement an error handler
  }

  register(user: User): Observable<User> {
    return this.http.post<User>(`${AppSettings.apiUrl}register/`, user, httpOptions);
  }

  // /** POST: add a new hero to the server */
  // addHero (hero: Hero): Observable<Hero> {
  //   return this.http.post<Hero>((this.heroesUrl), hero, httpOptions).pipe(
  //     tap((hero: Hero) => this.log(`added hero w/ id=${hero.heroId}`)),
  //     catchError(this.handleError<Hero>('addHero'))
  //   );
  // }
  
  /* error handerl
  private handleError(???)
  */
}