# Loan Management System

A full-stack loan management ecosystem for handling loan applications, balance tracking, and payment registration. The platform pairs a **.NET 6 Web API** backend with an **Angular 19** frontend built on **Angular Material**, delivering a secure, observable, and production-ready experience for operators managing borrower portfolios.

---

## 🏗️ Architecture & Technical Decisions

### Backend Architecture

The backend follows a **modular, layered design** aligned with **Clean Architecture** and the **Ports & Adapters** pattern, keeping the Web API project thin and focused on HTTP concerns:

| Project | Responsibility |
|---------|----------------|
| `Fundo.Domain` | Core entities (`Loan`), enums (`LoanStatus`), and domain contracts |
| `Fundo.Application` | Application services, business rules, and repository contracts (e.g., `ILoanRepository`, `ILoanService`) |
| `Fundo.Infrastructure` | EF Core `DbContext`, entity configurations, SQL Server concrete repository implementations, migrations, and seed data |
| `Fundo.Applications.WebApi` | REST controllers, request DTOs, JWT services, middleware, and application startup |

Business rules live in **`Fundo.Application`** and depend only on abstractions (`ILoanRepository`), not on EF Core or SQL Server. Concrete persistence is provided by **`Fundo.Infrastructure`** (`LoanRepository`), which implements the application ports. The Web API composes both layers via dependency injection (`AddInfrastructure` + `AddApplication`), ensuring that application logic has **zero coupling** to the underlying database mechanism.

This separation improves testability, maintainability, and future extensibility (e.g., swapping SQL Server for another store or adding CQRS handlers without polluting `Startup`).

### Domain Integrity & Business Rules

All critical validations are enforced **server-side**, ensuring the API remains the single source of truth regardless of client behavior:

- **Payment amount validation** — Payments must be greater than zero and cannot exceed the loan's `currentBalance`.
- **Full-balance coverage** — Overpayment attempts return `400 Bad Request` with a descriptive message.
- **Automated status transition** — When `currentBalance` reaches zero after a valid payment, the loan status is automatically updated from `Active` to `Paid`.
- **Loan creation** — New loans are persisted with `Active` status and `currentBalance` initialized to the requested `amount`.
- **Input validation** — `CreateLoanRequest` and `PaymentRequest` DTOs use Data Annotations to reject malformed payloads before they reach the domain layer.

### Frontend Architecture

The Angular 19 client is a **standalone-component application** with lazy-loaded routes and a service-oriented structure:

- **Standalone Components** — No NgModules; each feature (`loan-list`, `loan-form`, `loan-detail`, `login`, `settings`) is self-contained.
- **Reactive Forms** — Loan registration uses `FormBuilder` with synchronous validators for applicant name, contract ID, tax ID, and amount.
- **HTTP Interceptors** — A JWT `auth.interceptor` automatically attaches the Bearer token to outbound API requests after login.
- **Route Guards** — `authGuard` and `guestGuard` protect authenticated and public routes respectively.
- **Dynamic Theme Switching** — A `ThemeService` toggles `light-theme` / `dark-theme` classes on `document.body`, with global CSS variables and MDC token overrides ensuring **forced contrast compliance** across cards, tables, filters, and the persistent sidebar.

---

## 🚀 Extra & Bonus Features Implemented

### JWT Authentication

All loan endpoints are protected with `[Authorize]` and the **Bearer token** authentication scheme. A dedicated `AuthController` exposes `POST /api/auth/login`, backed by `JwtTokenService` for token generation and validation. The Angular login flow stores the token and propagates it via the HTTP interceptor, with route guards preventing unauthenticated access to the dashboard.

### Structured Logging

**Serilog** is integrated at application startup (`Program.cs`) with configuration driven from `appsettings.json`. Logs include named properties (e.g., payment amount and loan ID) and are emitted to the console with structured templates, providing clear observability suitable for production log aggregation pipelines.

### CI/CD Automation

A fully operational **GitHub Actions** workflow (`.github/workflows/dotnet-ci.yml`) triggers on pushes to `main` and `master`. The pipeline:

1. Checks out the repository
2. Installs the .NET 6 SDK
3. Restores dependencies from `backend/src`
4. Builds in **Release** configuration
5. Executes the full test suite

### Premium UI/UX System

Beyond the baseline table requirement, the frontend delivers a polished operator experience:

- **Persistent sidebar navigation** (`mat-sidenav`) with brand identity ("Fundo Loans" / `account_balance` icon), theme toggle, and logout
- **Real-time advanced filtering** — Search by applicant name, tax ID (CPF/CNPJ), contract number, and loan status (All / Active / Paid)
- **Full table pagination** — `MatTableDataSource` + `mat-paginator` handling 50 seeded records seamlessly (5 / 10 / 25 per page)
- **Route transition progress bar** — Indeterminate `mat-progress-bar` during navigation events
- **Global error pipeline** — RxJS `catchError` with `MatSnackBar` warnings displayed for **10 seconds** on payment failures
- **Dark / Light mode** — Theme-aware surfaces with explicit contrast rules for tables, cards, filters, and sidebar typography
- **Loan application form** — Dedicated `/loans/new` route with reactive validation and success feedback

---

## 💻 How to Run & Environment Setup

### Option 1: Docker Compose (Recommended)

From the **repository root**, start the full backend stack (SQL Server 2022 + API):

```bash
docker compose up --build
```

| Service | URL / Port | Notes |
|---------|------------|-------|
| API | `http://localhost:8080` | ASP.NET Core running in **Production** |
| SQL Server | `localhost:1433` | SA password: `YourStrong@Passw0rd123` |

The API container sets `ASPNETCORE_ENVIRONMENT=Production`, which triggers **automatic EF Core migrations** at startup (`Database.Migrate()` in `Startup.cs`). The database is seeded with **50 realistic loan records** on first run.

> **Frontend:** Docker Compose currently provisions the backend only. Run the Angular app locally (see Option 2) and point `environment.ts` to `http://localhost:8080`.

### Option 2: Local Development

#### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js 18+](https://nodejs.org/) and npm
- SQL Server or LocalDB (for non-Docker backend runs)

#### Backend

```bash
cd backend/src
dotnet restore
dotnet run --project Fundo.Applications.WebApi
```

If port **5000** is occupied by a local agent or another process, bind an alternate port explicitly:

**PowerShell (Windows):**
```powershell
$env:ASPNETCORE_URLS="http://localhost:8080"
dotnet run --project Fundo.Applications.WebApi
```

**Bash (macOS / Linux):**
```bash
export ASPNETCORE_URLS="http://localhost:8080"
dotnet run --project Fundo.Applications.WebApi
```

The API will be available at `http://localhost:8080` (or the port you configured).

#### Frontend

```bash
cd frontend
npm install
npm start
```

The Angular dev server starts at `http://localhost:4200`. Ensure `frontend/src/environments/environment.ts` points to your API base URL:

```typescript
export const environment = {
  apiBaseUrl: 'http://localhost:8080',
};
```

### Demo Credentials

Use the following credentials on the login page:

| Field | Value |
|-------|-------|
| **Username** | `admin` |
| **Password** | `admin` |

---

## 🧪 Automated Testing

The backend test suite lives in `Fundo.Services.Tests` and covers both **unit** and **integration** layers.

### Run Tests

From the solution directory:

```bash
cd backend/src
dotnet test
```

For CI parity, run in Release mode:

```bash
dotnet test --configuration Release
```

### Test Architecture

| Layer | Scope | Isolation Strategy |
|-------|-------|-------------------|
| **Unit Tests** | `LoanService` business rules (status transitions, payment bounds) and `LoansController` routing (creation, 404 handling, dependency wiring) | EF Core **InMemory** database per test run via `LoanRepository` |
| **Integration Tests** | Full HTTP pipeline — loan creation, partial payment, full settlement, overpayment rejection | `CustomWebApplicationFactory` replaces SQL Server with **InMemory** provider |

The `CustomWebApplicationFactory`:

1. Spins up the real Web API host via `WebApplicationFactory<Startup>`
2. Swaps `FundoDbContext` registration for an in-memory database (unique name per factory instance)
3. Exposes `CreateAuthenticatedClient()` — performs a login against `/api/auth/login` and attaches the JWT automatically

This design **completely shields test executions from external infrastructure** — no SQL Server instance, Docker container, or network dependency is required to run the suite.

---

## API Endpoints (Reference)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/auth/login` | Public | Obtain JWT Bearer token |
| `GET` | `/api/loans` | Bearer | List all loans |
| `GET` | `/api/loans/{id}` | Bearer | Retrieve loan by ID |
| `POST` | `/api/loans` | Bearer | Create a new loan application |
| `POST` | `/api/loans/{id}/payment` | Bearer | Register a payment against a loan |

---

## Project Structure

```
loan-management-system-main/
├── backend/
│   ├── Dockerfile
│   └── src/
│       ├── Fundo.Domain/
│       ├── Fundo.Application/
│       ├── Fundo.Infrastructure/
│       ├── Fundo.Applications.WebApi/
│       └── Fundo.Services.Tests/
├── frontend/
│   └── src/app/
│       ├── components/
│       ├── services/
│       ├── guards/
│       └── interceptors/
├── .github/workflows/dotnet-ci.yml
├── docker-compose.yml
└── README.md
```

---

## License

This project was developed as a take-home assessment submission.
