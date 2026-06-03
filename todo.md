# DMV Automation SaaS — MVP Implementation TODO Checklist

This checklist is designed to be used alongside `prompt_plan.md`.

Project: **Car Dealership DMV Automation SaaS**  
Architecture: **Monorepo, .NET 8 Modular Monolith Backend, Next.js Frontend, Azure-first infrastructure**  
MVP rule: **No AI / No OCR in V1**  
Delivery style: **Small incremental TDD steps, one prompt/branch/PR at a time**

---

## How to Use This Checklist

Use one implementation branch per prompt.

Example branch naming:

```text
prompt-01-project-skeleton
prompt-02-backend-shared-architecture
prompt-03-persistence-foundation
```

For every prompt:

- [ ] Create a new branch.
- [ ] Paste only that prompt into the coding agent.
- [ ] Let the coding agent implement the smallest safe version.
- [ ] Review all changed files manually.
- [ ] Run backend tests.
- [ ] Run frontend tests if frontend was touched.
- [ ] Run lint/build.
- [ ] Confirm no orphaned code.
- [ ] Confirm code is wired into API/UI where required.
- [ ] Commit with a clear message.
- [ ] Open PR.
- [ ] Merge only after tests pass.

---

# Global MVP Rules

## Scope Rules

- [ ] Do not add AI features in V1.
- [ ] Do not add OCR extraction in V1.
- [ ] Do not add electronic DMV submission in V1.
- [ ] Do not add credit bureau/lender approval workflows in V1.
- [ ] Do not add payment processing in V1.
- [ ] Do not add accounting integrations in V1.
- [ ] Do not add customer portal accounts in V1.
- [ ] Do not add SSO/MFA in V1.
- [ ] Do not add multi-location dealership hierarchy in V1.
- [ ] Keep V1 focused on deterministic automation.

## Architecture Rules

- [ ] Use a monorepo.
- [ ] Keep `/backend`, `/frontend`, `/infra`, and `/docs` clearly separated.
- [ ] Backend must be .NET 8.
- [ ] Backend must follow modular monolith boundaries.
- [ ] Frontend must use Next.js + React + TypeScript.
- [ ] Database must be SQL Server / Azure SQL compatible.
- [ ] File storage must use an abstraction compatible with Azure Blob Storage.
- [ ] Long-running tasks must use background jobs.
- [ ] Shared rules must live in backend services, not duplicated in frontend.
- [ ] Every feature must have tests before it is considered done.

## Security Rules

- [ ] Every tenant-scoped record must have `TenantId`.
- [ ] Environment-scoped data must distinguish `Sandbox` and `Production`.
- [ ] Dealer users must never access another tenant’s data.
- [ ] Sandbox data must never appear in Production queries.
- [ ] Sensitive data must be encrypted at rest.
- [ ] Sensitive data must be masked by default.
- [ ] Sensitive field reveal must require explicit action.
- [ ] Sensitive field reveal must be audit logged.
- [ ] All document downloads must be audit logged.
- [ ] Platform impersonation must require reason and visible banner.
- [ ] Platform impersonation must be audit logged.
- [ ] All external integration secrets must use secure configuration/Key Vault abstraction.

## Testing Rules

- [ ] Add unit tests for domain rules.
- [ ] Add integration tests for API/database behavior.
- [ ] Add tenant isolation tests for every tenant-scoped module.
- [ ] Add environment isolation tests for sandbox/production behavior.
- [ ] Add frontend component tests for important screens.
- [ ] Add E2E tests for critical workflows.
- [ ] Tests must run locally.
- [ ] Tests must run in CI.
- [ ] Do not rely on live external services in automated tests.
- [ ] Use fake adapters for VIN, email, DocuSign, blob storage, and PDF where needed.

---

# Phase 0 — Repository and Foundation

## Prompt 01 — Repository and Project Skeleton

### Branch

- [ ] Create branch `prompt-01-project-skeleton`.

### Backend

- [x] Create `/backend`.
- [x] Create .NET solution.
- [x] Create ASP.NET Core Web API project.
- [x] Create backend unit test project.
- [x] Create backend integration test project.
- [x] Add `/src` folder structure.
- [x] Add health endpoint `GET /health`.
- [x] Health endpoint returns HTTP 200.
- [x] Health endpoint returns app metadata.
- [x] Add basic API configuration files.
- [x] Add local appsettings.
- [x] Add development appsettings.
- [ ] Add test appsettings if needed.

### Frontend

- [x] Create `/frontend`.
- [x] Create Next.js app.
- [x] Enable TypeScript.
- [x] Add basic landing page.
- [x] Add frontend test setup.
- [x] Add lint setup.
- [x] Add build script.

### Docs

- [x] Create `/docs`.
- [x] Add local setup instructions.
- [x] Add backend run command.
- [x] Add frontend run command.
- [x] Add backend test command.
- [x] Add frontend test command.

### Infrastructure

- [x] Create `/infra`.
- [x] Add placeholder Azure folder.
- [x] Add placeholder deployment notes.

### Tests

- [x] Backend test verifies `/health` returns 200.
- [x] Frontend smoke test verifies landing page renders.
- [x] Backend builds successfully.
- [x] Frontend builds successfully.
- [x] README explains local startup.

### Done

- [ ] No business features implemented yet.
- [ ] Repo structure is clean.
- [ ] First commit created.

---

## Prompt 02 — Backend Shared Architecture

### Branch

- [ ] Create branch `prompt-02-backend-shared-architecture`.

### Backend Shared Components

- [x] Add common `Result` type.
- [x] Add success result pattern.
- [x] Add failure result pattern.
- [x] Add standard API error response.
- [x] Add global exception handling middleware.
- [x] Add correlation ID middleware.
- [x] Add request logging middleware.
- [x] Add base entity abstraction.
- [x] Add `Id`.
- [x] Add `CreatedAtUtc`.
- [x] Add `UpdatedAtUtc`.
- [x] Add `CreatedByUserId`.
- [x] Add `UpdatedByUserId`.
- [x] Add `IsDeleted`.
- [x] Add `DeletedAtUtc`.
- [x] Add `TenantEnvironment` enum with `Sandbox` and `Production`.

### Tests

- [x] Unit test `Result.Success`.
- [x] Unit test `Result.Failure`.
- [x] Unit test error response mapping.
- [x] Integration test unhandled exception returns standard error shape.
- [x] Integration test correlation ID is returned in response headers.
- [ ] Integration test request without correlation ID receives generated ID.
- [ ] Integration test request with correlation ID preserves/returns it.

### Done

- [x] Middleware wired into API pipeline.
- [x] No unused middleware left disconnected.
- [x] No unused abstractions left without immediate purpose.
- [x] Tests pass.

---

## Prompt 03 — Persistence Foundation

### Branch

- [ ] Create branch `prompt-03-persistence-foundation`.

### Database Setup

- [ ] Add EF Core packages.
- [ ] Add SQL Server provider.
- [ ] Add `ApplicationDbContext`.
- [ ] Add design-time DbContext factory.
- [ ] Add migration support.
- [ ] Add base entity configuration.
- [ ] Add soft-delete query filter.
- [ ] Add created/updated timestamp interceptor.
- [ ] Add local connection string configuration.
- [ ] Add migration command documentation.

### Initial Entities

- [ ] Add `Tenant`.
- [ ] Add `TenantEnvironmentRecord`.
- [ ] Add `TenantSettings`.
- [ ] Add `AppUser`.
- [ ] Add `Role`.
- [ ] Add `Permission`.
- [ ] Add `UserRole`.

### Testing Infrastructure

- [ ] Add integration test database setup.
- [ ] Use Testcontainers SQL Server if available.
- [ ] Add documented fallback if Testcontainers is unavailable.
- [ ] Ensure tests do not use production database.

### Tests

- [ ] Integration test migrations apply cleanly.
- [ ] Integration test entity can be inserted.
- [ ] Integration test `CreatedAtUtc` is set.
- [ ] Integration test `UpdatedAtUtc` is set.
- [ ] Integration test soft-deleted records are hidden by default.
- [ ] Integration test tenant-scoped entity requires `TenantId`.

### Done

- [ ] First migration created.
- [ ] Database can be created locally.
- [ ] Tests pass.

---

# Phase 1 — Tenant, Auth, RBAC, Audit

## Prompt 04 — Tenant and Environment Creation

### Branch

- [ ] Create branch `prompt-04-tenant-environment-creation`.

### Tenant Model

- [ ] Tenant represents one dealership location.
- [ ] Tenant has unique slug.
- [ ] Tenant has legal name.
- [ ] Tenant has subscription/status field.
- [ ] Tenant status supports Trial, Active, Suspended, Canceled.

### Environment Model

- [ ] Every tenant receives Sandbox environment.
- [ ] Every tenant receives Production environment.
- [ ] Sandbox and Production environment records are distinct.
- [ ] Environment value is required on environment-scoped data.

### Tenant Settings

- [ ] Create default tenant settings.
- [ ] Include dealer profile placeholders.
- [ ] Include branding placeholders.
- [ ] Include feature flag defaults.
- [ ] Include deal numbering settings.
- [ ] Include session timeout default of 15 minutes.

### API

- [ ] Add development-only `POST /api/tenants/bootstrap`.
- [ ] Add placeholder `GET /api/tenants/current`.
- [ ] Ensure bootstrap is not available in production.

### Tests

- [ ] Unit test tenant creation creates Sandbox and Production.
- [ ] Integration test tenant settings are created.
- [ ] Integration test duplicate tenant slug rejected.
- [ ] Integration test duplicate legal name behavior is defined.
- [ ] Integration test environment records are distinct.
- [ ] Integration test bootstrap endpoint disabled outside development.

### Done

- [ ] Tenant creation is reusable by future registration.
- [ ] No auth logic duplicated here.
- [ ] Tests pass.

---

## Prompt 05 — RBAC Foundation

### Branch

- [ ] Create branch `prompt-05-rbac-foundation`.

### Roles

- [ ] Add `DealerAdmin`.
- [ ] Add `PlatformSuperAdmin`.
- [ ] Model future `Salesperson`.
- [ ] Model future `FinanceManager`.
- [ ] Model future `TitleClerk`.
- [ ] Model future `ReadOnlyAuditor`.

### Permissions

- [ ] Create permission catalog.
- [ ] Add tenant-scoped permission naming convention.
- [ ] Add platform permission naming convention.
- [ ] Add role-permission mapping.
- [ ] Add authorization policy names.
- [ ] Add permission constants.
- [ ] Prevent duplicate permission keys.

### Current User/Tenant

- [ ] Add `ICurrentUser`.
- [ ] Add current user implementation placeholder.
- [ ] Add `ICurrentTenant`.
- [ ] Add current tenant implementation placeholder.
- [ ] Add authorization helper/service.

### Tests

- [ ] Unit test permission catalog has no duplicates.
- [ ] Unit test DealerAdmin receives expected permissions.
- [ ] Unit test PlatformSuperAdmin receives expected permissions.
- [ ] Integration test unauthorized request returns standard 401/403.
- [ ] Integration test policy registration works.

### Done

- [ ] Controllers do not hard-code role checks.
- [ ] Policies are reusable.
- [ ] Tests pass.

---

## Prompt 06 — Registration and Email Verification

### Branch

- [ ] Create branch `prompt-06-registration-email-verification`.

### Registration Flow

- [ ] Add dealership registration request DTO.
- [ ] Validate legal name.
- [ ] Validate admin first name.
- [ ] Validate admin last name.
- [ ] Validate admin email.
- [ ] Validate password.
- [ ] Create tenant.
- [ ] Create Sandbox and Production.
- [ ] Create tenant settings.
- [ ] Create initial DealerAdmin user.
- [ ] Hash password using adaptive algorithm.
- [ ] User starts as unverified.
- [ ] Generate email verification token.
- [ ] Store token hash only.
- [ ] Store token expiration.
- [ ] Send verification email through email abstraction.

### API

- [ ] Add `POST /api/auth/register-dealership`.
- [ ] Add `POST /api/auth/verify-email`.
- [ ] Add `POST /api/auth/resend-verification`.

### Email

- [ ] Add `IEmailSender`.
- [ ] Add fake email sender for development/test.
- [ ] Do not configure real provider yet.
- [ ] Add development-only way to inspect fake email if needed.

### Tests

- [ ] Unit test password hash verification.
- [ ] Unit test raw password is never stored.
- [ ] Unit test token is hashed before storage.
- [ ] Unit test expired token rejected.
- [ ] Unit test consumed token rejected.
- [ ] Integration test registration creates tenant.
- [ ] Integration test registration creates admin user.
- [ ] Integration test registration creates both environments.
- [ ] Integration test unverified user cannot access production-protected endpoint.
- [ ] Integration test verification marks user verified.
- [ ] Integration test resend verification creates new token.

### Frontend

- [ ] Add registration page.
- [ ] Add verify email page.
- [ ] Add manual token entry for local development.
- [ ] Show success/failure messages.

### Done

- [ ] Registration reuses tenant creation service.
- [ ] No raw tokens stored.
- [ ] No real email provider required.
- [ ] Tests pass.

---

## Prompt 07 — Login, Logout, Session Timeout

### Branch

- [ ] Create branch `prompt-07-login-logout-session-timeout`.

### Authentication

- [ ] Choose secure auth approach.
- [ ] Document auth choice in `/docs/auth.md`.
- [ ] Add login endpoint.
- [ ] Add logout endpoint.
- [ ] Add current user endpoint.
- [ ] Add forgot password endpoint.
- [ ] Add reset password endpoint.
- [ ] Add failed login count.
- [ ] Add account throttling or lockout.
- [ ] Add timeout calculation.
- [ ] Default timeout is 15 minutes.
- [ ] Logout invalidates session/token.
- [ ] Unverified users cannot log in or access protected features.

### API

- [ ] Add `POST /api/auth/login`.
- [ ] Add `POST /api/auth/logout`.
- [ ] Add `POST /api/auth/forgot-password`.
- [ ] Add `POST /api/auth/reset-password`.
- [ ] Add `GET /api/auth/me`.

### Tests

- [ ] Integration test valid login.
- [ ] Integration test invalid password fails.
- [ ] Integration test failed login increments counter.
- [ ] Integration test lockout/throttling behavior.
- [ ] Integration test unverified user blocked.
- [ ] Integration test logout prevents further access.
- [ ] Unit test timeout calculation.
- [ ] Unit test reset password token expiration.
- [ ] Integration test reset password changes password.

### Frontend

- [ ] Add login page.
- [ ] Add forgot password page.
- [ ] Add reset password page.
- [ ] Add authenticated layout shell.
- [ ] Add logout button.
- [ ] Display current user.
- [ ] Display current tenant name.

### Done

- [ ] Auth works locally.
- [ ] Auth tests pass.
- [ ] Auth docs updated.

---

## Prompt 08 — Tenant Context Middleware

### Branch

- [ ] Create branch `prompt-08-tenant-context-middleware`.

### Tenant Context

- [ ] Resolve tenant from authenticated user.
- [ ] Resolve environment from request.
- [ ] Support `X-Tenant-Environment`.
- [ ] Validate environment value.
- [ ] Reject missing environment when required.
- [ ] Allow local default to Sandbox only in development.
- [ ] Prevent DealerAdmin from accessing another tenant.
- [ ] Prepare for future impersonation but do not implement it yet.

### Database Filtering

- [ ] Apply tenant filter to tenant-scoped queries.
- [ ] Apply environment filter to environment-scoped queries.
- [ ] Ensure filters cannot be accidentally bypassed in normal repositories.
- [ ] Add repository/query pattern if needed.

### Tests

- [ ] Integration test DealerAdmin can access own tenant.
- [ ] Integration test DealerAdmin cannot access another tenant.
- [ ] Integration test Sandbox data is not returned in Production.
- [ ] Integration test Production data is not returned in Sandbox.
- [ ] Integration test invalid environment header rejected.
- [ ] Integration test missing environment handled as designed.
- [ ] Integration test tenant filter applies to sample entity.

### Frontend

- [ ] API client sends selected environment header.
- [ ] Default environment is clear to user.
- [ ] Environment selector placeholder can be added if not yet in full shell.

### Done

- [ ] Tenant/environment context is centralized.
- [ ] No controllers manually filter tenant in inconsistent ways.
- [ ] Tests pass.

---

## Prompt 09 — Audit Logging Framework

### Branch

- [ ] Create branch `prompt-09-audit-logging-framework`.

### Audit Entity

- [ ] Add `AuditLog`.
- [ ] Store tenant id.
- [ ] Store environment.
- [ ] Store user id.
- [ ] Store action.
- [ ] Store entity type.
- [ ] Store entity id.
- [ ] Store timestamp.
- [ ] Store IP address.
- [ ] Store user agent.
- [ ] Store metadata JSON.

### Audit Service

- [ ] Add `IAuditService`.
- [ ] Add strongly typed audit action keys.
- [ ] Add audit helper for common events.
- [ ] Ensure audit failure does not hide main business error unless critical.
- [ ] Add query/filter service.

### API

- [ ] Add `GET /api/audit-logs`.
- [ ] Support filters by action.
- [ ] Support filters by entity type.
- [ ] Support filters by date range.
- [ ] Support pagination.

### Wiring

- [ ] Audit registration.
- [ ] Audit login.
- [ ] Audit logout.
- [ ] Audit failed login if appropriate.

### Tests

- [ ] Unit test audit event creation.
- [ ] Integration test login writes audit log.
- [ ] Integration test logout writes audit log.
- [ ] Integration test audit log is tenant isolated.
- [ ] Integration test metadata JSON saved/retrieved.
- [ ] Integration test audit logs require permission.
- [ ] Integration test pagination works.

### Done

- [ ] Audit framework ready for all future modules.
- [ ] Tests pass.

---

# Phase 2 — Tenant Settings and Frontend Shell

## Prompt 10 — Tenant Settings, Branding, Feature Flags

### Branch

- [ ] Create branch `prompt-10-tenant-settings-branding-feature-flags`.

### Dealer Profile Settings

- [ ] Legal name.
- [ ] DBA.
- [ ] Dealer license number.
- [ ] Physical address.
- [ ] Mailing address.
- [ ] County.
- [ ] Phone.
- [ ] Email.
- [ ] Website.
- [ ] Authorized signer.
- [ ] Signer title.
- [ ] Logo reference.

### Branding

- [ ] Brand colors.
- [ ] Email sender name.
- [ ] Email footer.
- [ ] Logo placeholder/reference.

### Feature Flags

- [ ] Billing.
- [ ] DocuSign.
- [ ] In-store signatures.
- [ ] Future AI.
- [ ] DMV submission.
- [ ] Beta features.

### API

- [ ] Add `GET /api/tenant/settings`.
- [ ] Add `PUT /api/tenant/settings`.
- [ ] Add `GET /api/tenant/feature-flags`.
- [ ] Add `PUT /api/tenant/feature-flags`.

### Validation

- [ ] Validate email format.
- [ ] Validate phone format.
- [ ] Validate session timeout range.
- [ ] Validate color values if implemented.
- [ ] Prevent DealerAdmin from changing platform-only flags if required.

### Tests

- [ ] Integration test DealerAdmin can update own settings.
- [ ] Integration test cannot update another tenant’s settings.
- [ ] Integration test invalid session timeout rejected.
- [ ] Unit test feature flag defaults.
- [ ] Integration test settings changes are audited.
- [ ] Integration test feature flag changes are audited.

### Frontend

- [ ] Add Settings page.
- [ ] Add Dealer Profile form.
- [ ] Add Branding form.
- [ ] Add Feature Flags display/edit based on permission.
- [ ] Add validation messages.
- [ ] Save and reload settings.

### Done

- [ ] Settings are fully wired backend to frontend.
- [ ] Tests pass.

---

## Prompt 11 — Frontend App Shell

### Branch

- [ ] Create branch `prompt-11-frontend-app-shell`.

### Layout

- [ ] Add protected app layout.
- [ ] Add left navigation.
- [ ] Add top bar.
- [ ] Add responsive layout.
- [ ] Add mobile-friendly navigation if practical.
- [ ] Add loading state.
- [ ] Add unauthorized state.
- [ ] Add error boundary.

### Navigation

- [ ] Dashboard.
- [ ] Customers.
- [ ] Inventory.
- [ ] Deals.
- [ ] Documents.
- [ ] Forms.
- [ ] Reports.
- [ ] Settings.
- [ ] Super Admin only visible to PlatformSuperAdmin.

### Environment Selector

- [ ] Add Sandbox/Production selector.
- [ ] Clearly display current environment.
- [ ] Persist selected environment safely.
- [ ] Send selected environment on API requests.
- [ ] Add visual warning/label for Sandbox.
- [ ] Add visual warning/label for Production.

### API Client

- [ ] Centralize API client.
- [ ] Add auth handling.
- [ ] Add environment header.
- [ ] Add standard error handling.
- [ ] Add correlation ID support if useful.

### Tests

- [ ] Component test DealerAdmin nav.
- [ ] Component test Super Admin hidden from DealerAdmin.
- [ ] Component test PlatformSuperAdmin sees Super Admin.
- [ ] Component test environment selector changes API header.
- [ ] E2E login lands on dashboard.
- [ ] E2E logout returns to login.

### Done

- [ ] All placeholder routes use shared shell.
- [ ] No duplicate API clients.
- [ ] Tests pass.

---

# Phase 3 — Customers and Driver License Capture

## Prompt 12 — Customer Backend

### Branch

- [ ] Create branch `prompt-12-customer-backend`.

### Entities

- [ ] Add `Customer`.
- [ ] Add `CustomerOwner`.
- [ ] Add `DriverLicense`.
- [ ] Add tenant id.
- [ ] Add environment where required.
- [ ] Add soft delete fields.
- [ ] Add audit metadata.

### Customer Features

- [ ] Create customer.
- [ ] Update customer.
- [ ] Get customer detail.
- [ ] Search customers.
- [ ] Soft delete customer.
- [ ] Restore customer.
- [ ] Support primary owner.
- [ ] Support additional owners.
- [ ] Preferred language: English.
- [ ] Preferred language: Spanish.
- [ ] Phone required for each buyer/owner.

### Search Fields

- [ ] Name.
- [ ] Phone.
- [ ] Driver license number.
- [ ] Email.
- [ ] Address.

### API

- [ ] Add `GET /api/customers`.
- [ ] Add `POST /api/customers`.
- [ ] Add `GET /api/customers/{id}`.
- [ ] Add `PUT /api/customers/{id}`.
- [ ] Add `DELETE /api/customers/{id}`.
- [ ] Add `POST /api/customers/{id}/restore`.

### Tests

- [ ] Unit test customer validation.
- [ ] Unit test phone required.
- [ ] Unit test language validation.
- [ ] Integration test create customer with primary owner.
- [ ] Integration test add additional owner.
- [ ] Integration test search by name.
- [ ] Integration test search by phone.
- [ ] Integration test search by email.
- [ ] Integration test soft delete hides customer.
- [ ] Integration test restore works.
- [ ] Integration test tenant isolation.
- [ ] Integration test environment isolation.
- [ ] Integration test audit logs for create/update/delete/restore.

### Done

- [ ] Customer backend usable without DL scan.
- [ ] Tests pass.

---

## Prompt 13 — AAMVA Driver License Parser

### Branch

- [ ] Create branch `prompt-13-aamva-driver-license-parser`.

### Parser Service

- [ ] Add `IAamvaParser`.
- [ ] Accept raw PDF417 text payload.
- [ ] Parse first name.
- [ ] Parse middle name.
- [ ] Parse last name.
- [ ] Parse address.
- [ ] Parse city.
- [ ] Parse state.
- [ ] Parse ZIP.
- [ ] Parse date of birth.
- [ ] Parse DL number.
- [ ] Parse DL expiration.
- [ ] Normalize dates.
- [ ] Return validation errors for malformed payload.
- [ ] Do not perform OCR.

### Scan Metadata

- [ ] Store scan method.
- [ ] Store scan timestamp.
- [ ] Store scanning user.
- [ ] Store raw payload only if platform policy allows.
- [ ] Secure raw payload if stored.

### API

- [ ] Add `POST /api/driver-license/parse`.
- [ ] Add `POST /api/customers/from-driver-license`.

### Frontend

- [ ] Add DL scan section/page.
- [ ] Add large scanner input field.
- [ ] Add manual paste support.
- [ ] Show parsed fields before save.
- [ ] Allow admin correction before save.
- [ ] Save corrected customer.

### Tests

- [ ] Unit test valid AAMVA payload.
- [ ] Unit test malformed payload.
- [ ] Unit test missing optional fields.
- [ ] Unit test date parsing.
- [ ] Unit test parser does not throw on unknown fields.
- [ ] Integration test parse endpoint.
- [ ] Integration test customer created from reviewed DL data.
- [ ] Integration test raw scan payload not stored when disabled.
- [ ] Integration test audit log for DL scan save.

### Done

- [ ] DL barcode parsing works.
- [ ] Admin review/correction is required before save.
- [ ] Tests pass.

---

## Prompt 14 — Sensitive Data Encryption and Masking

### Branch

- [ ] Create branch `prompt-14-sensitive-data-encryption-masking`.

### Sensitive Fields

- [ ] DL number encrypted.
- [ ] DOB encrypted or protected according to chosen implementation.
- [ ] Design supports future sensitive fields.
- [ ] Sensitive fields masked by default.
- [ ] Reveal requires explicit action.
- [ ] Reveal writes audit log.

### Encryption

- [ ] Add encryption abstraction.
- [ ] Add local development encryption key.
- [ ] Add configuration validation.
- [ ] Add production warning if using dev key.
- [ ] Add round-trip encryption/decryption.
- [ ] Ensure raw database values are not plaintext.

### API

- [ ] Normal customer GET returns masked values.
- [ ] Add `POST /api/customers/{customerId}/owners/{ownerId}/reveal-sensitive`.
- [ ] Reveal endpoint requires permission.
- [ ] Reveal endpoint audits action.
- [ ] Reveal endpoint returns only requested sensitive values.

### Frontend

- [ ] Show masked DL number.
- [ ] Show masked DOB.
- [ ] Add Reveal button.
- [ ] Add confirmation dialog.
- [ ] Show warning after reveal.
- [ ] Optionally auto-hide after short time.

### Tests

- [ ] Unit test masking format.
- [ ] Unit test encryption round trip.
- [ ] Integration test database value is encrypted.
- [ ] Integration test normal GET returns masked data.
- [ ] Integration test unauthorized reveal blocked.
- [ ] Integration test authorized reveal returns unmasked data.
- [ ] Integration test reveal writes audit log.
- [ ] Component test Reveal button behavior.

### Done

- [ ] Sensitive data never appears by default.
- [ ] Tests pass.

---

# Phase 4 — Documents

## Prompt 15 — Blob Storage and Document Foundation

### Branch

- [ ] Create branch `prompt-15-blob-storage-document-foundation`.

### Storage Abstraction

- [ ] Add `IBlobStorageService` or equivalent.
- [ ] Add local file storage provider for development.
- [ ] Add Azure Blob-ready provider abstraction.
- [ ] Generate safe blob paths.
- [ ] Include tenant id in storage path.
- [ ] Include environment in storage path.
- [ ] Prevent path traversal.

### Document Entities

- [ ] Add `Document`.
- [ ] Add `DocumentVersion`.
- [ ] Add `ExtractedData` JSON column.
- [ ] Ensure `ExtractedData` defaults to null.
- [ ] Add category.
- [ ] Add notes.
- [ ] Add current version marker.
- [ ] Add soft delete/recover.

### Categories

- [ ] DriverLicense.
- [ ] Insurance.
- [ ] Title.
- [ ] Registration.
- [ ] LienholderFinancing.
- [ ] PayoffLetter.
- [ ] ProofOfAddress.
- [ ] BillOfSale.
- [ ] OdometerDisclosure.
- [ ] PowerOfAttorney.
- [ ] DMVForms.
- [ ] SignedDocuments.
- [ ] Other.

### API

- [ ] Add `POST /api/documents/upload`.
- [ ] Add `GET /api/documents/{id}`.
- [ ] Add `GET /api/documents/{id}/download`.
- [ ] Add `POST /api/documents/{id}/versions`.
- [ ] Add `DELETE /api/documents/{id}`.
- [ ] Add `POST /api/documents/{id}/restore`.

### Frontend

- [ ] Add Documents page.
- [ ] Add upload form.
- [ ] Add category selector.
- [ ] Add notes.
- [ ] Add document list.
- [ ] Add download action.
- [ ] Add delete/restore if permission allows.

### Tests

- [ ] Unit test category validation.
- [ ] Integration test upload creates document.
- [ ] Integration test upload creates first version.
- [ ] Integration test second upload creates new version.
- [ ] Integration test current version switches.
- [ ] Integration test `ExtractedData` remains null.
- [ ] Integration test tenant isolation.
- [ ] Integration test environment isolation.
- [ ] Integration test download writes audit log.
- [ ] Integration test soft delete/restore.
- [ ] Component test upload form.

### Done

- [ ] Documents are stored only, no OCR.
- [ ] Tests pass.

---

# Phase 5 — Inventory and VIN

## Prompt 16 — Inventory Backend

### Branch

- [ ] Create branch `prompt-16-inventory-backend`.

### Inventory Entity

- [ ] VIN.
- [ ] Year.
- [ ] Make.
- [ ] Model.
- [ ] Trim.
- [ ] Body type.
- [ ] Mileage.
- [ ] Color.
- [ ] Stock number.
- [ ] Purchase cost.
- [ ] Reconditioning cost.
- [ ] Asking price.
- [ ] Sale price placeholder for later deal link.
- [ ] Status.
- [ ] Audit metadata.
- [ ] Soft delete fields.

### Statuses

- [ ] Available.
- [ ] PendingSale.
- [ ] Sold.
- [ ] Archived.

### Business Rules

- [ ] VIN required.
- [ ] VIN format validated.
- [ ] Duplicate active VIN rejected within same tenant.
- [ ] Sold VIN does not block reacquisition.
- [ ] Archived VIN does not block reacquisition.
- [ ] Soft deleted VIN behavior is defined.
- [ ] Status changes audited.

### API

- [ ] Add `GET /api/inventory`.
- [ ] Add `POST /api/inventory`.
- [ ] Add `GET /api/inventory/{id}`.
- [ ] Add `PUT /api/inventory/{id}`.
- [ ] Add `DELETE /api/inventory/{id}`.
- [ ] Add `POST /api/inventory/{id}/restore`.

### Tests

- [ ] Unit test VIN required.
- [ ] Unit test VIN format.
- [ ] Integration test create vehicle.
- [ ] Integration test duplicate active VIN rejected.
- [ ] Integration test sold VIN can be re-added.
- [ ] Integration test archived VIN can be re-added.
- [ ] Integration test status update audited.
- [ ] Integration test tenant isolation.
- [ ] Integration test environment isolation.
- [ ] Integration test soft delete/restore.

### Done

- [ ] Inventory backend works without VIN decode.
- [ ] Tests pass.

---

## Prompt 17 — VIN Decode Adapter

### Branch

- [ ] Create branch `prompt-17-vin-decode-adapter`.

### VIN Decode Service

- [ ] Add `IVinDecoder`.
- [ ] Add fake VIN decoder.
- [ ] Add NHTSA-compatible adapter.
- [ ] Add configuration to select provider.
- [ ] Validate VIN before decode.
- [ ] Do not call live external API in tests.
- [ ] Store original decoded data JSON.
- [ ] Store final confirmed vehicle data.

### API

- [ ] Add `POST /api/vin/decode`.
- [ ] Add decode review DTO.
- [ ] Add `POST /api/inventory/from-vin`.
- [ ] Ensure decode alone does not create inventory.
- [ ] Ensure confirmation creates inventory.

### Frontend

- [ ] Add VIN input/scan field.
- [ ] Add Decode button.
- [ ] Show decoded review form.
- [ ] Allow admin edits.
- [ ] Save confirmed vehicle.

### Tests

- [ ] Unit test invalid VIN rejected before decode.
- [ ] Unit test fake decoder response mapping.
- [ ] Unit test adapter handles provider errors.
- [ ] Integration test decode review does not create vehicle.
- [ ] Integration test confirmed decoded vehicle creates inventory.
- [ ] Integration test original decoded JSON stored.
- [ ] Integration test final confirmed fields stored.
- [ ] Integration test audit log for inventory from VIN.

### Done

- [ ] VIN decode flow is deterministic and testable.
- [ ] Tests pass.

---

## Prompt 18 — Inventory Photos and CSV Import

### Branch

- [ ] Create branch `prompt-18-inventory-photos-csv-import`.

### Inventory Photos

- [ ] Add vehicle photo relationship.
- [ ] Upload photo.
- [ ] Remove photo.
- [ ] Reorder photos.
- [ ] Use document/blob abstraction.
- [ ] Audit photo upload/remove/reorder.

### CSV Import

- [ ] Add CSV parser.
- [ ] VIN required for each row.
- [ ] Support stock number.
- [ ] Support mileage.
- [ ] Support color.
- [ ] Support asking price.
- [ ] Support purchase cost.
- [ ] Support status.
- [ ] Validate each row independently.
- [ ] Import valid rows.
- [ ] Reject invalid rows.
- [ ] Continue after invalid rows.
- [ ] Decode imported VINs where possible.
- [ ] Create import summary.
- [ ] Create error report.
- [ ] Prepare for async/background processing.

### API

- [ ] Add `POST /api/inventory/{id}/photos`.
- [ ] Add `DELETE /api/inventory/{id}/photos/{photoId}`.
- [ ] Add `PUT /api/inventory/{id}/photos/reorder`.
- [ ] Add `POST /api/inventory/import-csv`.
- [ ] Add `GET /api/inventory/imports/{id}`.
- [ ] Add `GET /api/inventory/imports/{id}/error-report`.

### Frontend

- [ ] Add inventory import page.
- [ ] Add CSV upload field.
- [ ] Add import result page.
- [ ] Show total rows.
- [ ] Show imported rows.
- [ ] Show rejected rows.
- [ ] Show row-level errors.
- [ ] Add error report download.
- [ ] Add photo upload section.

### Tests

- [ ] Unit test CSV parser valid row.
- [ ] Unit test CSV parser invalid row.
- [ ] Unit test row-level validation.
- [ ] Integration test partial success import.
- [ ] Integration test duplicate active VIN row rejected.
- [ ] Integration test import error report generated.
- [ ] Integration test imported VIN decode uses fake decoder.
- [ ] Integration test photo upload stores blob/document.
- [ ] Integration test audit logs.
- [ ] Component test import results display.

### Done

- [ ] CSV import is safe and partial-success.
- [ ] Tests pass.

---

## Prompt 19 — Inventory Frontend Completion

### Branch

- [ ] Create branch `prompt-19-inventory-frontend-completion`.

### Inventory List

- [ ] Display vehicle rows.
- [ ] Filter by status.
- [ ] Search by VIN.
- [ ] Search by stock number.
- [ ] Search by make/model.
- [ ] Show status badge.
- [ ] Link to detail page.
- [ ] Link to create page.
- [ ] Link to CSV import page.

### Inventory Detail

- [ ] Show full vehicle details.
- [ ] Show pricing/cost fields.
- [ ] Show photos.
- [ ] Show status.
- [ ] Edit button.
- [ ] Delete/restore where applicable.

### Create/Edit

- [ ] Manual create form.
- [ ] VIN validation.
- [ ] VIN decode flow integrated.
- [ ] Review decoded data before save.
- [ ] Edit vehicle.
- [ ] Save validation errors.

### Tests

- [ ] Component test list renders rows.
- [ ] Component test status filter changes query.
- [ ] Component test create form requires VIN.
- [ ] Component test VIN decode review flow.
- [ ] E2E test create inventory manually.
- [ ] E2E test decode VIN then save.
- [ ] E2E test CSV import navigation.

### Done

- [ ] Inventory is a complete vertical slice.
- [ ] Tests pass.

---

# Phase 6 — Deals and Trade-Ins

## Prompt 20 — Deal Backend Foundation

### Branch

- [ ] Create branch `prompt-20-deal-backend-foundation`.

### Deal Entity

- [ ] Add `Deal`.
- [ ] Add `DealOwner`.
- [ ] Add `DealFinancials`.
- [ ] Add tenant id.
- [ ] Add environment.
- [ ] Add deal number.
- [ ] Add status.
- [ ] Add selected inventory vehicle id.
- [ ] Add soft delete fields.
- [ ] Add audit metadata.

### Deal Statuses

- [ ] Draft.
- [ ] ReviewRequired.
- [ ] ReadyForSignatureOrPrint.
- [ ] SentToDocuSign.
- [ ] PartiallySigned.
- [ ] SignedCompleted.
- [ ] PrintedForWetSignature.
- [ ] Archived.
- [ ] CanceledVoided.

### Deal Numbering

- [ ] Use tenant-configured format.
- [ ] Generate unique deal number.
- [ ] Prevent duplicate deal number within tenant/environment.
- [ ] Unit test sequence behavior.

### Inventory Status Rules

- [ ] Creating deal from Available vehicle sets PendingSale.
- [ ] Cannot create deal from Sold vehicle.
- [ ] Cannot create deal from Archived vehicle unless rule says otherwise.
- [ ] Cancel deal returns vehicle to Available unless admin chooses otherwise later.
- [ ] Inventory status changes audited.

### API

- [ ] Add `GET /api/deals`.
- [ ] Add `POST /api/deals`.
- [ ] Add `GET /api/deals/{id}`.
- [ ] Add `PUT /api/deals/{id}`.
- [ ] Add `POST /api/deals/{id}/cancel`.

### Tests

- [ ] Unit test deal number generation.
- [ ] Unit test valid status transitions.
- [ ] Unit test invalid status transitions.
- [ ] Integration test create deal from Available vehicle.
- [ ] Integration test inventory becomes PendingSale.
- [ ] Integration test cannot create deal from Sold vehicle.
- [ ] Integration test cancel updates deal status.
- [ ] Integration test tenant isolation.
- [ ] Integration test environment isolation.
- [ ] Integration test audit logs.

### Done

- [ ] Draft deal creation works.
- [ ] Tests pass.

---

## Prompt 21 — Deal Owners and Financials

### Branch

- [ ] Create branch `prompt-21-deal-owners-financials`.

### Deal Owners

- [ ] Add existing customer owner to deal.
- [ ] Snapshot owner data at time of association.
- [ ] Support multiple owners.
- [ ] Remove owner from deal.
- [ ] Ensure primary owner behavior is defined.

### Sale Type

- [ ] Cash.
- [ ] Financed.
- [ ] Financed shows lienholder requirements.
- [ ] Cash does not require lienholder.

### Financed Fields

- [ ] Lender/lienholder name.
- [ ] Lienholder address.
- [ ] Optional account/reference number.
- [ ] Amount financed.
- [ ] Down payment.
- [ ] Lien recording info.

### Manual Financial Fields

- [ ] Purchase price.
- [ ] Dealer fee.
- [ ] Title fee.
- [ ] Registration fee.
- [ ] Plate fee.
- [ ] Sales tax.
- [ ] Electronic filing fee.
- [ ] Lien recording fee.
- [ ] Trade-in allowance.
- [ ] Trade-in payoff.
- [ ] Down payment.
- [ ] Amount financed.
- [ ] Balance due.

### API

- [ ] Add `POST /api/deals/{id}/owners`.
- [ ] Add `DELETE /api/deals/{id}/owners/{ownerId}`.
- [ ] Add `PUT /api/deals/{id}/financials`.
- [ ] Add `PUT /api/deals/{id}/sale-type`.

### Tests

- [ ] Unit test owner snapshot.
- [ ] Unit test financed validation.
- [ ] Unit test cash validation.
- [ ] Integration test add owner.
- [ ] Integration test remove owner.
- [ ] Integration test update financials.
- [ ] Integration test update sale type to financed.
- [ ] Integration test missing lienholder fails checklist later or validation where appropriate.
- [ ] Integration test audit logs.

### Done

- [ ] Deal data is ready for trade-ins and checklist.
- [ ] Tests pass.

---

## Prompt 22 — Trade-In Backend

### Branch

- [ ] Create branch `prompt-22-trade-in-backend`.

### Trade-In Entity

- [ ] Add `TradeInVehicle`.
- [ ] VIN.
- [ ] VIN decoded data.
- [ ] Mileage.
- [ ] Year.
- [ ] Make.
- [ ] Model.
- [ ] Trim.
- [ ] Trade-in value.
- [ ] Payoff amount.
- [ ] Lienholder.
- [ ] Title status.
- [ ] Deal relationship.
- [ ] Document relationship.

### Disposition

- [ ] Model AddToInventoryAvailable.
- [ ] Model AddToInventoryArchived.
- [ ] Model Wholesale.
- [ ] Model HoldForReview.
- [ ] Model DoNothing.
- [ ] Do not automatically create inventory.

### API

- [ ] Add `GET /api/deals/{id}/trade-ins`.
- [ ] Add `POST /api/deals/{id}/trade-ins`.
- [ ] Add `PUT /api/deals/{id}/trade-ins/{tradeInId}`.
- [ ] Add `DELETE /api/deals/{id}/trade-ins/{tradeInId}`.
- [ ] Add `POST /api/deals/{id}/trade-ins/{tradeInId}/documents`.

### Tests

- [ ] Unit test trade-in validation.
- [ ] Unit test payoff/lienholder behavior.
- [ ] Integration test add trade-in.
- [ ] Integration test VIN decode for trade-in.
- [ ] Integration test trade-in document upload links correctly.
- [ ] Integration test trade-in does not create inventory automatically.
- [ ] Integration test tenant isolation.
- [ ] Integration test environment isolation.
- [ ] Integration test audit logs.

### Done

- [ ] Trade-ins are linked to deals safely.
- [ ] Tests pass.

---

## Prompt 23 — Deal Frontend Wizard

### Branch

- [ ] Create branch `prompt-23-deal-frontend-wizard`.

### Wizard Steps

- [ ] Step 1: Create/select customer.
- [ ] Step 2: Add buyers/owners.
- [ ] Step 3: Select inventory vehicle.
- [ ] Step 4: Add trade-ins if applicable.
- [ ] Step 5: Enter financial values manually.
- [ ] Step 6: Select cash or financed.
- [ ] Step 7: Review draft deal summary.

### UI Requirements

- [ ] Save each step incrementally.
- [ ] Show deal number.
- [ ] Show deal status.
- [ ] Show selected vehicle.
- [ ] Show owners.
- [ ] Show financial summary.
- [ ] Show trade-ins.
- [ ] Show validation errors.
- [ ] Allow save-and-continue.
- [ ] Avoid one huge final submit.

### Tests

- [ ] Component test wizard step navigation.
- [ ] Component test financed sale shows lienholder fields.
- [ ] Component test cash sale hides lienholder fields.
- [ ] Component test owner selection.
- [ ] Component test vehicle selection.
- [ ] E2E test create draft deal.
- [ ] E2E test add owner.
- [ ] E2E test add financed financials.
- [ ] E2E test add trade-in.

### Done

- [ ] Dealer can create a complete draft deal.
- [ ] No PDF/signature features yet.
- [ ] Tests pass.

---

# Phase 7 — Forms and Mapping

## Prompt 24 — Form Template Backend

### Branch

- [ ] Create branch `prompt-24-form-template-backend`.

### Entities

- [ ] Add `FormTemplate`.
- [ ] Add `FormVersion`.
- [ ] Add `FormFieldMapping`.
- [ ] Add tenant id.
- [ ] Add environment if needed.
- [ ] Add active version marker.
- [ ] Add archive behavior.
- [ ] Add audit metadata.

### Template Behavior

- [ ] Upload one PDF form at a time.
- [ ] Create first version on upload.
- [ ] Support new version upload.
- [ ] Copy previous mapping as draft when possible.
- [ ] Activate reviewed version.
- [ ] Archive instead of delete.
- [ ] Store workflow flags: Print.
- [ ] Store workflow flags: DocuSign.
- [ ] Store workflow flags: In-store signature.
- [ ] No AI mapping.

### API

- [ ] Add `GET /api/forms`.
- [ ] Add `POST /api/forms/upload`.
- [ ] Add `GET /api/forms/{id}`.
- [ ] Add `POST /api/forms/{id}/versions`.
- [ ] Add `POST /api/forms/{id}/activate`.
- [ ] Add `POST /api/forms/{id}/archive`.

### Tests

- [ ] Integration test upload creates template.
- [ ] Integration test upload creates version.
- [ ] Integration test new version copies previous mapping.
- [ ] Integration test activate requires reviewed version or defined criteria.
- [ ] Integration test archive hides template from active list.
- [ ] Integration test tenant isolation.
- [ ] Integration test environment isolation.
- [ ] Integration test audit logs.

### Done

- [ ] Form templates can be uploaded and versioned.
- [ ] Tests pass.

---

## Prompt 25 — Form Field Dictionary and Mapping Model

### Branch

- [ ] Create branch `prompt-25-form-field-dictionary-mapping-model`.

### Field Groups

- [ ] Buyer/Owner.
- [ ] Dealer.
- [ ] Vehicle.
- [ ] Deal.
- [ ] Finance.
- [ ] Trade-In.
- [ ] Fees.
- [ ] Signatures.
- [ ] System.

### Field Types

- [ ] Text.
- [ ] Date.
- [ ] Checkbox.
- [ ] Radio.
- [ ] Dropdown.
- [ ] Initials.
- [ ] Signature.
- [ ] Dealer-only.
- [ ] Customer-only.

### Mapping Properties

- [ ] Existing AcroForm field name.
- [ ] Overlay page number.
- [ ] Overlay X coordinate.
- [ ] Overlay Y coordinate.
- [ ] Overlay width.
- [ ] Overlay height.
- [ ] Font size.
- [ ] Alignment.
- [ ] Owner index.
- [ ] Trade-in index.
- [ ] Required flag.
- [ ] Conditional visibility metadata.
- [ ] Conditional required metadata.

### API

- [ ] Add `GET /api/forms/field-dictionary`.
- [ ] Add `GET /api/forms/{id}/versions/{versionId}/mapping`.
- [ ] Add `PUT /api/forms/{id}/versions/{versionId}/mapping`.
- [ ] Add `POST /api/forms/{id}/versions/{versionId}/preview-test-data`.

### Tests

- [ ] Unit test field dictionary has stable keys.
- [ ] Unit test no duplicate dictionary keys.
- [ ] Unit test mapping validation.
- [ ] Unit test invalid field type rejected.
- [ ] Integration test save mapping.
- [ ] Integration test invalid system field rejected.
- [ ] Integration test owner-indexed field accepted.
- [ ] Integration test trade-in-indexed field accepted.
- [ ] Integration test audit log for mapping change.

### Done

- [ ] Mapping data model is ready for UI and PDF generation.
- [ ] Tests pass.

---

## Prompt 26 — Forms Frontend and Visual Mapper

### Branch

- [ ] Create branch `prompt-26-forms-frontend-visual-mapper`.

### Screens

- [ ] Form template list.
- [ ] Upload form.
- [ ] Form version detail.
- [ ] Mapping editor.
- [ ] Preview/test data screen.

### Mapper MVP

- [ ] Render PDF pages or placeholder if rendering library not configured.
- [ ] Allow adding overlay field.
- [ ] Allow selecting system field from dictionary.
- [ ] Allow manual coordinate entry.
- [ ] Allow existing AcroForm field mapping.
- [ ] Allow field type selection.
- [ ] Allow owner/trade-in index.
- [ ] Allow required flag.
- [ ] Save mapping.
- [ ] Show workflow flags.
- [ ] Show Print support.
- [ ] Show DocuSign support.
- [ ] Show In-store signature support.
- [ ] No AI mapping.

### Tests

- [ ] Component test form list.
- [ ] Component test upload form.
- [ ] Component test field dictionary loads.
- [ ] Component test mapping row can be added.
- [ ] Component test validation errors.
- [ ] E2E test upload form.
- [ ] E2E test save one mapping.
- [ ] E2E test activate form after mapping.

### Done

- [ ] Admin can create and map a form manually.
- [ ] Real PDF generation not required yet.
- [ ] Tests pass.

---

# Phase 8 — Rules, Checklist, PDF Packets

## Prompt 27 — Rule Engine and Pre-Generation Checklist

### Branch

- [ ] Create branch `prompt-27-rule-engine-pre-generation-checklist`.

### Rule Engine

- [ ] Add `RuleConfig`.
- [ ] Rule is tenant-specific.
- [ ] Rule is state-aware.
- [ ] Rule changes are auditable.
- [ ] Rule changes are versioned or history-preserved.
- [ ] MVP rules are behind-the-scenes/admin configured.

### Form Selection Conditions

- [ ] Tenant.
- [ ] State.
- [ ] Transaction type.
- [ ] Buyer state.
- [ ] Cash/financed flag.
- [ ] Trade-in presence.
- [ ] Lienholder presence.
- [ ] Multiple owners.
- [ ] Signature mode.

### Checklist Areas

- [ ] Buyer data.
- [ ] Vehicle data.
- [ ] Finance data.
- [ ] Trade-in data.
- [ ] Templates.
- [ ] Documents.
- [ ] Signatures.
- [ ] Overrides.

### Blocking/Warning Behavior

- [ ] Missing required buyer data blocks.
- [ ] Missing required vehicle data blocks.
- [ ] Missing required finance data blocks.
- [ ] Missing required template blocks.
- [ ] Missing required signature fields block.
- [ ] Missing documents can block or warn by rule.
- [ ] Warnings can allow continue with reason if configured.
- [ ] Override reason is required.
- [ ] Override is audit logged.

### API

- [ ] Add `GET /api/deals/{id}/checklist`.
- [ ] Add `POST /api/deals/{id}/checklist/overrides`.
- [ ] Add `GET /api/deals/{id}/forms/selection`.
- [ ] Add `PUT /api/deals/{id}/forms/selection`.

### Tests

- [ ] Unit test Florida cash deal form selection.
- [ ] Unit test financed deal requires lienholder.
- [ ] Unit test trade-in adds trade-in form when configured.
- [ ] Unit test multiple owners condition.
- [ ] Unit test required form cannot be removed without allowed override.
- [ ] Integration test missing buyer phone blocks.
- [ ] Integration test missing active template blocks.
- [ ] Integration test warning override requires reason.
- [ ] Integration test override writes audit log.
- [ ] Integration test tenant isolation.

### Done

- [ ] Packet generation has reliable gatekeeping.
- [ ] Tests pass.

---

## Prompt 28 — PDF Generation Service

### Branch

- [ ] Create branch `prompt-28-pdf-generation-service`.

### PDF Service

- [ ] Add `IPdfGenerationService`.
- [ ] Add PDF library implementation.
- [ ] Support AcroForm fill.
- [ ] Support text overlay.
- [ ] Support date formatting.
- [ ] Support checkbox mapping.
- [ ] Support signature placeholder fields.
- [ ] Support owner-indexed fields.
- [ ] Support trade-in-indexed fields.
- [ ] Add deal data flattener.
- [ ] Store generated PDF through document/blob abstraction.

### Cover Sheet

- [ ] Include deal number.
- [ ] Include deal status.
- [ ] Include buyers/owners.
- [ ] Include vehicle info.
- [ ] Include dealer info.
- [ ] Include included forms.
- [ ] Include uploaded documents.
- [ ] Include signature requirements/status.
- [ ] Include open warnings.
- [ ] Include override reasons.
- [ ] Include submission/manual status.
- [ ] Include template version metadata.

### Sandbox Watermark

- [ ] Add TEST watermark for sandbox PDFs.
- [ ] Ensure Production PDFs are not watermarked.
- [ ] Add test coverage for both.

### Test Fixtures

- [ ] Add small fillable PDF sample.
- [ ] Add small non-fillable overlay PDF sample.
- [ ] Keep fixtures small.
- [ ] Ensure fixtures are legal to store in repo.

### Tests

- [ ] Unit test deal data flattening.
- [ ] Unit test field mapping to values.
- [ ] Test AcroForm fill output.
- [ ] Test overlay output.
- [ ] Unit test cover sheet content.
- [ ] Integration test sandbox PDF watermark.
- [ ] Integration test production PDF no TEST watermark.
- [ ] Integration test generated document stores form version id.

### Done

- [ ] PDF generation can produce individual PDFs and cover sheet.
- [ ] Background jobs not required yet.
- [ ] Tests pass.

---

## Prompt 29 — Packet Generation Background Job

### Branch

- [ ] Create branch `prompt-29-packet-generation-background-job`.

### Background Job

- [ ] Add `BackgroundJob`.
- [ ] Add job type.
- [ ] Add job status Queued.
- [ ] Add job status Running.
- [ ] Add job status Completed.
- [ ] Add job status Failed.
- [ ] Add retry count.
- [ ] Add error details.
- [ ] Add tenant/environment context.
- [ ] Add background worker/service.

### Packet

- [ ] Add `Packet`.
- [ ] Add `PacketDocument`.
- [ ] Link packet to deal.
- [ ] Link packet documents to generated documents.
- [ ] Preserve form order.
- [ ] Preserve form version used.

### API

- [ ] Add `POST /api/deals/{id}/generate-packet`.
- [ ] Add `GET /api/deals/{id}/packet`.
- [ ] Add `GET /api/background-jobs/{id}`.

### Rules

- [ ] Packet generation blocked if checklist has blocking errors.
- [ ] Packet generation allowed if only acknowledged warnings.
- [ ] Packet generation writes audit log.
- [ ] Large generation does not block UI.

### Frontend

- [ ] Add packet step to deal detail.
- [ ] Show checklist result.
- [ ] Show blocking errors.
- [ ] Show warnings.
- [ ] Add Generate Packet button.
- [ ] Show background job status.
- [ ] Show generated documents after completion.

### Tests

- [ ] Unit test job status transitions.
- [ ] Integration test checklist block prevents generation.
- [ ] Integration test valid deal queues packet job.
- [ ] Integration test worker generates packet documents.
- [ ] Integration test failed job stores error details.
- [ ] Integration test generated packet preserves form order.
- [ ] Integration test audit log.
- [ ] Component test checklist display.
- [ ] Component test job progress display.

### Done

- [ ] Dealer can generate a packet asynchronously.
- [ ] Tests pass.

---

## Prompt 30 — ZIP Deal Jacket Export

### Branch

- [ ] Create branch `prompt-30-zip-deal-jacket-export`.

### ZIP Export

- [ ] Add export service.
- [ ] Include packet cover sheet.
- [ ] Include generated forms.
- [ ] Include signed forms when available.
- [ ] Include uploaded supporting documents.
- [ ] Include summary JSON or PDF.
- [ ] Include open warnings.
- [ ] Include override reasons.
- [ ] Include template versions used.
- [ ] Include audit-friendly metadata.
- [ ] Add manifest file.
- [ ] Store export file.
- [ ] Support background job for large export.

### API

- [ ] Add `POST /api/deals/{id}/export-zip`.
- [ ] Add `GET /api/deals/{id}/exports/{exportId}/download`.
- [ ] Add export job status if needed.

### Frontend

- [ ] Add Export Deal Jacket button.
- [ ] Show export progress.
- [ ] Show download link when ready.

### Tests

- [ ] Unit test ZIP manifest creation.
- [ ] Integration test ZIP includes generated packet documents.
- [ ] Integration test ZIP includes uploaded deal documents.
- [ ] Integration test ZIP includes metadata summary.
- [ ] Integration test tenant isolation.
- [ ] Integration test export action writes audit log.
- [ ] Integration test download writes audit log.

### Done

- [ ] Full deal jacket can be exported.
- [ ] Tests pass.

---

# Phase 9 — Signature Workflows

## Prompt 31 — In-Store Signature Workflow

### Branch

- [ ] Create branch `prompt-31-in-store-signature-workflow`.

### Signature Entities

- [ ] Add `SignatureEnvelope` if not already added.
- [ ] Add `SignatureSigner`.
- [ ] Add `SignatureEvent`.
- [ ] Add signer role.
- [ ] Add status.
- [ ] Add timestamp.
- [ ] Add device/browser info.
- [ ] Add user id.
- [ ] Add deal relationship.

### Signer Roles

- [ ] Buyer.
- [ ] Additional owner.
- [ ] Dealer representative.
- [ ] Lienholder where needed.

### Signature Capture

- [ ] Capture signature image/vector.
- [ ] Validate signer role.
- [ ] Store signature securely.
- [ ] Embed captured signature into mapped PDF signature fields.
- [ ] Store completed signed PDF as SignedDocuments.
- [ ] Audit signature capture.

### API

- [ ] Add `POST /api/deals/{id}/capture-signature`.
- [ ] Add `GET /api/deals/{id}/signatures`.

### Frontend

- [ ] Add in-store signature screen.
- [ ] Add canvas capture.
- [ ] Add signer selection.
- [ ] Add clear/retry signature.
- [ ] Add submit signature.
- [ ] Show captured signature status.

### Tests

- [ ] Unit test signer role validation.
- [ ] Unit test signature event creation.
- [ ] Integration test capture signature stores event.
- [ ] Integration test signed PDF stored as SignedDocuments.
- [ ] Integration test audit log for signature capture.
- [ ] Integration test unauthorized tenant blocked.
- [ ] Component test signature canvas submit.
- [ ] E2E test capture in-store signature.

### Done

- [ ] In-store signature workflow works.
- [ ] Tests pass.

---

## Prompt 32 — DocuSign OAuth and Remote Signature Shell

### Branch

- [ ] Create branch `prompt-32-docusign-oauth-remote-signature-shell`.

### DocuSign Integration

- [ ] Add `IDocuSignClient`.
- [ ] Add fake DocuSign client.
- [ ] Add configuration for real DocuSign later.
- [ ] Add OAuth connect flow.
- [ ] Add OAuth callback endpoint.
- [ ] Store token securely.
- [ ] Support token refresh.
- [ ] Support disconnect.
- [ ] Support reconnect.
- [ ] Add health/status check.

### Sending

- [ ] Validate DocuSign feature flag.
- [ ] Validate dealer connected DocuSign.
- [ ] Validate signer email addresses.
- [ ] Create signature envelope.
- [ ] Send selected packet documents.
- [ ] Track Sent.
- [ ] Track Viewed.
- [ ] Track Signed.
- [ ] Track Declined.
- [ ] Track Completed.
- [ ] Store completed PDFs as SignedDocuments.

### API

- [ ] Add `GET /api/integrations/docusign/status`.
- [ ] Add OAuth callback route.
- [ ] Add `POST /api/integrations/docusign/disconnect`.
- [ ] Add `POST /api/deals/{id}/send-docusign`.
- [ ] Add `POST /api/webhooks/docusign`.

### Frontend

- [ ] Add DocuSign settings connection status.
- [ ] Add Connect DocuSign button.
- [ ] Add Disconnect button.
- [ ] Add Send to DocuSign button.
- [ ] Show envelope status.

### Tests

- [ ] Unit test cannot send when feature flag off.
- [ ] Unit test cannot send when signer email missing.
- [ ] Unit test fake DocuSign send.
- [ ] Integration test connection status.
- [ ] Integration test fake send creates envelope.
- [ ] Integration test webhook updates status.
- [ ] Integration test completed document stored.
- [ ] Integration test audit logs.

### Done

- [ ] DocuSign shell is ready for real credentials.
- [ ] Tests pass.

---

## Prompt 33 — Paper Print Workflow

### Branch

- [ ] Create branch `prompt-33-paper-print-workflow`.

### Print Workflow

- [ ] Admin can download/print packet.
- [ ] Mark deal as PrintedForWetSignature.
- [ ] Upload signed scans later.
- [ ] Signed scans stored as SignedDocuments.
- [ ] Print workflow audited.

### API

- [ ] Add `POST /api/deals/{id}/mark-printed`.
- [ ] Add `POST /api/deals/{id}/signed-documents/upload`.

### Frontend

- [ ] Add Print Packet button.
- [ ] Add Mark Printed action.
- [ ] Add Upload Signed Scan action.
- [ ] Show signed documents on deal detail.

### Tests

- [ ] Unit test transition to PrintedForWetSignature.
- [ ] Unit test invalid transition rejected.
- [ ] Integration test mark printed writes audit log.
- [ ] Integration test signed document upload links to deal.
- [ ] Integration test signed document visible in export.
- [ ] Integration test tenant isolation.
- [ ] Component test upload signed scan.

### Done

- [ ] Paper workflow is complete.
- [ ] Tests pass.

---

# Phase 10 — Notifications, Reports, Admin, Billing

## Prompt 34 — Email Notifications

### Branch

- [ ] Create branch `prompt-34-email-notifications`.

### Notification Model

- [ ] Add `Notification`.
- [ ] Add notification type.
- [ ] Add recipient.
- [ ] Add subject.
- [ ] Add body.
- [ ] Add status.
- [ ] Add sent timestamp.
- [ ] Add error details.
- [ ] Add tenant/environment.

### Email Templates

- [ ] Add template storage.
- [ ] Support dealership branding.
- [ ] Support English.
- [ ] Support Spanish.
- [ ] Support editable templates where allowed.
- [ ] Support disabling notification types.

### Notification Types

- [ ] DocuSign sent.
- [ ] Reminder.
- [ ] Missing document request.
- [ ] Deal status update.
- [ ] Completion notice.

### Email Sender

- [ ] Add fake sender.
- [ ] Add provider abstraction.
- [ ] Do not require real provider in tests.
- [ ] Log all sent notifications.
- [ ] Audit template/preference changes.

### API

- [ ] Add notification preferences endpoints.
- [ ] Add email template endpoints.
- [ ] Add test-send endpoint only if safe/dev-only.

### Frontend

- [ ] Add notification settings under Settings.
- [ ] Add template editor if in scope.
- [ ] Add enable/disable notification toggles.

### Tests

- [ ] Unit test template rendering.
- [ ] Unit test disabled notification is not sent.
- [ ] Unit test Spanish template selected.
- [ ] Integration test notification record created.
- [ ] Integration test failed send records error.
- [ ] Integration test template change audited.
- [ ] Component test notification settings.

### Done

- [ ] Email framework ready for DocuSign/deal events.
- [ ] Tests pass.

---

## Prompt 35 — Reports and Dashboard

### Branch

- [ ] Create branch `prompt-35-reports-dashboard`.

### Reports

- [ ] Deals by status.
- [ ] Completed deals.
- [ ] Pending signatures.
- [ ] Vehicles sold.
- [ ] Inventory status.
- [ ] Trade-in activity.
- [ ] Audit log reports.
- [ ] CSV import errors.

### Dashboard Widgets

- [ ] Deals in progress.
- [ ] Review required.
- [ ] Pending signatures.
- [ ] Missing documents.
- [ ] Available vehicles.
- [ ] Pending sale vehicles.
- [ ] Recent activity.
- [ ] Import errors.

### Exports

- [ ] CSV export.
- [ ] PDF export placeholder or implementation.
- [ ] Filters applied before export.
- [ ] Export actions audited.

### API

- [ ] Add `GET /api/reports/deals`.
- [ ] Add `GET /api/reports/inventory`.
- [ ] Add `GET /api/reports/audit`.
- [ ] Add `POST /api/reports/export`.
- [ ] Add `GET /api/dashboard/widgets`.
- [ ] Add `PUT /api/dashboard/layout`.

### Frontend

- [ ] Add dashboard page.
- [ ] Add dashboard widgets.
- [ ] Add reports pages.
- [ ] Add filters.
- [ ] Add export buttons.
- [ ] Add save dashboard layout.

### Tests

- [ ] Integration test report filters by tenant.
- [ ] Integration test report filters by environment.
- [ ] Integration test deal status counts.
- [ ] Integration test inventory status counts.
- [ ] Integration test CSV export.
- [ ] Integration test dashboard layout saved per user.
- [ ] Integration test audit report permissions.
- [ ] Component test dashboard widgets.
- [ ] Component test report filters.

### Done

- [ ] Operational reporting is usable.
- [ ] Tests pass.

---

## Prompt 36 — Platform Super Admin

### Branch

- [ ] Create branch `prompt-36-platform-super-admin`.

### Super Admin Capabilities

- [ ] View tenants.
- [ ] Create/disable tenants.
- [ ] View tenant health.
- [ ] Manage trial/subscription status.
- [ ] Manage feature flags.
- [ ] Manage behind-the-scenes rules by tenant/state.
- [ ] View diagnostics.
- [ ] Search audit logs.
- [ ] Start impersonation.
- [ ] End impersonation.

### Impersonation

- [ ] Mandatory reason required.
- [ ] Visible banner required.
- [ ] Start event audited.
- [ ] End event audited.
- [ ] Impersonation cannot bypass audit.
- [ ] DealerAdmin cannot impersonate.
- [ ] PlatformSuperAdmin cannot silently modify without audit.

### API

- [ ] Add `GET /api/platform/tenants`.
- [ ] Add `GET /api/platform/tenants/{id}`.
- [ ] Add `PUT /api/platform/tenants/{id}/status`.
- [ ] Add `PUT /api/platform/tenants/{id}/feature-flags`.
- [ ] Add `GET /api/platform/diagnostics`.
- [ ] Add `POST /api/platform/impersonation/start`.
- [ ] Add `POST /api/platform/impersonation/end`.

### Frontend

- [ ] Show Super Admin nav only for PlatformSuperAdmin.
- [ ] Add tenant list.
- [ ] Add tenant detail.
- [ ] Add feature flag management.
- [ ] Add impersonation start UI.
- [ ] Add impersonation end UI.
- [ ] Add visible impersonation banner.

### Tests

- [ ] Integration test DealerAdmin blocked from platform endpoints.
- [ ] Integration test PlatformSuperAdmin can list tenants.
- [ ] Integration test impersonation requires reason.
- [ ] Integration test impersonation start audited.
- [ ] Integration test impersonation end audited.
- [ ] Integration test impersonation banner/context present.
- [ ] Integration test impersonation cannot bypass tenant audit.
- [ ] Component test Super Admin nav visibility.

### Done

- [ ] Platform support workflows are controlled and audited.
- [ ] Tests pass.

---

## Prompt 37 — Billing Switch and Subscription Access

### Branch

- [ ] Create branch `prompt-37-billing-switch-subscription-access`.

### Billing Switch

- [ ] Add platform-level Billing Enabled setting.
- [ ] Billing OFF allows internal/private pilot operation.
- [ ] Billing ON enforces subscription access.
- [ ] Audit billing switch changes.

### Subscription States

- [ ] Trial.
- [ ] Active.
- [ ] PastDue.
- [ ] Suspended.
- [ ] Canceled.

### Access Rules

- [ ] Trial allowed when billing ON if valid.
- [ ] Active allowed.
- [ ] PastDue behavior defined.
- [ ] Suspended blocked.
- [ ] Canceled blocked except export grace period.
- [ ] Canceled grace period allows export only.
- [ ] Canceled after grace period blocks export.

### API

- [ ] Add subscription entity if not already present.
- [ ] Add subscription update endpoint for platform admin.
- [ ] Add billing setting endpoint for platform admin.
- [ ] Add export-only mode handling.

### Frontend

- [ ] Show blocked access page.
- [ ] Show export-only notice.
- [ ] Show subscription state in super admin tenant detail.

### Tests

- [ ] Unit test access matrix.
- [ ] Integration test billing OFF allows trial tenant.
- [ ] Integration test billing ON allows active tenant.
- [ ] Integration test billing ON blocks suspended tenant.
- [ ] Integration test canceled grace period export-only.
- [ ] Integration test subscription changes audited.
- [ ] Component test access-blocked page.

### Done

- [ ] Billing switch is implemented without payment processing.
- [ ] Tests pass.

---

# Phase 11 — Tenant Export, Hardening, E2E

## Prompt 38 — Sandbox Reset and Tenant Export

### Branch

- [ ] Create branch `prompt-38-sandbox-reset-tenant-export`.

### Sandbox Reset

- [ ] Add sandbox reset service.
- [ ] Reset only sandbox data.
- [ ] Never delete production data.
- [ ] Require strong confirmation.
- [ ] Audit reset.
- [ ] Preserve required tenant/environment records.

### Tenant Export

- [ ] Export structured data.
- [ ] Export documents.
- [ ] Export forms.
- [ ] Export mappings.
- [ ] Export reports.
- [ ] Export audit logs.
- [ ] Add export manifest.
- [ ] Run large export as background job.
- [ ] Store export file.
- [ ] Audit export and download.

### API

- [ ] Add `POST /api/sandbox/reset`.
- [ ] Add `POST /api/tenant/export`.
- [ ] Add `GET /api/tenant/exports/{id}/download`.

### Frontend

- [ ] Add sandbox reset action in Settings.
- [ ] Add strong confirmation dialog.
- [ ] Add tenant export action in Settings.
- [ ] Show export status.
- [ ] Show download link.

### Tests

- [ ] Integration test sandbox reset deletes sandbox data.
- [ ] Integration test sandbox reset preserves production data.
- [ ] Integration test tenant export includes manifest.
- [ ] Integration test tenant export includes structured data.
- [ ] Integration test tenant export includes documents.
- [ ] Integration test tenant export includes audit logs.
- [ ] Integration test tenant isolation.
- [ ] Integration test audit logs.
- [ ] Component test reset confirmation.

### Done

- [ ] Sandbox management and tenant export are usable.
- [ ] Tests pass.

---

## Prompt 39 — Observability, Security Hardening, and Performance

### Branch

- [ ] Create branch `prompt-39-observability-security-performance`.

### Observability

- [ ] Central logging configured.
- [ ] Correlation IDs included in logs.
- [ ] Error logs include context.
- [ ] Job failures visible.
- [ ] Integration failures visible.
- [ ] Application Insights-ready configuration.
- [ ] Add health diagnostics where safe.

### Security

- [ ] Add security headers.
- [ ] Add auth endpoint rate limiting.
- [ ] Add file upload extension validation.
- [ ] Add file upload size limit.
- [ ] Add content type checks.
- [ ] Add safe error messages.
- [ ] Add production configuration validation.
- [ ] Ensure secrets are not logged.
- [ ] Ensure sensitive fields are not logged.

### Performance

- [ ] Add pagination to list endpoints.
- [ ] Add indexes for tenant/environment/common search fields.
- [ ] Confirm list/search pages target under 2 seconds under normal load.
- [ ] Add benchmark script or documented performance checklist.
- [ ] Ensure background jobs retry and expose status.

### Docs

- [ ] Add `/docs/security.md`.
- [ ] Add `/docs/observability.md`.
- [ ] Add `/docs/performance-checklist.md`.

### Tests

- [ ] Integration test security headers.
- [ ] Integration test auth rate limiting.
- [ ] Integration test oversized file rejected.
- [ ] Integration test unsupported file type rejected.
- [ ] Integration test bad content type rejected.
- [ ] Integration test background job failure visible.
- [ ] Add basic performance/benchmark script.
- [ ] Verify logs do not include secrets in test scenario.

### Done

- [ ] App is hardened for MVP beta.
- [ ] Tests pass.

---

## Prompt 40 — End-to-End MVP Flow and Deployment Runbook

### Branch

- [ ] Create branch `prompt-40-e2e-mvp-flow-deployment-runbook`.

### Happy Path E2E

- [ ] Register dealership.
- [ ] Verify email.
- [ ] Log in.
- [ ] Confirm sandbox environment.
- [ ] Configure dealer profile.
- [ ] Create customer manually.
- [ ] Create customer from AAMVA test payload.
- [ ] Create inventory vehicle with fake VIN decode.
- [ ] Create deal.
- [ ] Add owner.
- [ ] Add financials.
- [ ] Upload form template.
- [ ] Save mapping.
- [ ] Configure simple rule.
- [ ] Run checklist.
- [ ] Generate packet.
- [ ] Capture in-store signature or fake DocuSign.
- [ ] Export deal ZIP.
- [ ] Confirm audit logs exist.

### Negative E2E

- [ ] Dealer cannot access another tenant.
- [ ] Sandbox and Production data are isolated.
- [ ] Missing required checklist item blocks packet generation.
- [ ] Sensitive DL number masked by default.
- [ ] Sensitive DOB masked by default.
- [ ] Unauthorized user cannot reveal sensitive data.
- [ ] Suspended tenant blocked when billing enabled.

### Deployment Runbook

- [ ] Add `/docs/deployment-runbook.md`.
- [ ] Document required Azure resources.
- [ ] Document Azure SQL setup.
- [ ] Document Blob Storage setup.
- [ ] Document Key Vault secrets.
- [ ] Document App Service or Container App setup.
- [ ] Document frontend hosting setup.
- [ ] Document environment variables.
- [ ] Document database migration process.
- [ ] Document monitoring setup.
- [ ] Document backup/restore notes.
- [ ] Document rollback plan.
- [ ] Document smoke test after deployment.

### UAT Script

- [ ] Add `/docs/uat-script.md`.
- [ ] Include dealership registration scenario.
- [ ] Include customer scenario.
- [ ] Include DL scan scenario.
- [ ] Include inventory scenario.
- [ ] Include deal scenario.
- [ ] Include forms scenario.
- [ ] Include packet generation scenario.
- [ ] Include signature scenario.
- [ ] Include export scenario.
- [ ] Include audit review scenario.

### Final Validation

- [ ] All backend tests pass.
- [ ] All frontend tests pass.
- [ ] All E2E tests pass.
- [ ] CI pipeline passes.
- [ ] No orphaned endpoints.
- [ ] No orphaned UI screens.
- [ ] No unreferenced services.
- [ ] No test-only bypass enabled in production.
- [ ] README updated.
- [ ] Deployment docs complete.
- [ ] MVP demo script complete.

### Done

- [ ] MVP beta candidate is ready.

---

# Cross-Cutting Checklist by Module

## Authentication

- [ ] Registration works.
- [ ] Email verification works.
- [ ] Login works.
- [ ] Logout works.
- [ ] Password reset works.
- [ ] Failed login throttling works.
- [ ] Session timeout works.
- [ ] Auth tests pass.

## Tenancy

- [ ] Tenant created correctly.
- [ ] Sandbox created correctly.
- [ ] Production created correctly.
- [ ] Tenant isolation verified.
- [ ] Environment isolation verified.
- [ ] Tenant export works.
- [ ] Sandbox reset works.

## RBAC

- [ ] DealerAdmin permissions verified.
- [ ] PlatformSuperAdmin permissions verified.
- [ ] Future roles modeled.
- [ ] Unauthorized access blocked.
- [ ] Forbidden access returns standard error.

## Audit

- [ ] Login audited.
- [ ] Logout audited.
- [ ] Customer changes audited.
- [ ] Inventory changes audited.
- [ ] Deal changes audited.
- [ ] Document upload/download audited.
- [ ] Sensitive reveal audited.
- [ ] Form changes audited.
- [ ] Packet generation audited.
- [ ] Signature events audited.
- [ ] Impersonation audited.
- [ ] Export audited.

## Customers

- [ ] Create customer.
- [ ] Search customer.
- [ ] Update customer.
- [ ] Delete customer.
- [ ] Restore customer.
- [ ] Multiple owners.
- [ ] DL barcode parsing.
- [ ] Manual correction after parsing.
- [ ] Sensitive masking.
- [ ] Sensitive reveal.

## Documents

- [ ] Upload document.
- [ ] Categorize document.
- [ ] Add notes.
- [ ] Version document.
- [ ] Download document.
- [ ] Delete document.
- [ ] Restore document.
- [ ] Include `ExtractedData` nullable JSON.
- [ ] Confirm no OCR extraction.

## Inventory

- [ ] Create inventory.
- [ ] Edit inventory.
- [ ] Delete/restore inventory.
- [ ] Duplicate active VIN blocked.
- [ ] Sold/archived VIN can be reacquired.
- [ ] VIN decode.
- [ ] Store original decoded data.
- [ ] Store confirmed data.
- [ ] Upload photos.
- [ ] CSV import.
- [ ] Partial success import.
- [ ] Error report download.

## Deals

- [ ] Create deal.
- [ ] Auto deal number.
- [ ] Add owners.
- [ ] Select vehicle.
- [ ] Vehicle moves to PendingSale.
- [ ] Add financials.
- [ ] Cash sale.
- [ ] Financed sale.
- [ ] Add trade-ins.
- [ ] Cancel deal.
- [ ] Deal status transitions.

## Forms

- [ ] Upload PDF.
- [ ] Create version.
- [ ] Upload new version.
- [ ] Copy prior mapping.
- [ ] Save mapping.
- [ ] Activate form.
- [ ] Archive form.
- [ ] Manual visual mapping.
- [ ] Field dictionary works.
- [ ] No AI mapping.

## Rules and Checklist

- [ ] Required forms selected.
- [ ] Optional forms managed.
- [ ] Required forms cannot be removed unless override allowed.
- [ ] Buyer data validation.
- [ ] Vehicle data validation.
- [ ] Finance data validation.
- [ ] Trade-in validation.
- [ ] Template validation.
- [ ] Document validation.
- [ ] Signature validation.
- [ ] Warning override with reason.
- [ ] Override audited.

## PDF and Packets

- [ ] AcroForm fill works.
- [ ] Overlay fill works.
- [ ] Cover sheet generated.
- [ ] Sandbox TEST watermark.
- [ ] Production no TEST watermark.
- [ ] Packet generated in background.
- [ ] Packet status tracked.
- [ ] Generated documents stored.
- [ ] Template version preserved.
- [ ] ZIP export works.

## Signatures

- [ ] In-store signature capture.
- [ ] Signature metadata stored.
- [ ] Signature embedded in PDF.
- [ ] Signed document stored.
- [ ] Paper print workflow.
- [ ] Signed scan upload.
- [ ] DocuSign connect shell.
- [ ] DocuSign fake send.
- [ ] DocuSign webhook updates status.

## Notifications

- [ ] Email abstraction.
- [ ] Fake sender.
- [ ] Branded templates.
- [ ] English templates.
- [ ] Spanish templates.
- [ ] Disable notification type.
- [ ] Notification log.
- [ ] Email failure handling.

## Reports

- [ ] Deal reports.
- [ ] Inventory reports.
- [ ] Audit reports.
- [ ] CSV import error reports.
- [ ] Dashboard widgets.
- [ ] Dashboard layout save.
- [ ] CSV export.
- [ ] PDF export or placeholder.

## Super Admin

- [ ] Tenant list.
- [ ] Tenant detail.
- [ ] Tenant status management.
- [ ] Feature flag management.
- [ ] Diagnostics.
- [ ] Audit search.
- [ ] Rule management.
- [ ] Impersonation start.
- [ ] Impersonation end.
- [ ] Visible impersonation banner.

## Billing

- [ ] Billing Enabled OFF pilot mode.
- [ ] Billing Enabled ON enforcement.
- [ ] Trial state.
- [ ] Active state.
- [ ] PastDue state.
- [ ] Suspended state.
- [ ] Canceled state.
- [ ] Grace period export-only mode.

---

# Pull Request Checklist

Use this for every PR.

## Code Quality

- [ ] Code is small enough to review.
- [ ] No unrelated changes.
- [ ] No commented-out dead code.
- [ ] No duplicated business rules.
- [ ] No secrets committed.
- [ ] No test-only code enabled in production.
- [ ] Error messages are safe.
- [ ] Logs do not expose sensitive values.

## Backend

- [ ] Unit tests added/updated.
- [ ] Integration tests added/updated.
- [ ] API endpoints documented if needed.
- [ ] Tenant isolation tested.
- [ ] Environment isolation tested.
- [ ] Authorization tested.
- [ ] Audit logging added where needed.
- [ ] Migration included if schema changed.
- [ ] Migration reviewed.

## Frontend

- [ ] Component tests added/updated.
- [ ] E2E tests added/updated if workflow changed.
- [ ] API client reused.
- [ ] Auth guard respected.
- [ ] Environment header sent.
- [ ] Loading states handled.
- [ ] Error states handled.
- [ ] Validation messages shown.
- [ ] UI is responsive enough for desktop/tablet.

## Documentation

- [ ] README updated if setup changed.
- [ ] Docs updated if architecture changed.
- [ ] UAT script updated if workflow changed.
- [ ] Deployment docs updated if configuration changed.

## Verification

- [ ] Backend build passes.
- [ ] Backend unit tests pass.
- [ ] Backend integration tests pass.
- [ ] Frontend build passes.
- [ ] Frontend tests pass.
- [ ] Lint passes.
- [ ] E2E tests pass where applicable.
- [ ] Manual smoke test completed.

---

# Definition of Done for MVP

The MVP is done when:

- [ ] A dealership can self-register.
- [ ] Dealer admin can verify email.
- [ ] Dealer admin can log in.
- [ ] Dealer admin can switch between Sandbox and Production.
- [ ] Sandbox and Production are isolated.
- [ ] Dealer admin can configure profile and branding.
- [ ] Dealer admin can create customers.
- [ ] Dealer admin can parse AAMVA driver license barcode.
- [ ] Dealer admin can manually correct parsed DL data.
- [ ] Sensitive DL/DOB data is encrypted and masked.
- [ ] Dealer admin can upload DL images and documents without OCR.
- [ ] Dealer admin can create inventory manually.
- [ ] Dealer admin can decode VIN.
- [ ] Dealer admin can import inventory CSV with partial success.
- [ ] Dealer admin can create a deal.
- [ ] Dealer admin can add owners.
- [ ] Dealer admin can add financial values manually.
- [ ] Dealer admin can add trade-ins.
- [ ] Dealer admin can upload and map PDF forms.
- [ ] Dealer admin can run pre-generation checklist.
- [ ] Dealer admin can override allowed warnings with reason.
- [ ] Dealer admin can generate PDF packet.
- [ ] Sandbox PDFs are watermarked TEST.
- [ ] Dealer admin can capture in-store signature.
- [ ] Dealer admin can use paper print workflow.
- [ ] Dealer admin can use DocuSign shell/fake integration.
- [ ] Dealer admin can export full deal ZIP.
- [ ] Dealer admin can view dashboard and reports.
- [ ] Platform admin can manage tenants.
- [ ] Platform admin can manage feature flags.
- [ ] Platform admin can impersonate with reason and audit.
- [ ] Billing switch works.
- [ ] Tenant export works.
- [ ] Sandbox reset works.
- [ ] Audit logs exist for critical actions.
- [ ] All critical tests pass.
- [ ] Deployment runbook exists.
- [ ] UAT script exists.
- [ ] No V1 out-of-scope features were added accidentally.

---

# Future V2 Placeholder Checklist

Do not implement these in V1. Keep the architecture ready.

## OCR

- [ ] Insurance card OCR.
- [ ] Title OCR.
- [ ] Registration OCR.
- [ ] Payoff letter OCR.
- [ ] Proof of address OCR.
- [ ] Document classification.
- [ ] OCR confidence scores.
- [ ] OCR review/correction UI.

## Cross-Document Validation

- [ ] VIN on title vs inventory VIN.
- [ ] Buyer name on DL vs deal buyer name.
- [ ] Buyer address on DL vs customer profile.
- [ ] Lienholder on payoff letter vs deal financials.
- [ ] Insurance policy holder vs buyer name.
- [ ] Non-blocking warnings.
- [ ] Override reason.
- [ ] Audit override.
- [ ] Use `Document.ExtractedData`.

## Future Integrations

- [ ] Electronic DMV submission.
- [ ] Additional states.
- [ ] Customer portal.
- [ ] SMS notifications.
- [ ] Credit application.
- [ ] Lender integrations.
- [ ] DMS integrations.
- [ ] CRM integrations.
- [ ] Accounting integrations.
- [ ] SSO/MFA.
- [ ] Multi-location dealership groups.

