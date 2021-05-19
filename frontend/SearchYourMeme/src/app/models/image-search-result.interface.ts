import { TextSearchResult } from "./text-search-result.interface";

export interface ImageSearchResult extends TextSearchResult {
    tags: string;
}