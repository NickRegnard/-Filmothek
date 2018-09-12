import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

import { AppSettings } from '../appSettings';
import { Movie } from '../Models/movie';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

  constructor(
    private http: HttpClient
  ) { }

  getAllMovies(): Observable<Movie[]> {
    return this.http.get<Movie[]>(AppSettings.apiUrl+'movies')
  } 

  getMovieById(id:number): Observable<Movie> {
    return this.http.get<Movie>(AppSettings.apiUrl+`movie${id}`)
  }

  addMovie(movie: Movie): Observable<Movie> {
    return this.http.post<Movie>(AppSettings.apiUrl+`createMovie`, movie, AppSettings.httpOptions);
  }

  editMovie(movie: Movie, id:number): Observable<Movie> {
    return this.http.put<Movie>(AppSettings.apiUrl+`updateMovie${id}`, movie, AppSettings.httpOptions);
  }

  BorrowMovie(movie: Movie): Observable<Movie> {
    return this.http.post<Movie>(AppSettings.apiUrl+`Borrow`, movie.id, AppSettings.httpOptions);
  }
}
