# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Frontend (`cd frontend`)
```bash
npm install          # Install dependencies
npm run dev          # Dev server on localhost:3000 (uses Turbopack)
npm run build        # Production build
npm run start        # Serve production build
npm run lint         # Run ESLint
```

### Backend (`cd backend`)
```bash
dotnet restore       # Restore NuGet packages
dotnet build         # Build project
dotnet run           # Run on localhost:5089
dotnet watch run     # Run with hot reload
```

There are no test suites configured for either frontend or backend.

## Architecture

Yaatra is a ride-hailing simulation platform. The frontend is **Next.js 16 (App Router) + TypeScript + Tailwind CSS v4**, the backend is **ASP.NET Core 10 (.NET 10, C#)** with in-memory storage only — all data resets on restart.

### Frontend (`frontend/`)

- **`app/`** — Next.js App Router entry (single layout + page)
- **`components/`** — Domain-organized React components:
  - `yaatra/` — Root `YaatraApp` component; orchestrates view switching
  - `rider/` — Multi-step rider flow (Home → Confirm → Searching → DriverArriving → InTrip → Payment → Rating)
  - `driver/` — Driver flow (Home → Pickup → InTrip)
  - `command/` — `AdminCommandCenter` with live metrics, anomalies, ML predictions
  - `map/` — `FleetMap` and `MapPanel` built on MapLibre GL v5
  - `layout/` — `AppShell` wrapper
  - `common/` — `StatusChip`, `ToastHost`
  - `ui/` — `GlassCard`, `KpiWidget`
- **`contexts/`** — `RideStateContext` (trip state machine), `ToastContext`
- **`hooks/`** — `useTripPolling` (polls backend for status changes), `useMapProps`
- **`lib/api.ts`** — `YaatraAPI` class; base URL defaults to `http://localhost:5089/api` (override with `NEXT_PUBLIC_API_URL`)
- **`lib/types.ts`** — All shared TypeScript interfaces
- **`lib/geo.ts`** — Geolocation helpers (NCR region)

State is managed via React Context; there is no Redux or Zustand. The frontend polls the backend for trip updates — there is no WebSocket connection.

### Backend (`backend/`)

Clean architecture: Controllers → Services → Repositories → Models.

- **`Controllers/`** — `TripsController` (`/api/trips`), `DriversController` (`/api/drivers`), `MetricsController` (`/api/metrics`, `/api/admin`)
- **`Services/`** — `TripService` (trip lifecycle), `PricingService` (fare calculation), `MetricsService` (live stats, anomaly detection, ML predictions)
- **`Repositories/`** — `InMemoryRepositories.cs` backed by `ConcurrentDictionary`; seeded from `Data/SeedData.cs`
- **`Models/`** — `Trip`, `Driver`, `TripStatus` enum (Created → Searching → Assigned → Arriving → InProgress → Completed/Cancelled)
- **`Simulations/FleetSimulation.cs`** — `BackgroundService` that ticks every 2 seconds, advancing trip progress and updating driver positions
- **`Helpers/NcrGeocoding.cs`** — Maps location name strings to lat/lng (NCR/Delhi/Gurgaon/Noida region only)

**Pricing** (base fare + per-km rate, surge applied when traffic multiplier >1.2):
| Vehicle | Base | Per km |
|---------|------|--------|
| Bike    | ₹25  | ₹8     |
| Auto    | ₹35  | ₹12    |
| Sedan   | ₹50  | ₹18    |
| SUV     | ₹80  | ₹28    |

CORS is open to all origins. OpenAPI/Swagger is enabled in development.

### Frontend–Backend contract

The `YaatraAPI` client in `lib/api.ts` calls `/api/trips/*` and `/api/drivers/*`. If you add or rename a backend endpoint, update `lib/api.ts` to match — the method names in the client class do not always match the HTTP route names 1:1.
