import { Meme } from "./meme";

export interface ImageSearchResult {
    memes: Meme[];
    tags: string;
}