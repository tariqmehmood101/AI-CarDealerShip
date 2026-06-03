# Car Dealership DMV Automation SaaS MVP

This repository is a monorepo scaffold for the Car Dealership DMV Automation MVP.

## Structure

- `backend/` - .NET 8 ASP.NET Core Web API solution and test projects.
- `frontend/` - Next.js + React + TypeScript landing page.
- `docs/` - architecture notes and local setup documentation.
- `infra/` - placeholder for Azure deployment artifacts.

## Prerequisites

- .NET 8 SDK
- Node.js 20+ and npm (frontend development)

## Backend

Build and run from the repository root:

```bash
cd backend
dotnet build
dotnet run --project src/CarDealership.Api/CarDealership.Api.csproj
```

Health endpoint:

- `http://localhost:5000/health`
- `https://localhost:5001/health`

## Backend tests

```bash
dotnet test backend/CarDealership.sln
```

## Frontend

Install dependencies and start the development server:

```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:3000`.

## Frontend tests

```bash
npm test --prefix frontend
```

## Root test command

From the repository root, run:

```bash
npm test
```

This executes the backend solution tests and the frontend Jest tests.

## Documentation

See `docs/architecture.md` and `docs/local-setup.md` for more details.
