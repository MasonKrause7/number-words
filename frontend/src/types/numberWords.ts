export type NumberWordItem = {
    originalNumber: number;
    word: string;
    isOver9000: boolean;
};

export type NumberWordsResponse = {
    numberWordItems: NumberWordItem[];
};

export type ParseResult =
    | { ok: true; values: string[] }
    | { ok: false; errors: string[] };
