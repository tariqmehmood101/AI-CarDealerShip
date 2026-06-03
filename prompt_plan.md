# DMV Automation SaaS — TDD Code Generation Prompt Plan

Based on the attached **Car Dealership DMV Automation SaaS SRS v3.0**.

## How to Use This File

Use these prompts one at a time with an LLM coding agent such as GitHub Copilot Agent Mode, Cursor, Claude Code, or Windsurf.

Recommended workflow:

1. Create one GitHub issue per prompt.
2. Create one branch per prompt.
3. Run only one prompt at a time.
4. Require build, lint, unit tests, integration tests, and relevant frontend tests to pass before moving forward.
5. Commit after each completed prompt.
6. Do not skip prompts.
7. Do not allow orphaned code. Every prompt should wire new code into the existing app.

Recommended repo structure:

```text
dmv-automation-saas/
├── backend/
├── frontend/
├── docs/
├── infra/
├── .github/
└── README.md
```

---

# Prompt 1 — Repository and Project Skeleton

```text
You are building the Car Dealership DMV Automation SaaS MVP.

Create the initial monorepo structure with:

- /backend: .NET 8 ASP.NET Core Web API
- /frontend: Next.js + React + TypeScript
- /docs: architecture notes and local setup
- /infra: placeholder for Azure deployment scripts
- /tests or backend test projects as appropriate

Backend requirements:
- Create a .NET solution.
- Add API project.
- Add test projects for unit and integration tests.
- Add a /src structure that supports a modular monolith.
- Add a health endpoint: GET /health returning 200 and app metadata.

Frontend requirements:
- Create a Next.js app with TypeScript.
- Add a simple landing page that calls or links to API health configuration.
- Add basic linting and test setup.

Testing:
- Add a backend test verifying /health returns 200.
- Add a frontend smoke test verifying the landing page renders.
- Add README instructions for running backend, frontend, and tests locally.

Constraints:
- Do not implement business modules yet.
- Keep the solution minimal but production-friendly.
- Ensure all generated code builds and tests pass.
```

---

# Prompt 2 — Backend Shared Architecture

```text
Add backend shared architecture for the modular monolith.

Implement:

- Common result type for success/failure.
- Standard API error response shape.
- Correlation ID middleware.
- Request logging middleware.
- Global exception handling middleware.
- Base entity abstractions:
  - Id
  - CreatedAtUtc
  - UpdatedAtUtc
  - CreatedByUserId
  - UpdatedByUserId
  - IsDeleted
  - DeletedAtUtc
- Domain enum support for TenantEnvironment: Sandbox, Production.

Testing:
- Unit test the Result type.
- Unit test error response mapping.
- Integration test that unhandled exceptions return the standard error shape.
- Integration test that correlation ID is returned in response headers.

Integration:
- Wire middleware into the API pipeline.
- Do not leave unused middleware or unused abstractions.
```

---

# Prompt 3 — Persistence Foundation

```text
Add EF Core persistence foundation for Azure SQL / SQL Server.

Implement:

- ApplicationDbContext.
- Design-time DbContext factory.
- Base entity configuration.
- Migration support.
- Soft-delete query filter support.
- UpdatedAtUtc/CreatedAtUtc save-change interceptor.
- Local development connection string configuration.
- Testcontainers-based SQL Server integration test setup, if available in the repo environment. If not, use a clear fallback and document the tradeoff.

Create initial entities:
- Tenant
- TenantEnvironmentRecord
- TenantSettings
- AppUser
- Role
- Permission
- UserRole

Testing:
- Integration test that migrations apply cleanly.
- Integration test that CreatedAtUtc and UpdatedAtUtc are set.
- Integration test that soft-deleted records are excluded by default.
- Unit test entity validation where applicable.

Constraints:
- Every tenant-scoped entity must have TenantId.
- Every environment-scoped entity must have Environment: Sandbox or Production where required.
- Do not implement authentication yet.
```

---

# Prompt 4 — Tenant and Environment Creation

```text
Implement tenant creation foundation.

Business requirements:
- One tenant represents one dealership location.
- Every new tenant must receive both Sandbox and Production environments.
- Sandbox and Production data must be isolated.
- Tenant has status: Trial, Active, Suspended, Canceled.
- TenantSettings stores dealer profile placeholders, branding placeholders, session timeout, feature flags, and deal numbering settings.

Backend:
- Add TenantService.
- Add endpoint POST /api/tenants/bootstrap for internal/dev tenant creation only.
- Add endpoint GET /api/tenants/current to return current tenant context once auth is later added.
- For now, protect bootstrap behind development environment only.

Testing:
- Unit test tenant creation creates exactly two environments.
- Integration test TenantSettings is created.
- Integration test duplicate tenant legal name or slug is handled.
- Integration test sandbox and production environment records are distinct.

Integration:
- Wire DbContext and endpoints.
- Add seed data only for local development.
```

---

# Prompt 5 — RBAC Foundation

```text
Implement role-based access control foundation.

Roles required now:
- DealerAdmin
- PlatformSuperAdmin

Future roles should be modeled but not active:
- Salesperson
- FinanceManager
- TitleClerk
- ReadOnlyAuditor

Implement:
- Permission catalog.
- Role-permission mapping.
- Authorization policy names.
- CurrentUser abstraction.
- CurrentTenant abstraction placeholder.

Testing:
- Unit test permission catalog has no duplicate permission keys.
- Unit test DealerAdmin receives tenant-scoped permissions.
- Unit test PlatformSuperAdmin receives platform permissions.
- Integration test unauthorized request receives standard 401/403 shape.

Constraints:
- Do not build full login yet.
- Do not hard-code tenant access checks inside controllers; create reusable authorization/tenant services.
```

---

# Prompt 6 — Registration and Email Verification

```text
Implement dealership registration and email verification.

Business requirements:
- Dealer self-service registration creates tenant, tenant settings, sandbox, production, and initial DealerAdmin user.
- New accounts require email verification before production use.
- Password hashing must use a secure adaptive algorithm.
- Email sending should use an interface, with a local fake provider for tests/dev.

Endpoints:
- POST /api/auth/register-dealership
- POST /api/auth/verify-email
- POST /api/auth/resend-verification

Data:
- Store verification token hash, expiration, consumed timestamp.
- Do not store raw tokens.

Testing:
- Unit test password hashing and verification.
- Unit test token expiration and consumed token behavior.
- Integration test registration creates tenant and user.
- Integration test unverified user cannot access protected production endpoint.
- Integration test verification enables login eligibility.

Frontend:
- Add simple registration page.
- Add verify email page that can accept token input manually for local dev.

Constraints:
- No real email provider yet.
- Use fake email sender and expose last email only in development/testing.
```

---

# Prompt 7 — Login, Logout, Session Timeout

```text
Implement authentication login/logout with configurable inactivity timeout.

Requirements:
- Email/password login.
- Account throttling or lockout after repeated failed login attempts.
- Default inactivity timeout: 15 minutes.
- Tenant admin can later configure within platform-approved limits.
- Logout invalidates the session/token.
- No SSO or MFA in V1.

Choose a secure auth approach suitable for this app:
- Prefer secure HTTP-only cookie session or JWT with refresh token rotation.
- Document the choice in /docs/auth.md.

Endpoints:
- POST /api/auth/login
- POST /api/auth/logout
- POST /api/auth/forgot-password
- POST /api/auth/reset-password
- GET /api/auth/me

Testing:
- Integration test valid login.
- Integration test unverified user blocked.
- Integration test failed login count increments.
- Integration test lockout/throttling behavior.
- Integration test logout prevents further access.
- Unit test timeout calculation.

Frontend:
- Add login page.
- Add authenticated layout shell.
- Add logout button.
- Display current user and tenant name after login.
```

---

# Prompt 8 — Tenant Context Middleware

```text
Implement tenant and environment context enforcement.

Requirements:
- Every API request must validate tenant context and user permission.
- Tenant data must be isolated.
- Sandbox and Production must be isolated.
- UI must clearly know current environment.

Backend:
- Add middleware that resolves TenantId and TenantEnvironment from authenticated user/session and request header or route.
- Support header: X-Tenant-Environment with Sandbox or Production.
- Default to Sandbox for local development only.
- Add safeguards so DealerAdmin cannot access another tenant.
- PlatformSuperAdmin may access tenant data only through explicit support/impersonation flow later.

Testing:
- Integration test DealerAdmin can access own tenant.
- Integration test DealerAdmin cannot access another tenant.
- Integration test Sandbox data is not returned in Production queries.
- Integration test missing/invalid environment returns standard error.
- Add tests proving tenant filter is applied to sample tenant-scoped entity.

Integration:
- Wire CurrentTenant and CurrentEnvironment into DbContext query behavior.
- Do not leave bypass paths.
```

---

# Prompt 9 — Audit Logging Framework

```text
Implement audit logging framework.

Requirements:
Audit critical actions including:
- Authentication
- Customer create/update/delete/restore
- Sensitive data reveal
- Inventory changes
- Deal changes
- Document upload/download/delete
- Forms upload/map/activate
- Packet generation
- Signature events
- Super admin impersonation later

Backend:
- Add AuditLog entity.
- Add AuditService.
- Add audit action enum or strongly typed keys.
- Capture tenant, environment, user, entity type, entity id, action, timestamp, IP, user agent, metadata JSON.
- Add GET /api/audit-logs with filters.

Testing:
- Unit test audit event creation.
- Integration test login writes audit log.
- Integration test tenant isolation on audit log query.
- Integration test metadata JSON is stored and retrieved.
- Integration test failed permission access can be audited where appropriate.

Integration:
- Wire audit service into auth registration/login/logout flows.
```

---

# Prompt 10 — Tenant Settings, Branding, Feature Flags

```text
Implement dealership settings.

Requirements:
Dealer profile fields:
- Legal name
- DBA
- Dealer license number
- Address
- Mailing address
- County
- Phone
- Email
- Website
- Authorized signer
- Signer title
- Logo reference

Branding:
- Logo
- Brand colors
- Email sender name
- Email footer

Feature flags:
- Billing
- DocuSign
- In-store signatures
- Future AI
- DMV submission
- Beta features

Endpoints:
- GET /api/tenant/settings
- PUT /api/tenant/settings
- GET /api/tenant/feature-flags
- PUT /api/tenant/feature-flags

Testing:
- Integration test DealerAdmin can update own tenant settings.
- Integration test tenant settings cannot be read across tenant.
- Integration test invalid session timeout is rejected.
- Unit test feature flag defaults.
- Integration test audit log is written for settings changes.

Frontend:
- Add Settings page.
- Add Dealer Profile form.
- Add Branding form with non-functional logo placeholder if blob storage is not ready yet.
- Add Feature Flags read-only or editable based on role.
```

---

# Prompt 11 — Frontend App Shell

```text
Build the authenticated frontend shell.

Requirements:
- Login/register pages exist from previous steps.
- Add protected app layout.
- Add left navigation:
  - Dashboard
  - Customers
  - Inventory
  - Deals
  - Documents
  - Forms
  - Reports
  - Settings
  - Super Admin, visible only for PlatformSuperAdmin
- Add top bar:
  - Tenant name
  - Current environment selector: Sandbox / Production
  - Clear environment label
  - User menu
- Persist selected environment safely in local storage or user preference.
- Send X-Tenant-Environment header on API requests.

Testing:
- Component test nav renders expected items for DealerAdmin.
- Component test Super Admin hidden for DealerAdmin.
- Component test environment selector changes header used by API client.
- E2E smoke test login lands on dashboard.

Constraints:
- Do not create fake disconnected pages. Every nav item can have a placeholder route, but it must use the shared app shell and auth guard.
```

---

# Prompt 12 — Customer Backend

```text
Implement customer and owner backend.

Requirements:
- Customer profile reusable within same tenant only.
- Support primary buyer plus additional owners/buyers.
- Search by name, phone, DL number, email, and address.
- Phone number required for each buyer/owner.
- Email required later when DocuSign is ON; for now validate through deal/signature workflow, not customer creation.
- Preferred language: English or Spanish.
- Soft delete and recovery.
- Audit logging.

Entities:
- Customer
- CustomerOwner
- DriverLicense

Endpoints:
- GET /api/customers
- POST /api/customers
- GET /api/customers/{id}
- PUT /api/customers/{id}
- DELETE /api/customers/{id}
- POST /api/customers/{id}/restore

Testing:
- Unit test validation rules.
- Integration test create customer with primary owner.
- Integration test search by phone/email/name.
- Integration test soft delete hides customer.
- Integration test restore works.
- Integration test tenant isolation.
- Integration test audit logs for create/update/delete/restore.

Do not implement AAMVA parsing yet.
```

---

# Prompt 13 — AAMVA Driver License Parser

```text
Implement AAMVA driver license barcode parsing as a pure backend service.

Requirements:
- Scanner behaves like keyboard input, so parser receives raw PDF417 text payload.
- Parse common AAMVA fields into customer/owner/driver license DTO:
  - First name
  - Middle name
  - Last name
  - Address
  - City
  - State
  - ZIP
  - DOB
  - DL number
  - DL expiration
- Display parsed fields for admin review before saving.
- Allow correction before save.
- Store scan method, scan timestamp, and user when saved.
- Do not perform OCR from DL image.

Endpoints:
- POST /api/driver-license/parse
- POST /api/customers/from-driver-license

Testing:
- Unit tests using sample AAMVA payloads.
- Unit test malformed payload returns validation errors, not crash.
- Unit test date parsing.
- Integration test parsed payload can create customer after admin confirmation.
- Integration test raw scan payload is not stored unless policy flag allows it.
- Integration test audit log is written for DL scan save.

Frontend:
- Add Customer DL Scan page/section.
- Add large scan input field.
- Show parsed review form before save.
```

---

# Prompt 14 — Sensitive Data Encryption and Masking

```text
Implement encryption and masking for sensitive customer fields.

Sensitive fields:
- DL number
- DOB
- Any future sensitive field should be easy to add.

Requirements:
- Sensitive fields encrypted at rest in database.
- Sensitive fields masked by default in API responses.
- Reveal requires explicit endpoint/action.
- Reveal must be audit logged.
- UI must show masked values by default.

Backend:
- Add encryption service abstraction.
- Use development key from configuration for local only.
- Add value conversion or explicit encrypt/decrypt handling.
- Add masked DTOs.
- Add endpoint POST /api/customers/{customerId}/owners/{ownerId}/reveal-sensitive.

Testing:
- Unit test masking format.
- Unit test encryption round trip.
- Integration test raw database value is not plaintext.
- Integration test normal customer GET returns masked DL/DOB.
- Integration test reveal returns unmasked values only to authorized user.
- Integration test reveal writes audit log.

Frontend:
- Show masked DL/DOB.
- Add Reveal button with confirmation dialog.
- After reveal, show value temporarily with a visible warning.
```

---

# Prompt 15 — Blob Storage and Document Foundation

```text
Implement document storage foundation.

Requirements:
- Store documents in secure cloud storage abstraction.
- Support local development storage.
- Support categories:
  - DriverLicense
  - Insurance
  - Title
  - Registration
  - LienholderFinancing
  - PayoffLetter
  - ProofOfAddress
  - BillOfSale
  - OdometerDisclosure
  - PowerOfAttorney
  - DMVForms
  - SignedDocuments
  - Other
- Support document version history with one current version.
- Documents are stored without OCR extraction in V1.
- Include Document.ExtractedData JSON column from V1, nullable, reserved for V2.
- Soft delete/recover.
- Audit upload/download/delete/restore.

Entities:
- Document
- DocumentVersion

Endpoints:
- POST /api/documents/upload
- GET /api/documents/{id}
- GET /api/documents/{id}/download
- POST /api/documents/{id}/versions
- DELETE /api/documents/{id}
- POST /api/documents/{id}/restore

Testing:
- Unit test category validation.
- Integration test upload creates document and version.
- Integration test second upload creates new version and marks current.
- Integration test ExtractedData defaults to null.
- Integration test tenant/environment isolation.
- Integration test download audit log.
- Integration test soft delete/restore.

Frontend:
- Add Documents page with upload form, category, notes, list, download.
```

---

# Prompt 16 — Inventory Backend

```text
Implement inventory backend.

Requirements:
- Inventory fields:
  - VIN
  - Year
  - Make
  - Model
  - Trim
  - Body type
  - Mileage
  - Color
  - Stock number
  - Purchase cost
  - Reconditioning cost
  - Asking price
  - Sale price when linked to deal later
  - Status: Available, PendingSale, Sold, Archived
- Prevent duplicate VINs in active inventory within same tenant.
- Duplicate blocking applies only to active inventory; sold/archived do not block reacquisition.
- Soft delete/recover.
- Audit status changes and edits.

Endpoints:
- GET /api/inventory
- POST /api/inventory
- GET /api/inventory/{id}
- PUT /api/inventory/{id}
- DELETE /api/inventory/{id}
- POST /api/inventory/{id}/restore

Testing:
- Unit test VIN validation.
- Integration test create inventory.
- Integration test duplicate active VIN rejected.
- Integration test sold/archived VIN can be re-added.
- Integration test status update audited.
- Integration test tenant isolation.
- Integration test soft delete/restore.

Do not implement VIN decode yet.
```

---

# Prompt 17 — VIN Decode Adapter

```text
Implement VIN decode support.

Requirements:
- Admin can enter VIN manually or scan VIN barcode as text.
- Validate VIN format before decode.
- Call VIN decode API through an interface.
- Use NHTSA or equivalent adapter, but tests must not depend on live external API.
- Store original decoded data and final confirmed vehicle data.
- Admin can edit decoded data before final confirmation.

Backend:
- Add IVinDecoder interface.
- Add NhtsaVinDecoder implementation behind configuration.
- Add FakeVinDecoder for tests/local.
- Add endpoint POST /api/vin/decode.
- Add inventory endpoint POST /api/inventory/from-vin that decodes, returns review DTO, and creates inventory only after confirmation.

Testing:
- Unit test VIN checksum/format validation where practical.
- Unit test invalid VIN rejected before decode.
- Unit test fake decoder maps response.
- Integration test decode review does not create inventory until confirmed.
- Integration test confirmed decoded vehicle stores OriginalDecodedData JSON and final fields.
- Integration test audit log for inventory created from VIN.

Frontend:
- Add VIN decode flow to Inventory create page.
- User enters/scans VIN, reviews decoded fields, edits, saves.
```

---

# Prompt 18 — Inventory Photos and CSV Import

```text
Implement inventory photos and CSV import.

Requirements:
- Vehicle photos upload/remove/reorder.
- CSV import supports:
  - VIN required
  - Optional stock number, mileage, color, asking price, purchase cost, status
  - Validate each row independently
  - Invalid rows rejected while valid rows continue
  - Import summary: total rows, imported rows, rejected rows, reason by row
  - Error report download
  - Imported VINs decoded automatically where possible
- Large imports should be background-process ready.

Backend:
- Add InventoryPhoto entity or document relationship.
- Add CSV import parser service.
- Add InventoryImportJob entity/status or reuse BackgroundJob foundation if already present.
- Add endpoints:
  - POST /api/inventory/{id}/photos
  - DELETE /api/inventory/{id}/photos/{photoId}
  - PUT /api/inventory/{id}/photos/reorder
  - POST /api/inventory/import-csv
  - GET /api/inventory/imports/{id}
  - GET /api/inventory/imports/{id}/error-report

Testing:
- Unit test CSV parser.
- Unit test row-level validation.
- Integration test partial success import.
- Integration test duplicate active VIN row rejected.
- Integration test error report generated.
- Integration test photo upload uses document/blob abstraction.
- Integration test audit logs.

Frontend:
- Add inventory import page.
- Add import results screen.
- Add photo upload section on inventory detail.
```

---

# Prompt 19 — Inventory Frontend Completion

```text
Complete the inventory frontend vertical slice.

Requirements:
- Inventory list with filters:
  - Status
  - VIN
  - Stock number
  - Make/model
- Inventory detail page.
- Create/edit page.
- VIN decode integrated.
- Photos section integrated.
- CSV import linked from inventory list.
- Status badges:
  - Available
  - Pending Sale
  - Sold
  - Archived

Testing:
- Component test inventory list renders rows.
- Component test status filter changes query.
- Component test create form validates required VIN.
- Component test VIN decode review flow.
- E2E test create inventory manually.
- E2E test decode VIN then save inventory using fake backend/test data.

Constraints:
- Use shared API client and auth/environment header.
- Do not duplicate API logic inside components.
```

---

# Prompt 20 — Deal Backend Foundation

```text
Implement deal backend foundation.

Requirements:
- Deal statuses:
  - Draft
  - ReviewRequired
  - ReadyForSignatureOrPrint
  - SentToDocuSign
  - PartiallySigned
  - SignedCompleted
  - PrintedForWetSignature
  - Archived
  - CanceledVoided
- Auto-generate deal number using tenant-configured format.
- Deal supports one dealership inventory vehicle in MVP.
- Creating a deal from Available vehicle sets inventory status to PendingSale.
- Completing a deal later will set vehicle to Sold.
- Canceling/voiding later returns vehicle to Available unless admin chooses otherwise.

Entities:
- Deal
- DealOwner
- DealFinancials

Endpoints:
- GET /api/deals
- POST /api/deals
- GET /api/deals/{id}
- PUT /api/deals/{id}
- POST /api/deals/{id}/cancel

Testing:
- Unit test deal number generation.
- Unit test valid/invalid status transitions.
- Integration test create deal from available vehicle.
- Integration test inventory becomes PendingSale.
- Integration test cannot create deal from Sold vehicle.
- Integration test tenant/environment isolation.
- Integration test audit logs.

Do not implement trade-ins or forms yet.
```

---

# Prompt 21 — Deal Owners and Financials

```text
Expand deal workflow with owners and manual financials.

Requirements:
- Deal supports multiple buyers/owners.
- Deal owner should snapshot customer owner data at time of deal association.
- Sale type: Cash or Financed.
- Financed deal stores:
  - Lender/lienholder name
  - Address
  - Optional account/reference number
  - Amount financed
  - Down payment
  - Lien recording info
- All taxes/fees are manual in V1.
- Financial fields:
  - Purchase price
  - Dealer fee
  - Title fee
  - Registration fee
  - Plate fee
  - Sales tax
  - Electronic filing fee
  - Lien recording fee
  - Trade-in allowance
  - Trade-in payoff
  - Down payment
  - Amount financed
  - Balance due

Endpoints:
- POST /api/deals/{id}/owners
- DELETE /api/deals/{id}/owners/{ownerId}
- PUT /api/deals/{id}/financials
- PUT /api/deals/{id}/sale-type

Testing:
- Unit test owner snapshot behavior.
- Unit test financed validation.
- Unit test cash sale does not require lienholder.
- Integration test add/remove owner.
- Integration test update financials.
- Integration test audit logs for owner and financial changes.
```

---

# Prompt 22 — Trade-In Backend

```text
Implement trade-in backend.

Requirements:
- Deal may include zero or more trade-in vehicles.
- Each trade-in supports:
  - VIN
  - VIN decode
  - Mileage
  - Year
  - Make
  - Model
  - Trim
  - Trade-in value
  - Payoff amount
  - Lienholder
  - Title status
- Trade-in documents can be uploaded and linked.
- Trade-in values feed deal financial summary, but totals remain manually confirmed.
- When deal closes later, admin decides disposition:
  - Add to Inventory Available
  - Add to Inventory Archived
  - Wholesale
  - Hold for Review
  - Do Nothing
- Trade-in must not automatically become inventory without admin decision.

Endpoints:
- GET /api/deals/{id}/trade-ins
- POST /api/deals/{id}/trade-ins
- PUT /api/deals/{id}/trade-ins/{tradeInId}
- DELETE /api/deals/{id}/trade-ins/{tradeInId}
- POST /api/deals/{id}/trade-ins/{tradeInId}/documents

Testing:
- Unit test trade-in validation.
- Integration test add trade-in with VIN decode.
- Integration test trade-in document upload links correctly.
- Integration test trade-in does not create inventory automatically.
- Integration test audit logs.
```

---

# Prompt 23 — Deal Frontend Wizard

```text
Build the deal frontend wizard.

Wizard steps:
1. Create/select customer
2. Add buyers/owners
3. Select inventory vehicle
4. Add trade-ins if applicable
5. Enter financial values manually
6. Select cash or financed
7. Review draft deal summary

Requirements:
- Use backend APIs from previous prompts.
- Show deal status.
- Show selected vehicle and PendingSale behavior after deal creation.
- Show owners and financial summary.
- Allow save-and-continue.
- Avoid large all-at-once form submission; each step should persist incrementally.

Testing:
- Component test wizard step navigation.
- Component test financed sale shows lienholder fields.
- Component test cash sale hides lienholder fields.
- E2E test create draft deal with one buyer and one vehicle.
- E2E test add financed financials.
- E2E test add trade-in.

Constraints:
- No PDF generation yet.
- No signatures yet.
- All pages must use authenticated shell and environment selector.
```

---

# Prompt 24 — Form Template Backend

```text
Implement form template backend.

Requirements:
- Dealer Admin uploads one PDF form at a time.
- Support template versioning.
- New deals use active version.
- Existing deals later preserve version used at generation time.
- Deleted forms are archived, not permanently removed.
- Form supports workflow flags:
  - Print
  - DocuSign
  - In-store signature
- No AI-assisted mapping in V1.

Entities:
- FormTemplate
- FormVersion
- FormFieldMapping

Endpoints:
- GET /api/forms
- POST /api/forms/upload
- GET /api/forms/{id}
- POST /api/forms/{id}/versions
- POST /api/forms/{id}/activate
- POST /api/forms/{id}/archive

Testing:
- Integration test upload creates template and version.
- Integration test new version copies previous mapping as draft if available.
- Integration test activate requires at least one reviewed version.
- Integration test archive hides template from active list.
- Integration test tenant/environment isolation.
- Integration test audit logs.
```

---

# Prompt 25 — Form Field Dictionary and Mapping Model

```text
Implement form data dictionary and mapping model.

Requirements:
Field groups:
- Buyer/Owner
- Dealer
- Vehicle
- Deal
- Finance
- Trade-In
- Fees
- Signatures
- System

Mapping field types:
- Text
- Date
- Checkbox
- Radio
- Dropdown
- Initials
- Signature
- Dealer-only
- Customer-only

Mapping supports:
- Existing AcroForm field name
- Overlay coordinates for non-fillable PDF:
  - Page number
  - X
  - Y
  - Width
  - Height
  - Font size
  - Alignment
- Owner index and trade-in index when needed
- Required flag
- Conditional visibility metadata

Endpoints:
- GET /api/forms/field-dictionary
- GET /api/forms/{id}/versions/{versionId}/mapping
- PUT /api/forms/{id}/versions/{versionId}/mapping
- POST /api/forms/{id}/versions/{versionId}/preview-test-data

Testing:
- Unit test field dictionary has stable keys.
- Unit test mapping validation.
- Integration test save mapping.
- Integration test invalid system field rejected.
- Integration test owner-indexed field is accepted.
- Integration test audit log for mapping change.
```

---

# Prompt 26 — Forms Frontend and Visual Mapper

```text
Build forms frontend and visual mapper MVP.

Screens:
- Form template list
- Upload form
- Form version detail
- Mapping editor
- Preview/test data screen

Visual mapper MVP:
- Render PDF pages or page placeholders if PDF rendering library is not yet configured.
- Allow admin to add overlay fields.
- Allow admin to select system field from dictionary.
- Allow admin to enter coordinates manually as fallback.
- Allow mapping existing AcroForm field names manually.
- Allow save mapping.
- Show whether form supports Print, DocuSign, In-store signature.

Testing:
- Component test field dictionary loads.
- Component test mapping row can be added.
- Component test mapping validation errors.
- E2E test upload form and save one mapping.
- E2E test activate form after mapping.

Constraints:
- No AI mapping.
- Do not generate real PDFs yet; use preview placeholder wired to backend.
```

---

# Prompt 27 — Rule Engine and Pre-Generation Checklist

```text
Implement rules and pre-generation checklist.

Requirements:
- Automatically select required forms based on:
  - Tenant
  - State
  - Transaction type
  - Buyer state
  - Cash/financed flag
  - Trade-in presence
  - Lienholder
  - Multiple owners
  - Signature mode
- Admin may add/remove optional forms.
- Admin cannot remove required forms unless rule allows override with reason.
- Support conditional field visibility and conditional required fields.
- Support conditional signatures.
- MVP rules are configured behind the scenes by platform support/developers.
- Rules are tenant-specific and state-aware.
- Rules are auditable/versioned.

Entities:
- RuleConfig
- ChecklistResult or generated checklist DTO
- ChecklistOverride

Endpoints:
- GET /api/deals/{id}/checklist
- POST /api/deals/{id}/checklist/overrides
- GET /api/deals/{id}/forms/selection
- PUT /api/deals/{id}/forms/selection

Testing:
- Unit test rule selection for Florida cash deal.
- Unit test financed deal requires lienholder fields.
- Unit test trade-in adds trade-in related form when configured.
- Unit test required form cannot be removed without allowed override.
- Integration test checklist blocks missing buyer phone.
- Integration test warning override requires reason and writes audit log.
- Integration test missing active template blocks generation.
```

---

# Prompt 28 — PDF Generation Service

```text
Implement PDF generation service.

Requirements:
- Generate completed PDFs from mapped form templates using confirmed deal data.
- Support AcroForm fill for fillable PDFs.
- Support text overlay for non-fillable PDFs.
- Generate deal packet cover sheet.
- Sandbox-generated PDFs must be watermarked TEST.
- Preserve generated PDF version and form template version used.
- Admin can later download individual PDFs, combined packet where feasible, and ZIP deal jacket.

Backend:
- Add IPdfGenerationService.
- Add PDF library implementation.
- Add test fixtures with small sample PDFs:
  - One fillable sample
  - One non-fillable overlay sample
- Add generated document storage through document/blob abstraction.

Testing:
- Unit test deal data flattening into form fields.
- Golden-file or metadata-based test for AcroForm fill.
- Golden-file or metadata-based test for overlay generation.
- Unit test cover sheet data includes deal number, buyers, vehicle, forms, warnings.
- Integration test sandbox PDF gets TEST watermark.
- Integration test generated PDF record stores form version id.

Do not implement background job orchestration yet.
```

---

# Prompt 29 — Packet Generation Background Job

```text
Implement packet generation as a background job.

Requirements:
- Packet generation may be long-running and must not block UI.
- Creates packet with selected forms in configured order.
- Creates cover sheet.
- Generates individual PDFs.
- Creates packet record.
- Stores status:
  - Queued
  - Running
  - Completed
  - Failed
- Supports retry visibility.
- Stores errors for support diagnostics.

Entities:
- BackgroundJob
- Packet
- PacketDocument

Endpoints:
- POST /api/deals/{id}/generate-packet
- GET /api/deals/{id}/packet
- GET /api/background-jobs/{id}

Testing:
- Unit test job status transitions.
- Integration test cannot generate packet when checklist has blocking errors.
- Integration test valid deal queues packet job.
- Integration test worker generates packet documents.
- Integration test failure stores error details.
- Integration test audit log for packet generation.

Frontend:
- Add packet step to deal detail.
- Show checklist result.
- Add Generate Packet button.
- Show job progress/status.
- Show generated documents list when completed.
```

---

# Prompt 30 — ZIP Deal Jacket Export

```text
Implement complete deal ZIP export.

Requirements:
ZIP should include:
- Packet cover sheet
- Generated forms
- Signed forms when available
- Uploaded supporting documents related to deal
- Summary JSON or PDF
- Open warnings and override reasons
- Template versions used
- Audit-friendly metadata

Backend:
- Add export service.
- Add endpoint POST /api/deals/{id}/export-zip.
- Run as background job if export is large.
- Add endpoint GET /api/deals/{id}/exports/{exportId}/download.

Testing:
- Unit test ZIP manifest creation.
- Integration test export includes generated packet documents.
- Integration test export includes uploaded deal documents.
- Integration test export includes metadata summary.
- Integration test tenant isolation.
- Integration test export action writes audit log.

Frontend:
- Add Export Deal Jacket button on deal detail.
- Show export progress and download link.
```

---

# Prompt 31 — In-Store Signature Workflow

```text
Implement in-store signature capture.

Requirements:
- Template defines signer roles:
  - Buyer
  - Additional owner
  - Dealer representative
  - Lienholder where needed
- In-store signature capture records:
  - Signer
  - Timestamp
  - User
  - Device/browser info where available
- Signature events are audit logged.
- Captured signature can be embedded into mapped PDF signature fields.
- Completed signed PDFs are stored as Signed Documents.

Backend:
- Add SignatureEnvelope, SignatureSigner, SignatureEvent entities if not already present.
- Add endpoint POST /api/deals/{id}/capture-signature.
- Add endpoint GET /api/deals/{id}/signatures.
- Update PDF generation or post-processing to apply captured signatures.

Testing:
- Unit test signer role validation.
- Integration test capture signature stores event.
- Integration test signed PDF stored as SignedDocuments category.
- Integration test audit log for signature capture.
- Integration test unauthorized user cannot capture for another tenant.

Frontend:
- Add in-store signature screen.
- Add simple canvas signature capture.
- Add signer selection.
- Show captured signature status.
```

---

# Prompt 32 — DocuSign OAuth and Remote Signature Shell

```text
Implement DocuSign integration shell.

Requirements:
- Dealer connects own DocuSign account via OAuth.
- Dealer can disconnect/reconnect.
- Token refresh supported.
- Status health check.
- System can send documents and track statuses:
  - Sent
  - Viewed
  - Signed
  - Declined
  - Completed
- Completed signed PDFs are stored as Signed Documents.
- DocuSign emails require signer email addresses.

Backend:
- Add IDocuSignClient interface.
- Add fake DocuSign client for tests/dev.
- Add OAuth connect callback endpoint.
- Add endpoints:
  - GET /api/integrations/docusign/status
  - POST /api/integrations/docusign/disconnect
  - POST /api/deals/{id}/send-docusign
  - POST /api/webhooks/docusign
- Store tokens securely through secret abstraction or encrypted database field for local dev.

Testing:
- Unit test cannot send when DocuSign feature flag is off.
- Unit test cannot send when signer email missing.
- Integration test connect status.
- Integration test fake send creates SignatureEnvelope.
- Integration test webhook updates status.
- Integration test completed document stored.
- Integration test audit logs.

Frontend:
- Add DocuSign settings connection status.
- Add Send to DocuSign button on deal packet/signature step.
```

---

# Prompt 33 — Paper Print Workflow

```text
Implement paper print / wet signature workflow.

Requirements:
- Admin can download or print forms for wet signature.
- Deal status can become PrintedForWetSignature.
- Signed scans can be uploaded later as Signed Documents.
- Print workflow must be audited.

Backend:
- Add endpoint POST /api/deals/{id}/mark-printed.
- Add endpoint POST /api/deals/{id}/signed-documents/upload.
- Ensure uploaded signed scans use Document category SignedDocuments.
- Update deal status rules.

Testing:
- Unit test valid transition to PrintedForWetSignature.
- Integration test mark printed writes audit log.
- Integration test signed document upload links to deal.
- Integration test signed document visible in deal packet/export.
- Integration test tenant isolation.

Frontend:
- Add Print Packet button.
- Add Mark Printed action.
- Add Upload Signed Scan action.
```

---

# Prompt 34 — Email Notifications

```text
Implement email notification framework.

Requirements:
- MVP sends email only; SMS is out of scope.
- Notifications support dealership branding.
- Customer preferred language supports English and Spanish.
- Supported notifications:
  - DocuSign sent
  - Reminder
  - Missing document request
  - Deal status update
  - Completion notice
- Email templates editable where allowed.
- Sent notifications logged and auditable.
- Dealer can disable specific notification types.

Backend:
- Add Notification entity.
- Add EmailTemplate entity or settings JSON.
- Add IEmailSender interface.
- Add fake local/test sender.
- Add notification service.
- Add endpoints for notification preferences/templates.

Testing:
- Unit test template rendering.
- Unit test disabled notification is not sent.
- Unit test Spanish template selection.
- Integration test notification record created.
- Integration test email send failure records error.
- Integration test audit log for template change.

Frontend:
- Add notification settings page under Settings.
```

---

# Prompt 35 — Reports and Dashboard

```text
Implement reporting and dashboard.

Requirements:
Reports:
- Deals by status
- Completed deals
- Pending signatures
- Vehicles sold
- Inventory status
- Trade-in activity
- Audit log reports
- CSV import errors

Dashboard widgets:
- Deals in progress
- Review required
- Pending signatures
- Missing documents
- Available vehicles
- Pending sale
- Recent activity
- Import errors

Exports:
- CSV export
- PDF export placeholder or implemented if PDF infrastructure supports it

Backend:
- Add reporting query services.
- Add endpoints:
  - GET /api/reports/deals
  - GET /api/reports/inventory
  - GET /api/reports/audit
  - POST /api/reports/export
  - GET /api/dashboard/widgets
  - PUT /api/dashboard/layout

Testing:
- Integration test report filters by tenant/environment.
- Integration test deal status counts.
- Integration test inventory status counts.
- Integration test CSV export.
- Integration test dashboard layout saved per user.
- Integration test audit report respects permissions.

Frontend:
- Add dashboard widgets.
- Add reports pages with filters and export buttons.
```

---

# Prompt 36 — Platform Super Admin

```text
Implement Platform Super Admin portal.

Requirements:
Platform Super Admin can:
- View tenants
- Create/disable tenants
- View tenant health
- Manage trial/subscription status
- Manage feature flags
- Manage behind-the-scenes rules by tenant/state
- View diagnostics
- Search audit logs
- Impersonate tenant admin with mandatory reason
- See visible impersonation banner
- Impersonation start/end must be audit logged

Backend:
- Add super admin authorization policy.
- Add endpoints under /api/platform:
  - GET /tenants
  - GET /tenants/{id}
  - PUT /tenants/{id}/status
  - PUT /tenants/{id}/feature-flags
  - GET /diagnostics
  - POST /impersonation/start
  - POST /impersonation/end

Testing:
- Integration test DealerAdmin cannot access platform endpoints.
- Integration test PlatformSuperAdmin can list tenants.
- Integration test impersonation requires reason.
- Integration test impersonation writes start/end audit logs.
- Integration test impersonation context shows visible flag.
- Integration test impersonation cannot bypass audit requirements.

Frontend:
- Add Super Admin navigation for PlatformSuperAdmin only.
- Add tenant list.
- Add impersonation start/end UI with reason input.
- Add visible impersonation banner.
```

---

# Prompt 37 — Billing Switch and Subscription Access

```text
Implement billing switch and subscription access control.

Requirements:
- Billing controlled by platform-level Billing Enabled switch.
- When Billing Enabled is OFF, system operates as private/internal pilot without subscription enforcement.
- When Billing Enabled is ON, tenants use flat monthly subscription.
- Subscription states:
  - Trial
  - Active
  - PastDue
  - Suspended
  - Canceled
- Tenant access controlled by subscription status when billing is enabled.
- Canceled tenant has configurable grace period for data export.

Backend:
- Add Subscription entity if not already present.
- Add platform billing setting.
- Add subscription access middleware/service.
- Add endpoints for platform admin to update subscription state.
- Add export-only mode for canceled grace period.

Testing:
- Unit test access matrix.
- Integration test billing off allows trial tenant access.
- Integration test billing on blocks suspended tenant.
- Integration test canceled within grace period allows export only.
- Integration test subscription changes audited.

Frontend:
- Show access-blocked page where appropriate.
- Show export-only notice where applicable.
```

---

# Prompt 38 — Sandbox Reset and Tenant Export

```text
Implement sandbox reset and tenant export.

Requirements:
- Every tenant has sandbox and production.
- Sandbox data isolated from production.
- Admin can reset/clean sandbox data.
- Tenant export includes:
  - Structured data
  - Documents
  - Forms
  - Mappings
  - Reports
  - Audit logs
- Export should run as background job if large.

Backend:
- Add endpoint POST /api/sandbox/reset.
- Add endpoint POST /api/tenant/export.
- Add endpoint GET /api/tenant/exports/{id}/download.
- Ensure sandbox reset cannot delete production data.
- Ensure audit logs record reset/export.

Testing:
- Integration test sandbox reset deletes only sandbox data.
- Integration test production data remains.
- Integration test tenant export includes expected manifest.
- Integration test export respects tenant isolation.
- Integration test audit log.

Frontend:
- Add sandbox reset action in Settings with strong confirmation.
- Add tenant export action in Settings.
```

---

# Prompt 39 — Observability, Security Hardening, and Performance

```text
Harden the application for MVP beta.

Requirements:
- Central logging.
- Traces and correlation IDs.
- Metrics for errors, job failures, integration failures.
- Application Insights-ready abstraction/configuration.
- Security headers.
- Rate limiting on auth endpoints.
- File upload validation:
  - Allowed extensions
  - Max file size
  - Content type checks
- Common list/search pages target under 2 seconds under normal load.
- Background jobs have retry/status/error visibility.

Testing:
- Integration test security headers.
- Integration test auth endpoint rate limiting.
- Integration test oversized file rejected.
- Integration test unsupported file type rejected.
- Integration test background job failure visible.
- Add basic performance test or documented benchmark script for main list endpoints.

Documentation:
- Add /docs/security.md.
- Add /docs/observability.md.
- Add /docs/performance-checklist.md.
```

---

# Prompt 40 — End-to-End MVP Flow and Deployment Runbook

```text
Create final MVP end-to-end tests and deployment runbook.

E2E test happy path:
1. Register dealership.
2. Verify email.
3. Log in.
4. Confirm sandbox environment.
5. Configure dealer profile.
6. Create customer manually or from AAMVA test payload.
7. Create inventory vehicle with VIN decode fake.
8. Create deal.
9. Add owner.
10. Add financials.
11. Upload form template.
12. Save mapping.
13. Configure simple rule.
14. Run checklist.
15. Generate packet.
16. Capture in-store signature or use fake DocuSign.
17. Export deal ZIP.
18. Confirm audit logs exist.

Also add negative E2E tests:
- Dealer cannot access another tenant.
- Production and sandbox data are isolated.
- Missing required checklist item blocks packet generation.
- Sensitive DL/DOB masked by default.

Documentation:
- Add /docs/deployment-runbook.md with:
  - Required Azure resources
  - App settings
  - Database migration process
  - Blob storage setup
  - Key Vault secrets
  - Monitoring setup
  - Backup/restore notes
  - Rollback plan
- Add /docs/uat-script.md for product owner testing.

Final validation:
- All tests pass.
- No orphaned code.
- Every module reachable from UI or API.
- README has local setup and test commands.
```

---

# Final Notes

This prompt plan intentionally keeps the MVP deterministic:

- No AI in V1.
- No OCR in V1.
- Driver license data comes from AAMVA barcode parsing or manual entry.
- Non-DL documents are stored only, with no extraction.
- `Document.ExtractedData` is reserved for V2 OCR and cross-document validation.
- Electronic DMV submission is out of scope for V1.

The best implementation pattern is:

```text
Prompt → Branch → Tests → Review → Commit → Merge → Next Prompt
```

