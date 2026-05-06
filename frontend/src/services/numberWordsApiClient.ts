import type { NumberWordsResponse } from '../types/numberWords';

const BASE_API_URL = "http://localhost:5273/";
const NUMBER_WORDS_URL = "api/numberwords";

export async function convertNumbers(values: string[]): Promise<NumberWordsResponse> {
    const API_URL = `${BASE_API_URL}${NUMBER_WORDS_URL}`;
    // Build JSON manually to avoid JavaScript Number precision loss for Int64 values.
    const body = `{"numbers":[${values.join(",")}]}`;

    const response = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body,
    });

    if (!response.ok) {
        throw new Error("Something went wrong while converting your numbers. Please check your input and try again.");
    }

    return response.json() as Promise<NumberWordsResponse>;
}
