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