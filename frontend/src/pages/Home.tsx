import { useState } from 'react';
import over9000Img from '../assets/robot-pic.jpg';

function Home() {
    const [inputValue, setInputValue] = useState<string>("");
    const [errors, setErrors] = useState<string[]>([]);

    const [responseData, setResponseData] = useState<NumberWordsResponse | null>(null);
    const [requestError, setRequestError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const LONG_MIN = BigInt("-9223372036854775807");
    const LONG_MAX = BigInt("9223372036854775807");
    const MAX_LIST_LENGTH = 1000;

    type NumberWordItem = {
        originalNumber: number;
        word: string;
        isOver9000: boolean;
    };

    type NumberWordsResponse = {
        numberWordItems: NumberWordItem[];
    };

    type ParseResult =
    | { ok: true; values: string[] }
    | { ok: false; errors: string[] };

    const VALID_INPUT_HINT = "Please enter whole numbers separated by commas (e.g. 42, 187, -5, 9001).";

    const parseAndValidateLongList = (raw: string): ParseResult => {
        const errors: string[] = [];
        const trimmed = raw.trim();

        if (trimmed.length === 0) {
            return { ok: false, errors: [`Please enter at least one number. ${VALID_INPUT_HINT}`] }
        }

        const parts = trimmed.split(",");

        if (parts.length > MAX_LIST_LENGTH) {
            errors.push(`Too many values — the maximum is ${MAX_LIST_LENGTH} numbers per request.`);
        }

        const normalized: string[] = [];

        parts.forEach((part, index) => {
            const token = part.trim();
            const itemNumber = index + 1;
            
            if (token.length === 0) {
                errors.push(`It looks like there's an extra comma near position #${itemNumber}. ${VALID_INPUT_HINT}`);
                return;
            }

            if (!/^-?\d+$/.test(token)) {
                errors.push(`"${token}" (position #${itemNumber}) isn't a valid whole number. ${VALID_INPUT_HINT}`);
                return;
            }

            let asBigInt: bigint;
            try {
                asBigInt = BigInt(token);
            } catch {
                errors.push(`"${token}" (position #${itemNumber}) isn't a valid whole number. ${VALID_INPUT_HINT}`);
                return;
            }

            if (asBigInt < LONG_MIN || asBigInt > LONG_MAX) {
                errors.push(`"${token}" (position #${itemNumber}) is too large or too small. Numbers must be between −9,223,372,036,854,775,808 and 9,223,372,036,854,775,807. ${VALID_INPUT_HINT}`);
                return;
            }

            normalized.push(asBigInt.toString());
        })

        return errors.length > 0 ? { ok: false, errors} : { ok: true, values: normalized };
    }

    const handleNumberWordsInputValueChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const next = event.target.value;
        setInputValue(next);

        if (errors.length > 0) {
            setErrors([]);
        }
    };

    const handleSubmit = async () => {
        const result = parseAndValidateLongList(inputValue);
        if (result.ok === false) {
            setErrors(result.errors);
            return;
        }

        setRequestError(null);
        setIsLoading(true);

        try{
            const body = `{"numbers":[${result.values.join(",")}]}`;

            const response = await fetch("http://localhost:5273/api/numberwords", {
              method: "POST",
              headers: { "Content-Type": "application/json" },
              body,
            });

            if (response.ok === false) {
                setResponseData(null);
                setRequestError("Something went wrong while converting your numbers. Please check your input and try again.");
                return;
            }

            const data: NumberWordsResponse = await response.json();
            setResponseData(data);
        } catch {
            setResponseData(null);
            setRequestError("Unable to reach the server. Please check your connection and try again.");
        } finally {
            setIsLoading(false);
        }
    }

    return (
      <div className="page-wrapper">
        <header className="page-header">
            <h1>Number Words</h1>
            <p>Enter a comma-separated list of integers to convert them to English words, <strong>sorted alphabetically</strong>.</p>
        </header>

        <form
            className="convert-form"
            onSubmit={(event) => {
                event.preventDefault();
                void handleSubmit();
            }}
        >
            <label htmlFor="number-words-input">Enter your numbers</label>
            <div className="input-row">
                <input 
                    id="number-words-input"
                    type="text"
                    placeholder="e.g. 42, 187, -5, 9001"
                    value={inputValue}
                    onChange={handleNumberWordsInputValueChange}
                />
                <button type="submit">Convert</button>
            </div>
        </form>

        {(errors.length > 0 || requestError) && (
            <div className="error-list">
                {errors.map((error) => (
                    <p key={error}>{error}</p>
                ))}
                {requestError && <p>{requestError}</p>}
            </div>
        )}

        {isLoading && <p className="loading-indicator">Converting…</p>}

        {responseData && (
            <div className="results-card">
                <h2>Results</h2>
                {responseData.numberWordItems.length === 0 ? (
                    <p className="no-results">No results.</p>
                ) : (
                    <ul className="results-list">
                        {responseData.numberWordItems.map((item, index) => (
                            <li key={`${index}-${item.originalNumber}-${item.word}`}>
                                {item.isOver9000 ? (
                                    <span className="over9000-item">
                                        <img
                                            src={over9000Img}
                                            alt="Over 9000!"
                                            className="over9000-img"
                                        />
                                        <span className="over9000-tooltip">{item.word}</span>
                                    </span>
                                ) : (
                                    <span>{item.word}</span>
                                )}
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        )}
      </div>
    )
  }
  
  export default Home;
