import { Basket } from "./basket";

export interface Method {
    id: string;
    title: string;
    metadata: Record<string, string>;
}

export interface Order {
    id: string;
    paymentMethodId: string;
    deliveryMethodId: string;
    deliveryExtras: string;
    deliveryStatus: DeliveryStatus;
    paymentStatus: PaymentStatus;
    orderStatus: OrderStatus;
    fullPrice: number;
    basketId: string;
    createdAt: Date;
    basket: Basket
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

export enum OrderStatus {
    Created,
    Payment,
    Delivery,
    Completed,
    Canceled
}