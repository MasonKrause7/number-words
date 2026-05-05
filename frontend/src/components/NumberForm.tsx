interface NumberFormProps {
    value: string;
    onChange: (value: string) => void;
    onSubmit: () => void;
}

function NumberForm({ value, onChange, onSubmit }: NumberFormProps) {
    return (
        <form
            className="number-form"
            onSubmit={(event) => {
                event.preventDefault();
                onSubmit();
            }}
        >
            <label htmlFor="number-words-input">Enter your numbers</label>
            <div className="input-row">
                <input
                    id="number-words-input"
                    type="text"
                    placeholder="e.g. 42, 187, -5, 9001"
                    value={value}
                    onChange={(e) => onChange(e.target.value)}
                />
                <button type="submit">Convert</button>
            </div>
        </form>
    );
}

export default NumberForm;
