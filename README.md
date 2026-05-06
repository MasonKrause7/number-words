# Number Words

A web application that converts numeric values into their English word equivalents. Users enter a comma-separated list of integers and the app returns the word representation of each number, sorted alphabetically.

## Key Features

- Accepts integers in the Int64 range (up to 1,000 values per request)
- Converts numbers to English words (e.g., `187` → "One Hundred Eighty Seven")
- Returns results sorted alphabetically by word
- Flags numbers over 9,000 with a special indicator
- Handles negative numbers with a "Negative" prefix

## Tech Stack

| Layer    | Technology                              |
|----------|-----------------------------------------|
| Backend  | C# / .NET 10, ASP.NET Core Web API     |
| Frontend | TypeScript, React, Vite                |
| Testing  | xUnit                                  |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) with npm

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd <repository-directory>
```

### 2. Run the Backend API

```bash
cd backend/NumberWords.Api
dotnet run
```

The API will start on **http://localhost:5273**.

### 3. Run the Frontend

In a separate terminal:

```bash
cd frontend
npm install
npm run dev
```

The frontend dev server will start on **http://localhost:5173**. The app is configured to call the backend API at `http://localhost:5273`, so make sure the backend is running first.

### 4. Open the App

Navigate to [http://localhost:5173](http://localhost:5173) in your browser.

## Running Tests

Backend tests use xUnit and are located in `backend/NumberWords.Api.Tests/`.

```bash
cd backend
dotnet test
```

## API Reference

### `POST /api/numberwords`

Converts an array of integers to their English word equivalents.

**Request body:**

```json
{
  "numbers": [42, 187, -5, 9001]
}
```

**Response:**

```json
{
  "numberWordItems": [
    { "number": 42, "word": "Forty Two", "isOverNineThousand": false },
    { "number": -5, "word": "Negative Five", "isOverNineThousand": false },
    { "number": 9001, "word": "Nine Thousand One", "isOverNineThousand": true },
    { "number": 187, "word": "One Hundred Eighty Seven", "isOverNineThousand": false }
  ]
}
```

Results are sorted alphabetically by the `word` field.

## Project Structure

```
.
├── backend/
│   ├── NumberWords.slnx                 # .NET solution file
│   ├── NumberWords.Api/                 # ASP.NET Core Web API
│   │   ├── Controllers/                 # API endpoints
│   │   ├── Models/
│   │   │   ├── DomainModel/             # Business logic & domain types
│   │   │   ├── RequestDtos/             # Inbound request shapes
│   │   │   └── ResponseDtos/            # Outbound response shapes
│   │   └── Program.cs                   # App entry point & DI setup
│   └── NumberWords.Api.Tests/           # xUnit test project
│
├── frontend/
│   ├── src/
│   │   ├── pages/                       # Page-level components
│   │   ├── components/                  # Reusable UI components
│   │   ├── services/                    # API client functions
│   │   ├── types/                       # TypeScript type definitions
│   │   └── utils/                       # Validation & utility functions
│   ├── package.json
│   └── vite.config.ts
│
└── README.md
```

## Architecture Notes

- **Backend** follows a Controller → Service pattern with dependency injection.
- **Frontend** keeps well structured components with pure functionality encapsulated in utilities.
- **Validation** is handled on both sides — the backend uses DataAnnotations on request DTOs; the frontend validates before sending requests.
- **CORS** is configured in the backend to allow requests from `http://localhost:5173` during development.
