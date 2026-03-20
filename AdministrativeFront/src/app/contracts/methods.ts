export interface Method {
    id: string;
    title: string;
    options: object;
}

export interface PaymentMethod extends Method {

}

export interface DeliveryMethod extends Method {

}