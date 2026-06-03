# Architecture Overview

This monorepo is structured for a modular DMV Automation SaaS MVP:

- `backend/` - .NET 8 modular monolith Web API containing the API project and test projects.
- `frontend/` - Next.js + React + TypeScript application for the public landing page.
- `docs/` - Architecture notes and local setup instructions.
- `infra/` - Placeholder for future Azure deployment scripts and infrastructure definitions.

Backend structure:

- `backend/CarDealership.sln` - solution file for the API and test projects.
- `backend/src/CarDealership.Api/` - ASP.NET Core Web API project.
- `backend/tests/CarDealership.UnitTests/` - unit tests for API models and logic.
- `backend/tests/CarDealership.IntegrationTests/` - integration tests for API endpoints.

Frontend structure:

- `frontend/pages/index.tsx` - landing page that checks the backend health endpoint.
- `frontend/__tests__/Home.test.tsx` - smoke test for page rendering.
