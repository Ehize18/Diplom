import { Location } from '@angular/common';
import { Component, computed, effect, OnDestroy, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SearchService } from '../../services/search-service';
import { SearchResult } from '../../models/good';
import { ShopService } from '../../services/shop-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [FormsModule],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header implements OnDestroy{
  searchData = signal<string>('');
  lastChangeTime = signal<number | undefined>(undefined);
  isSearch = signal<boolean>(false);
  searchedData = signal<string>('');
  searchResults = signal<SearchResult[]>([]);

  private searchTimer: any = null;

  constructor(
    private location: Location,
    private router: Router,
    private shopService: ShopService,
    private searchService: SearchService
  ) {
    effect(() => {
      if (this.isSearch()) {
        console.log('Поиск по запросу:', this.searchData());
        this.search(this.searchData());
      }
    });
  }

  onBackClick(): void {
    this.location.back();
  }

  search(query: string): void {
    this.searchService.search(
      this.shopService.shopId(),
      query
    ).subscribe(
      results => {
        this.searchResults.set(results);
        console.log(results);
      }
    )
  }

  onSearchResultClick(searchResult: SearchResult): void {
    const shopId = this.shopService.shopId();
    if (searchResult.sourceType === 1) {
      this.router.navigate([shopId, 'good', searchResult.id]);
    }
    else {
      this.router.navigate([shopId, 'category', searchResult.id]);
    }
    this.searchData.set('');
    this.onSearchDataChange();
  } 

  onSearchDataChange(): void {
    const value = this.searchData();
    
    if (this.searchTimer) {
      clearTimeout(this.searchTimer);
    }

    if (value.length === 0) {
      this.isSearch.set(false);
      return;
    }

    this.searchTimer = setTimeout(() => {
      this.isSearch.set(true);
      this.searchedData.set(this.searchData());
    }, 500);
  }

  ngOnDestroy(): void {
    if (this.searchTimer) {
      clearTimeout(this.searchTimer);
    }
  }
}
