import { HttpHeaders } from '@angular/common/http';

export class AppSettings {
  public static apiUrl ="http://localhost:60000/api/"
  public static httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };
}

