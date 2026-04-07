import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Category, GetDataResponse, Good, Property } from '../contracts/catalog';
import { Order } from '../contracts/order';

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

  public createCategory(shopId: string, category: Category, image?: File): Observable<any> {
    const formData = new FormData();
    formData.append("Title", category.title);
    formData.append("Description", category.description || '');
    if (category.isActive) {
      formData.append("IsActive", 'true');
    }
    else {
      formData.append("IsActive", 'false');
    }
    if (category.parentCategoryId) {
      formData.append("ParentCategoryId", category.parentCategoryId);
    }
    if (image) {
      formData.append("image", image, image.name);
    }
    return this.httpClient.post<any>(`${this._baseUrl}/${shopId}/category`, formData, this.HTTP_OPTIONS);
  }

  public createGood(shopId: string, good: Good, image?: File): Observable<any> {
    const formData = new FormData();
    formData.append("Title", good.title);
    formData.append("Description", good.description || '');
    formData.append("CategoryId", good.categoryId);
    formData.append("Count", good.count.toString());
    formData.append("Price", good.price.toString());
    
    if (image) {
      formData.append("image", image, image.name);
    }
    return this.httpClient.post<any>(`${this._baseUrl}/${shopId}/good`, formData, this.HTTP_OPTIONS);
  }

  public getData<T>(shopId: string, dataType: string, params?: Record<string, string | number | boolean | readonly (string | number | boolean)[]>) {
    return this.httpClient.get<GetDataResponse<T>>(`${this._baseUrl}/${shopId}/${dataType}`, {
      withCredentials: true,
      params: params
    });
  }

  public getOrders(shopId: string): Observable<GetDataResponse<Order>> {
    const params = {
      'orderBy': 'UpdatedAt',
      'isAscending': false
    }
    return this.getData<Order>(shopId, 'Order', params);
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

  public getProperties(shopId: string) {
    const params = {
      'orderBy': 'Title',
      'isAscending': false
    };
    return this.getData<Property>(shopId, 'Property', params);
  }

  public createProperty(shopId: string, title: string) {
    const body = {
      'title': title
    };
    return this.httpClient.post<string>(`${this._baseUrl}/${shopId}/property`, body, this.HTTP_OPTIONS);
  }

  public createPropertyValue(shopId: string, propertyId: string, title: string) {
    const body = {
      'title': title
    };
    return this.httpClient.post<string>(`${this._baseUrl}/${shopId}/property/${propertyId}`, body, this.HTTP_OPTIONS);
  }

  public updateCategory(shopId: string, category: Category, image?: File): Observable<string> {
    const formData = new FormData();
    formData.append("Title", category.title);
    formData.append("Description", category.description || '');
    if (category.isActive) {
      formData.append("IsActive", 'true');
    }
    else {
      formData.append("IsActive", 'false');
    }
    if (category.parentCategoryId) {
      formData.append("ParentCategoryId", category.parentCategoryId);
    }
    if (category.imageId) {
      formData.append("ImageId", category.imageId);
    }
    if (image) {
      formData.append("image", image, image.name);
    }
    
    return this.httpClient.put<string>(`${this._baseUrl}/${shopId}/category/${category.id}`, formData, this.HTTP_OPTIONS);
  }
}
