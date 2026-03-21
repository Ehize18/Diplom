import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Category, GetDataResponse } from '../contracts/catalog';

@Injectable({
  providedIn: 'root',
})
export class CatalogService {
  private HTTP_OPTIONS = {
    withCredentials: true
  };
  private _baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this._baseUrl = environment.apiUrl + "/shopcontent"
  }

  public getCategories(shopId: string): Observable<GetDataResponse<Category>> {
    return this.httpClient.get<GetDataResponse<Category>>(`${this._baseUrl}/${shopId}/category`, this.HTTP_OPTIONS);
  }
}
