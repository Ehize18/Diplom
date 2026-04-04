export interface Good {
    id: string;
    categoryId: string;
    count: number;
    price: number;
    oldPrice: number;
    imageId: string | null;
    properties: any[];
    title: string;
    description: string;
}

export interface GoodCategory {
    id: string;
    parentCategoryId: string;
    title: string;
    description: string;
    imageId: string | null;
    childs: GoodCategory[];
}

export interface SearchResult {
    id: string;
    title: string;
    sourceType: number;
}