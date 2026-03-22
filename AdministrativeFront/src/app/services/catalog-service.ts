import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Category, GetDataResponse, Good } from '../contracts/catalog';

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

  public createCategory(shopId: string, category: Category): Observable<any> {
    return this.httpClient.post<any>(`${this._baseUrl}/${shopId}/category`, category, this.HTTP_OPTIONS);
  }

  public createGood(shopId: string, good: Good): Observable<any> {
    return this.httpClient.post<any>(`${this._baseUrl}/${shopId}/good`, good, this.HTTP_OPTIONS);
  }

  public getData<T>(shopId: string, dataType: string, params?: Record<string, string | number | boolean | readonly (string | number | boolean)[]>) {
    return this.httpClient.get<GetDataResponse<T>>(`${this._baseUrl}/${shopId}/${dataType}`, {
      withCredentials: true,
      params: params
    })
  }

  public getCategories(shopId: string): Observable<GetDataResponse<Category>> {
    const params = {
      'orderBy': 'Title',
      'isAscending': true,
      'filterType': 0,
      'column': 'ParentCategoryId',
      'columnValue': ''
    }
    return this.getData<Category>(shopId, 'Category', params);
  }

  public getChildCategories(shopId: string, categoryId: string): Observable<GetDataResponse<Category>> {
    const params = {
      'orderBy': 'Title',
      'isAscending': true,
      'filterType': 0,
      'column': 'ParentCategoryId',
      'columnValue': categoryId
    }
    return this.getData<Category>(shopId, 'Category', params);
  }

  public getGoods(shopId: string, orderBy: string, isAscending: boolean, categoryId?: string) {
    let params: Record<string, string | number | boolean | readonly (string | number | boolean)[]> = {
      'orderBy': orderBy,
      'isAscending': isAscending,
    };
    if (categoryId) {
      params['filterType'] = 0;
      params['column'] = 'CategoryId';
      params['columnValue'] = categoryId;
    }
    return this.getData<Good>(shopId, 'Good', params);
  }

  public updateCategory(shopId: string, category: Category): Observable<string> {
    return this.httpClient.put<string>(`${this._baseUrl}/${shopId}/category/${category.id}`, category, this.HTTP_OPTIONS);
  }
}
