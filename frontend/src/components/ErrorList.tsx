interface ErrorListProps {
    errors: string[];
    requestError: string | null;
}

function ErrorList({ errors, requestError }: ErrorListProps) {
    if (errors.length === 0 && !requestError) {
        return null;
    }

    return (
        <div className="error-list">
            {errors.map((error) => (
                <p key={error}>{error}</p>
            ))}
            {requestError && <p>{requestError}</p>}
        </div>
    );
}

export default ErrorList;
