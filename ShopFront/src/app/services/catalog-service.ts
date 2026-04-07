import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Good, GoodCategory } from '../models/good';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CatalogService {

  constructor(private httpClient: HttpClient) {

  }

  getGoods(shopId: string, categoryId?: string, isActual?: boolean): Observable<Good[]> {
    let params;
    if (categoryId && isActual !== undefined) {
      params = {
          'categoryId': categoryId,
          'isActual': isActual
        }
    }
    return this.httpClient.get<Good[]>(environment.API_URL + '/good', {
        headers: {
          'X-Shop-Id': shopId
        },
        params: params
      });
  }

  getGoodById(shopId: string, goodId: string): Observable<Good> {
    return this.httpClient.get<Good>(environment.API_URL + `/good/${goodId}`, {
      headers: {
        'X-Shop-Id': shopId
      }
    });
  }

  getCategoriesByParent(shopId: string, parentCategoryId: string | null): Observable<GoodCategory[]> {
    let params;
    if (parentCategoryId) {
      params = {
        'parentCategoryId': parentCategoryId
      }
    }
    return this.httpClient.get<GoodCategory[]>(environment.API_URL + '/category/byParent', {
      headers: {
        'X-Shop-Id': shopId
      },
      params: params
    });
  }

  getCategoryById(shopId: string, categoryId: string, withChilds: boolean = true): Observable<GoodCategory> {
    return this.httpClient.get<GoodCategory>(environment.API_URL + '/category/' + categoryId, {
      headers: {
        'X-Shop-Id': shopId
      },
      params: {
        'withChilds': withChilds
      }
    });
  }
}
