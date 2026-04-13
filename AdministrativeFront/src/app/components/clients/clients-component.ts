import { Component, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Button } from '../controls/button/button';
import { ShopService } from '../../services/shop-service';
import { CatalogService } from '../../services/catalog-service';
import { Shop } from '../../contracts/shop';
import { ShopUser } from '../../contracts/user';
import { GetDataResponse } from '../../contracts/catalog';

interface ClientVM {
  id: string;
  username: string;
  vkId: number;
  status: string;
  ordersCount: number;
  totalSum: number;
  dialogCount: number;
  lastActivityDate: Date | null;
}

@Component({
  selector: 'app-clients-component',
  imports: [Button, DatePipe],
  templateUrl: './clients-component.html',
  styleUrl: './clients-component.css',
})
export class ClientsComponent {
  clients = signal<ClientVM[]>([]);

  constructor(
    private shopService: ShopService,
    private catalogService: CatalogService
  ) {
    this.shopService.getCurrentShopObservable().subscribe(
      (shop: Shop | undefined) => {
        if (shop) {
          this.catalogService.getClients(shop.id).subscribe(
            (response: GetDataResponse<ShopUser>) => {
              if (response.isSuccess) {
                const arr: ClientVM[] = response.results.map((client: ShopUser) => ({
                  id: client.id,
                  username: client.username,
                  vkId: client.vkId,
                  status: 'Клиент',
                  ordersCount: client.ordersCount,
                  totalSum: client.totalSum,
                  dialogCount: 0,
                  lastActivityDate: client.lastOrderDate ? new Date(client.lastOrderDate) : null
                }));
                this.clients.set(arr);
              }
            }
          );
        }
      }
    );
  }

  openVkProfile(vkId: number): void {
    window.open(`https://vk.com/id${vkId}`, '_blank');
  }
}
