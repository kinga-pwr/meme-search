import { Meme } from "./meme";

export interface TextSearchResult {
    numberOfResults: number;
    memes: Meme[];
}