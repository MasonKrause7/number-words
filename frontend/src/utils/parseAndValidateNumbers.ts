import type { ParseResult } from '../types/numberWords';

const LONG_MIN = BigInt("-9223372036854775808");
const LONG_MAX = BigInt("9223372036854775807");
const MAX_LIST_LENGTH = 1000;

const VALID_INPUT_HINT = "Please enter whole numbers separated by commas (e.g. 42, 187, -5, 9001).";

export function parseAndValidateNumbers(raw: string): ParseResult {
    const errors: string[] = [];
    const trimmed = raw.trim();

    // VALIDATION STEPS
    // 1. Non-empty string
    if (trimmed.length === 0) {
        return { ok: false, errors: [`Please enter at least one number. ${VALID_INPUT_HINT}`] };
    }

    const parts = trimmed.split(",");

    // 2. Less than or equal to the max list length
    if (parts.length > MAX_LIST_LENGTH) {
        errors.push(`Too many values — the maximum is ${MAX_LIST_LENGTH} numbers per request.`);
    }

    const normalized: string[] = [];

    parts.forEach((part, index) => {
        const token = part.trim();
        const itemNumber = index + 1;

        //Token Validation - provide descriptive error message

        // Non-empty token
        if (token.length === 0) {
            errors.push(`It looks like there's an extra comma near position #${itemNumber}. ${VALID_INPUT_HINT}`);
            return;
        }

        // Valid token (positive or negative whole numbers only)
        if (!/^-?\d+$/.test(token)) {
            errors.push(`"${token}" (position #${itemNumber}) isn't a valid whole number. ${VALID_INPUT_HINT}`);
            return;
        }

        // Casts to Int64
        let asBigInt: bigint;
        try {
            asBigInt = BigInt(token);
        } catch {
            errors.push(`"${token}" (position #${itemNumber}) isn't a valid whole number. ${VALID_INPUT_HINT}`);
            return;
        }

        // Within Long bounds
        if (asBigInt < LONG_MIN || asBigInt > LONG_MAX) {
            errors.push(`"${token}" (position #${itemNumber}) is too large or too small. Numbers must be between ${LONG_MIN} and ${LONG_MAX}. ${VALID_INPUT_HINT}`);
            return;
        }

        normalized.push(asBigInt.toString());
    });

    return errors.length > 0 ? { ok: false, errors } : { ok: true, values: normalized };
}
