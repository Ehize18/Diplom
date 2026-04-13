import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, of, map } from 'rxjs';
import { GetDataResponse } from '../contracts/catalog';
import { ShopStatistics } from '../contracts/statistics';
import { CatalogService } from './catalog-service';

@Injectable({
  providedIn: 'root',
})
export class StatisticsService {
  private _statistics = new BehaviorSubject<ShopStatistics | null>(null);
  private _pendingRequest: Observable<ShopStatistics> | null = null;

  constructor(private catalogService: CatalogService) {
  }

  getStatistics(shopId: string): Observable<ShopStatistics> {
    const cached = this._statistics.value;
    if (cached) {
      return of(cached);
    }

    if (this._pendingRequest) {
      return this._pendingRequest;
    }

    const request = this.catalogService.getStatistics(shopId).pipe(
      map((response: GetDataResponse<ShopStatistics>) => {
        const stats = response.results[0] as unknown as ShopStatistics;
        this._statistics.next(stats);
        this._pendingRequest = null;
        return stats;
      })
    );

    this._pendingRequest = request;

    return this._pendingRequest;
  }

  invalidate(): void {
    this._statistics.next(null);
    this._pendingRequest = null;
  }
}
