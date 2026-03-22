export interface GetDataResponse<T> {
    results: T[];
    isSuccess: boolean;
    error: string;
}

export interface Base {
    id: string;
    createdAt: Date;
    updatedAt: Date | null;
    createdById: string;
    updatedById: string | null;
}

export interface Category extends Base {
    title: string;
    description: string | null;
    isActive: boolean;
    parentCategoryId: string | null;
}

export interface Good extends Base {
    title: string;
    description: string | null,
    categoryId: string,
    price: number,
    oldPrice: number
    count: number
}