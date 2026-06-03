# Local Setup

## Prerequisites

- .NET 8 SDK
- Node.js 20+ and npm (for frontend development)

## Backend

1. Open a terminal in `backend/`.
2. Restore and build:
   ```bash
   dotnet build
   ```
3. Run the API:
   ```bash
   dotnet run --project src/CarDealership.Api/CarDealership.Api.csproj
   ```
4. The health endpoint is available at `https://localhost:5001/health` or `http://localhost:5000/health`.

## Frontend

1. Open a terminal in `frontend/`.
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start local development:
   ```bash
   npm run dev
   ```
4. Open `http://localhost:3000`.

## Tests

- Backend:
  ```bash
  dotnet test backend/CarDealership.sln
  ```
- Frontend:
  ```bash
  npm test --prefix frontend
  ```
