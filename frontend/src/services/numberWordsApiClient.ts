import type { NumberWordsRequest, NumberWordsResponse } from '../types/numberWords';

const API_URL = "http://localhost:5273/api/numberwords";

export async function convertNumbers(values: string[]): Promise<NumberWordsResponse> {
    const request: NumberWordsRequest = { numbers: values.map(Number) };

    const response = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
    });

    if (!response.ok) {
        throw new Error("Something went wrong while converting your numbers. Please check your input and try again.");
    }

    return response.json() as Promise<NumberWordsResponse>;
}
