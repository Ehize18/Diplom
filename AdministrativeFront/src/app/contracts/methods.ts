export interface Method {
    id: string;
    title: string;
    metadata: Record<string, string>;
}

export interface PaymentMethod extends Method {

}

export interface DeliveryMethod extends Method {

}