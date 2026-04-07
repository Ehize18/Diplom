import { Component, OnInit, signal } from '@angular/core';
import { Order } from '../../models/order';
import { OrderService } from '../../services/order-service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-orders-component',
  imports: [RouterLink],
  templateUrl: './orders-component.html',
  styleUrl: './orders-component.css',
})
export class OrdersComponent implements OnInit {
  orders = signal<Order[]>([]);

  constructor(
    private orderService: OrderService
  ) {
  }

  ngOnInit(): void {
    this.orderService.getOrders().subscribe(orders => {
      this.orders.set(orders)
    });
  }

  getOrderDate(order: Order): string {
    const date = new Date(order.createdAt);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
  }
}
