import { useState } from 'react';
import type { NumberWordsResponse } from '../types/numberWords';
import { parseAndValidateNumbers } from '../utils/parseAndValidateNumbers';
import { convertNumbers } from '../services/numberWordsApi';
import NumberForm from '../components/NumberForm';
import ErrorList from '../components/ErrorList';
import ResultsList from '../components/ResultsList';

function Home() {
    const [inputValue, setInputValue] = useState("");
    const [errors, setErrors] = useState<string[]>([]);
    const [responseData, setResponseData] = useState<NumberWordsResponse | null>(null);
    const [requestError, setRequestError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleInputChange = (value: string) => {
        setInputValue(value);
        if (errors.length > 0) {
            setErrors([]);
        }
    };

    const handleSubmit = async () => {
        const result = parseAndValidateNumbers(inputValue);
        if (!result.ok) {
            setErrors(result.errors);
            return;
        }

        setRequestError(null);
        setIsLoading(true);

        try {
            const data = await convertNumbers(result.values);
            setResponseData(data);
        } catch (error) {
            setResponseData(null);
            setRequestError(
                error instanceof Error
                    ? error.message
                    : "Unable to reach the server. Please check your connection and try again."
            );
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="page-wrapper">
            <header className="page-header">
                <h1>Number Words</h1>
                <p>Enter a comma-separated list of integers to convert them to English words, <strong>sorted alphabetically</strong>.</p>
            </header>

            <NumberForm
                value={inputValue}
                onChange={handleInputChange}
                onSubmit={() => void handleSubmit()}
            />

            <ErrorList errors={errors} requestError={requestError} />

            {isLoading && <p className="loading-indicator">Converting…</p>}

            {responseData && <ResultsList data={responseData} />}
        </div>
    );
}

export default Home;
