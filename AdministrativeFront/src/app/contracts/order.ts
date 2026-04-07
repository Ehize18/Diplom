import { Good } from "./catalog";
import { DeliveryMethod, PaymentMethod } from "./methods";
import { ShopUser } from "./user";

export interface Order {
    id: string;
    basket: Basket;
    basketId: string;
    deliveryMethod: DeliveryMethod;
    paymentMethod: PaymentMethod;
    paymentStatus: PaymentStatus;
    deliveryStatus: DeliveryStatus;
    orderStatus: OrderStatus;
    fullPrice: number;
    createdAt: Date;
    updatedAt: Date;
    deliveryExtras: string;
}

export interface Basket {
    id: string;
    user: ShopUser;
    goods: BasketItem[]
}

export interface BasketItem {
    id: string;
    price: number;
    count: number;
    goodId: string;
    good: Good
}

export enum DeliveryStatus {
    Created,
    InDelivery,
    Delivered,
    Fail
}

export enum PaymentStatus {
    Created,
    Payed,
    Fail
}

export enum OrderStatus{
    Created,
    Payment,
    Delivery,
    Completed,
    Canceled
}