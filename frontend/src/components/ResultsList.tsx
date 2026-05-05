import over9000Img from '../assets/robot-pic.jpg';
import type { NumberWordsResponse } from '../types/numberWords';

interface ResultsListProps {
    data: NumberWordsResponse;
}

function ResultsList({ data }: ResultsListProps) {
    const hasOver9000 = data.numberWordItems.some(item => item.isOver9000);

    return (
        <div className="results-card">
            <h2>
                Results
                {hasOver9000 && (
                    <span className="over9000-hint">
                        {' '}(values greater than |9000| can be revealed by hovering the NumberWord bot)
                    </span>
                )}
            </h2>
            {data.numberWordItems.length === 0 ? (
                <p className="no-results">No results.</p>
            ) : (
                <ul className="results-list">
                    {data.numberWordItems.map((item, index) => (
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
    );
}

export default ResultsList;
