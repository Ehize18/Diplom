import { AfterViewInit, Component, output } from '@angular/core';

@Component({
  selector: 'app-shop-card-preview',
  imports: [],
  templateUrl: './shop-card-preview.html',
  styleUrl: './shop-card-preview.css',
})
export class ShopCardPreview implements AfterViewInit {
  viewInit = output<boolean>();

  ngAfterViewInit(): void {
    this.viewInit.emit(true);
  }
}
