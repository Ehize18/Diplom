import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchResult } from '../models/good';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SearchService {
  constructor(private httpClient: HttpClient) {

  }

  search(shopId: string, query: string): Observable<SearchResult[]> {
    return this.httpClient.get<SearchResult[]>(environment.API_URL + '/search', {
      headers: {
        'X-Shop-Id': shopId
      },
      params: {
        'query': query
      }
    });
  }
}
