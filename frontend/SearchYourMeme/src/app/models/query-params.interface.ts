export interface QueryParams {
    status: string[];
    category: string[];
    details: string[];
    yearFrom: number;
    yearTo: number;
    fields: string[];
    sort?: string | null;
    sortAsc: boolean;
}

export interface ImageQueryParams extends QueryParams {
    url: string;
    searchSimilarities: boolean;
}