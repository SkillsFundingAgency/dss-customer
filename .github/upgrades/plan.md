# .NET 10 Upgrade Migration Plan

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
  - [NCS.DSS.Customer](#ncsdss-customer)
  - [NCS.DSS.Customer.Tests](#ncsdss-customer-tests)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description

This plan outlines the migration of the NCS.DSS.Customer Azure Functions solution from **.NET 8.0** to **.NET 10.0 (LTS)**. The solution consists of 2 projects:
- **NCS.DSS.Customer** - Azure Functions Worker application (2,129 LOC)
- **NCS.DSS.Customer.Tests** - NUnit test project (1,674 LOC)

### Scope

**Projects Affected:** 2 projects (3,803 total LOC)  
**Current State:** net8.0  
**Target State:** net10.0  
**Package Updates Required:** 9 packages  
**Deprecated Package:** 1 (Microsoft.Identity.Client - marked as deprecated, but still compatible)

### Selected Strategy

**All-At-Once Strategy** - All projects upgraded simultaneously in a single coordinated operation.

**Rationale:**
- **Simple Solution:** Only 2 projects with straightforward linear dependency (Tests ? Customer)
- **Low Complexity:** Both projects rated as Low difficulty, total codebase under 4,000 LOC
- **No Critical Issues:** No security vulnerabilities, no binary-incompatible API changes
- **Minimal Risk:** Only 3 behavioral API changes (low impact), all packages have clear upgrade paths
- **Fast Completion:** All-at-once approach minimizes total timeline for small solutions

### Complexity Assessment

**Discovered Metrics:**
- Total Projects: 2
- Dependency Depth: 1 level (linear)
- Lines of Code: 3,803
- Package Updates: 9 packages (40.9% of total)
- Compatible Packages: 13 packages (59.1%)
- Deprecated Packages: 1 (Microsoft.Identity.Client)
- API Behavioral Changes: 3 (System.Uri, Microsoft.Extensions.Hosting.HostBuilder)

**Classification:** **Simple**
- Both projects: Low difficulty
- No high-risk code patterns identified
- Clear dependency structure
- All package updates have defined target versions

### Critical Issues

**Deprecated Package:**
- **Microsoft.Identity.Client 4.61.3** - Marked as deprecated but still compatible with .NET 10. Monitor for replacement guidance from Microsoft in future releases.

**Behavioral Changes:**
- **System.Uri** - Constructor behavior changes
- **Microsoft.Extensions.Hosting.HostBuilder** - Hosting model behavior changes

These behavioral changes require runtime testing to ensure no regressions.

### Recommended Approach

**All-at-once atomic upgrade** with the following execution order:
1. Update both project files to net10.0 simultaneously
2. Update all 9 package references in a single batch
3. Restore dependencies and build entire solution
4. Fix any compilation errors discovered
5. Execute all tests to verify behavioral compatibility
6. Commit as single atomic change

**Estimated Timeline:** Single coordinated upgrade operation

### Iteration Strategy Used

**Fast Batch Approach** - Due to simple solution classification:
- Phase 1: Discovery & Classification (3 iterations) ?
- Phase 2: Foundation (3 iterations) - Next
- Phase 3: Dynamic Detail (2-3 iterations) - Both projects batched together

---

## Migration Strategy

### Approach Selection

**Selected Strategy:** **All-At-Once Strategy**

**Justification:**

This solution meets all ideal conditions for an all-at-once upgrade:

? **Small Solution:** 2 projects (well below 30-project threshold)  
? **Homogeneous Codebase:** Both projects currently on net8.0, targeting same net10.0 version  
? **Low External Dependency Complexity:** All 9 packages requiring updates have known target versions  
? **Clear Package Upgrade Path:** All packages are either compatible or have documented upgrade versions  
? **No Security Vulnerabilities:** Assessment shows zero security issues  
? **Low Risk Profile:** Both projects rated as Low difficulty, only 3 behavioral API changes

**Advantages for This Solution:**
- **Fastest Completion:** Single atomic operation eliminates multi-phase coordination
- **No Multi-Targeting:** Avoid complexity of maintaining net8.0 and net10.0 simultaneously
- **Clean Dependency Resolution:** All projects resolve dependencies at net10.0 level immediately
- **Simple Testing:** Single comprehensive test run validates entire upgrade
- **Single Commit:** Entire upgrade captured in one atomic source control commit

**Risks Mitigated:**
- Small codebase (3,803 LOC) allows comprehensive testing in single pass
- Linear dependency structure prevents cascade failures
- All package updates are well-defined (no experimental or preview packages)

### All-At-Once Strategy Rationale

**Why All-At-Once is Optimal Here:**

1. **Solution Characteristics:**
   - Only 2 projects with straightforward dependency relationship
   - Both projects share similar Azure Functions Worker technology stack
   - No legacy .NET Framework projects requiring gradual migration

2. **Package Update Profile:**
   - All 9 packages have clear net10.0-compatible versions
   - Azure Functions Worker packages move together as cohesive unit
   - No packages marked as incompatible or requiring alternatives

3. **Testing Capability:**
   - Test project (NCS.DSS.Customer.Tests) provides automated validation
   - NUnit test framework fully compatible with net10.0
   - Can validate entire upgrade in single test run

4. **Team Coordination:**
   - Single upgrade branch minimizes merge conflicts
   - All developers adapt to net10.0 simultaneously
   - No confusion from mixed framework versions

### Dependency-Based Ordering

While all projects upgrade simultaneously, the **validation order** follows dependency structure:

1. **Build Order:**
   - NCS.DSS.Customer (leaf) must build first
   - NCS.DSS.Customer.Tests (depends on Customer) builds second

2. **Compilation Error Resolution:**
   - Fix errors in NCS.DSS.Customer first
   - Then address any test project errors

3. **Testing Order:**
   - Run all tests in NCS.DSS.Customer.Tests after main project builds successfully

**No Parallel Execution Required:** Linear dependency chain means sequential build is natural and efficient.

### Execution Sequence

**Phase 0: Preparation**
- Verify .NET 10 SDK installed
- Confirm on upgrade-to-NET10 branch

**Phase 1: Atomic Upgrade** (single coordinated operation)
1. Update TargetFramework to net10.0 in both .csproj files
2. Update all 9 package references to target versions across both projects
3. Restore NuGet dependencies (`dotnet restore`)
4. Build entire solution (`dotnet build`)
5. Fix any compilation errors (reference Breaking Changes Catalog)
6. Rebuild to verify 0 errors

**Phase 2: Test Validation**
1. Execute all tests in NCS.DSS.Customer.Tests
2. Address any test failures related to behavioral API changes
3. Rerun tests to verify 100% pass rate

**Phase 3: Final Validation**
1. Full solution rebuild
2. Verify 0 warnings
3. Confirm deprecated package (Microsoft.Identity.Client) still functions
4. Commit atomic upgrade

### Risk Management Alignment

**All-At-Once Approach Fits Risk Profile:**
- **Low Risk Projects:** Both rated Low difficulty
- **Minimal Breaking Changes:** Only 3 behavioral changes (no binary incompatibilities)
- **Strong Test Coverage:** Automated test project validates changes
- **Fast Rollback:** Single commit makes reverting straightforward if needed

**Contingency Plan:**
If critical issues arise during Phase 1:
- Revert single commit back to net8.0
- Reassess specific blocking issue
- Apply targeted fix
- Retry atomic upgrade

**Not Recommended Alternatives:**
- ? **Incremental Migration:** Unnecessary overhead for 2-project solution
- ? **Multi-Targeting:** Adds complexity without benefit for this simple structure

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a straightforward linear dependency structure:

```
NCS.DSS.Customer.Tests (test project)
        ?
NCS.DSS.Customer (Azure Functions application)
```

**Key Characteristics:**
- **Dependency Depth:** 1 level
- **Leaf Node:** NCS.DSS.Customer (0 project dependencies)
- **Root Node:** NCS.DSS.Customer.Tests (depends on Customer project)
- **Circular Dependencies:** None
- **Shared Dependencies:** Both projects share Azure.Search.Documents, DFC.HTTP.Standard, DFC.JSON.Standard, Microsoft.Extensions.DependencyInjection, and System.Data.SqlClient packages

### Project Groupings by Migration Phase

For All-At-Once strategy, all projects are upgraded in a single phase:

#### Phase 1: Atomic Upgrade (All Projects)

**Projects:**
1. **NCS.DSS.Customer** (leaf node - Azure Functions Worker)
2. **NCS.DSS.Customer.Tests** (root node - test project)

**Rationale:**
- Both projects are tightly coupled (tests depend on main application)
- Simple dependency structure allows safe simultaneous upgrade
- No intermediate multi-targeting required
- All package updates can be applied atomically

**Execution Approach:**
- Update both TargetFramework properties to net10.0 simultaneously
- Update all package references across both projects in single operation
- Build entire solution to identify any compilation errors
- Fix errors in batch (breaking changes are minimal)
- Run tests to validate behavioral compatibility

### Critical Path Identification

**Critical Path:** NCS.DSS.Customer ? NCS.DSS.Customer.Tests

**Key Considerations:**
1. **NCS.DSS.Customer must build successfully** before tests can be validated
2. **Azure Functions Worker packages** must be upgraded together (Worker, Worker.Sdk, Worker.ApplicationInsights, Worker.Extensions.*)
3. **Shared packages** (Microsoft.Extensions.DependencyInjection, Azure.Search.Documents) must use compatible versions across both projects

**No Blocking Dependencies:** All packages have clear upgrade paths to .NET 10-compatible versions.

### Circular Dependency Details

**None detected.** The dependency graph is acyclic with clear directional flow.

---

## Project-by-Project Plans

### NCS.DSS.Customer

**Project Type:** Azure Functions Worker Application  
**Current State:** net8.0, Azure Functions v4, 16 NuGet packages, 2,129 LOC  
**Target State:** net10.0, Azure Functions v4 (with updated Worker packages)

**Current Framework:** net8.0  
**Target Framework:** net10.0

**Package Count:** 16 packages total
- 8 packages require updates
- 7 packages are compatible as-is
- 1 deprecated package (Microsoft.Identity.Client - still compatible)

**Dependencies:** 0 project references (leaf node)  
**Dependants:** 1 (NCS.DSS.Customer.Tests)

**Risk Level:** ?? Low
- Well-structured Azure Functions Worker project
- All critical packages have clear upgrade paths
- Only 3 behavioral API changes (low impact)

#### Migration Steps

##### 1. Prerequisites

**Verify .NET 10 SDK Installation:**
- Confirm .NET 10 SDK is installed on development machine
- Run: `dotnet --list-sdks` to verify net10.0 is available
- If missing, download from [.NET 10 Download](https://dotnet.microsoft.com/download/dotnet/10.0)

**Dependencies:**
- None (leaf node project)

**Tooling:**
- Azure Functions Core Tools v4 (compatible with Worker v2)
- Visual Studio 2022 17.12+ or Visual Studio Code with C# extension

##### 2. Technology/Framework Update

**File:** `NCS.DSS.Customer\NCS.DSS.Customer.csproj`

**Change TargetFramework property:**

```xml
<!-- BEFORE -->
<TargetFramework>net8.0</TargetFramework>

<!-- AFTER -->
<TargetFramework>net10.0</TargetFramework>
```

**Other Project Properties (No Changes Required):**
- `<AzureFunctionsVersion>v4</AzureFunctionsVersion>` - Remains unchanged
- `<OutputType>Exe</OutputType>` - Remains unchanged
- `<ImplicitUsings>enable</ImplicitUsings>` - Remains unchanged

##### 3. Package/Dependency Updates

Update the following `<PackageReference>` elements in `NCS.DSS.Customer.csproj`:

| Package Name | Current Version | Target Version | Reason |
|--------------|-----------------|----------------|--------|
| **Microsoft.Azure.Functions.Worker** | 1.22.0 | **2.51.0** | .NET 10 support, major version upgrade |
| **Microsoft.Azure.Functions.Worker.Sdk** | 1.17.4 | **2.0.7** | .NET 10 support, major version upgrade |
| **Microsoft.Azure.Functions.Worker.ApplicationInsights** | 2.0.0 | **2.50.0** | Compatibility with Worker 2.x |
| **Microsoft.Azure.Functions.Worker.Extensions.Http** | 3.2.0 | **3.3.0** | .NET 10 compatibility |
| **Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore** | 1.3.2 | **2.1.0** | .NET 10 support, major version upgrade |
| **Microsoft.Azure.Functions.Worker.Extensions.CosmosDB** | 3.0.9 | **4.14.0** | .NET 10 support, major version upgrade |
| **Microsoft.ApplicationInsights.WorkerService** | 2.22.0 | **2.23.0** | .NET 10 compatibility |
| **Microsoft.Extensions.DependencyInjection** | 9.0.0 | **10.0.2** | .NET 10 framework alignment |

**Packages Remaining Unchanged (Compatible):**
- Azure.Messaging.ServiceBus 7.17.5 ?
- Azure.Search.Documents 11.5.1 ?
- DFC.HTTP.Standard 0.1.11 ?
- DFC.JSON.Standard 0.1.4 ?
- DFC.Swagger.Standard 0.1.36 ?
- Microsoft.Azure.Cosmos 3.41.0 ?
- Microsoft.Identity.Client 4.61.3 ?? (deprecated but compatible)
- System.Data.SqlClient 4.8.6 ?

##### 4. Expected Breaking Changes

**API Behavioral Changes:**

1. **System.Uri Constructor Behavior**
   - **Impact:** URI parsing may behave differently for edge cases
   - **Affected Code:** Any HTTP trigger endpoints using URI parsing
   - **Action Required:** Test all HTTP triggers that parse or construct URIs
   - **Likelihood:** Low (affects edge cases like malformed URIs)

2. **Microsoft.Extensions.Hosting.HostBuilder**
   - **Impact:** Azure Functions host initialization sequence may differ
   - **Affected Code:** `Program.cs` or custom host configuration
   - **Action Required:** Verify Functions host starts correctly and DI container initializes
   - **Likelihood:** Low (Azure Functions Worker SDK handles most hosting logic)

**Azure Functions Worker 2.x Breaking Changes:**

The upgrade from Worker 1.x to 2.x may introduce:

- **Function Signature Changes:** Method signatures for bindings may have updated types
- **Middleware Registration:** Middleware configuration API may differ
- **Output Bindings:** Return types for output bindings may require updates
- **Dependency Injection:** Service registration patterns may need adjustment

**Mitigation:** Consult [Azure Functions Worker 2.x Migration Guide](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide) for detailed changes.

##### 5. Code Modifications

**Areas Requiring Review:**

1. **Program.cs / Host Configuration:**
   - Verify `HostBuilder` initialization still works
   - Check `ConfigureFunctionsWorkerDefaults()` method signature
   - Validate dependency injection service registrations
   - Ensure Application Insights configuration is correct

2. **Function Trigger Methods:**
   - Review HTTP trigger method signatures
   - Check Cosmos DB trigger bindings
   - Verify Service Bus trigger bindings
   - Update any output binding return types if needed

3. **Middleware (if any):**
   - Update middleware registration to Worker 2.x API
   - Check custom middleware implementations for compatibility

4. **URI Handling:**
   - Test any code that constructs or parses URIs
   - Verify URL routing logic in HTTP triggers

5. **Configuration Files:**
   - Review `host.json` for Worker 2.x compatibility
   - Check `local.settings.json` for any required updates

**Expected Code Changes:**
- Minimal changes expected (Worker SDK abstracts most breaking changes)
- Possible minor adjustments to function signatures or DI registration
- Configuration file updates if using advanced hosting features

##### 6. Testing Strategy

**Unit Testing:**
- Not directly applicable (main project has no tests)
- Tests executed via NCS.DSS.Customer.Tests project

**Integration Testing:**
- Run NCS.DSS.Customer.Tests suite after upgrade
- Validate all function triggers work correctly
- Test dependency injection container resolution

**Manual Testing Requirements:**
- Start Azure Functions locally using Functions Core Tools
- Test each HTTP endpoint manually
- Verify Cosmos DB trigger fires correctly
- Test Service Bus message processing
- Check Application Insights telemetry

**Performance Testing:**
- Monitor startup time (Worker 2.x may have different performance characteristics)
- Check memory usage after upgrade
- Validate cold start performance

##### 7. Validation Checklist

- [ ] **Build Success:** Project builds without errors on net10.0
- [ ] **No Warnings:** Project builds with 0 warnings (or only known acceptable warnings)
- [ ] **Package Restoration:** All NuGet packages restore successfully without conflicts
- [ ] **Functions Host Starts:** Azure Functions host initializes without errors
- [ ] **DI Container Works:** Dependency injection resolves all services correctly
- [ ] **HTTP Triggers Respond:** All HTTP endpoints return expected responses
- [ ] **Cosmos Trigger Fires:** Cosmos DB trigger processes changes correctly
- [ ] **Service Bus Processes:** Service Bus trigger handles messages correctly
- [ ] **Application Insights Logs:** Telemetry data flows to Application Insights
- [ ] **No Deprecated API Usage:** Code analysis shows no obsolete API warnings (except known deprecated packages)
- [ ] **Authentication Works:** Microsoft.Identity.Client authentication flows function correctly (if used)
- [ ] **Tests Pass:** All tests in NCS.DSS.Customer.Tests pass

### NCS.DSS.Customer.Tests

**Project Type:** NUnit Test Project  
**Current State:** net8.0, 9 NuGet packages, 1,674 LOC  
**Target State:** net10.0

**Current Framework:** net8.0  
**Target Framework:** net10.0

**Package Count:** 9 packages total
- 1 package requires update (Microsoft.Extensions.DependencyInjection)
- 8 packages are compatible as-is

**Dependencies:** 1 project reference (NCS.DSS.Customer)  
**Dependants:** 0 (root node)

**Risk Level:** ?? Low
- Standard NUnit test project
- Minimal package updates required
- No API compatibility issues

#### Migration Steps

##### 1. Prerequisites

**Verify NCS.DSS.Customer Upgraded:**
- ? NCS.DSS.Customer must be upgraded to net10.0 first
- ? NCS.DSS.Customer must build successfully before test project upgrade
- This project has a project reference dependency on NCS.DSS.Customer

**Dependencies:**
- **Project Reference:** NCS.DSS.Customer (must be at net10.0)

**Tooling:**
- NUnit test runner (compatible with .NET 10)
- Visual Studio Test Explorer or `dotnet test` CLI

##### 2. Technology/Framework Update

**File:** `NCS.DSS.Customer.Tests\NCS.DSS.Customer.Tests.csproj`

**Change TargetFramework property:**

```xml
<!-- BEFORE -->
<TargetFramework>net8.0</TargetFramework>

<!-- AFTER -->
<TargetFramework>net10.0</TargetFramework>
```

##### 3. Package/Dependency Updates

Update the following `<PackageReference>` element in `NCS.DSS.Customer.Tests.csproj`:

| Package Name | Current Version | Target Version | Reason |
|--------------|-----------------|----------------|--------|
| **Microsoft.Extensions.DependencyInjection** | 9.0.0 | **10.0.2** | .NET 10 framework alignment |

**Packages Remaining Unchanged (Compatible):**
- Azure.Search.Documents 11.5.1 ?
- DFC.Functions.DI.Standard 0.1.0 ?
- DFC.HTTP.Standard 0.1.11 ?
- DFC.JSON.Standard 0.1.4 ?
- Microsoft.Azure.WebJobs.Extensions.CosmosDB 4.7.0 ?
- Microsoft.NET.Test.Sdk 17.10.0 ?
- Moq 4.20.70 ?
- NUnit 4.1.0 ?
- NUnit3TestAdapter 4.5.0 ?
- System.Data.SqlClient 4.8.6 ?

##### 4. Expected Breaking Changes

**API Behavioral Changes:**
- **None identified** for test project
- All test framework packages (NUnit, Moq) are fully compatible with .NET 10
- Microsoft.Extensions.DependencyInjection update is minor version bump (9.0?10.0)

**Potential Test Failures:**
- Tests may fail due to behavioral changes in **NCS.DSS.Customer** (main project)
- System.Uri or HostBuilder behavioral changes may surface in test assertions
- Test failures would indicate issues in main project code, not test framework

##### 5. Code Modifications

**Expected Changes:**
- **None required** for test project itself
- Tests are expected to run without modification on .NET 10

**If Tests Fail:**
1. Analyze failure messages to determine root cause
2. Check if failures are due to:
   - Behavioral API changes in System.Uri
   - HostBuilder initialization differences
   - Azure Functions Worker 2.x changes
3. Update test assertions or mock configurations as needed
4. Update main project code if behavioral changes are unintended

**Areas to Monitor:**
- Tests that validate URI construction or parsing
- Tests that mock or configure the Functions host
- Tests that verify dependency injection behavior
- Integration tests that call Azure Functions endpoints

##### 6. Testing Strategy

**Unit Testing:**
- **Primary Goal:** Execute entire NUnit test suite
- Run: `dotnet test NCS.DSS.Customer.Tests.csproj`
- Verify 100% test pass rate

**Test Execution Order:**
1. Restore NuGet packages
2. Build NCS.DSS.Customer.Tests (depends on main project)
3. Run all tests using NUnit test runner
4. Analyze any failures

**Test Categories to Validate:**
- Unit tests for NCS.DSS.Customer services/functions
- Mock-based tests (verify Moq compatibility)
- Integration tests (if any)
- Dependency injection tests

**Failure Analysis:**
- Categorize failures by root cause (API behavioral change, Functions Worker change, etc.)
- Prioritize fixing failures in main project code over test code
- Re-run tests after each fix to verify resolution

##### 7. Validation Checklist

- [ ] **Build Success:** Test project builds without errors on net10.0
- [ ] **No Warnings:** Project builds with 0 warnings
- [ ] **Package Restoration:** All NuGet packages restore successfully without conflicts
- [ ] **Project Reference Resolves:** NCS.DSS.Customer reference resolves at net10.0
- [ ] **Test Discovery:** NUnit discovers all tests correctly
- [ ] **All Tests Pass:** 100% test pass rate
- [ ] **No Test Framework Errors:** NUnit/Moq work correctly on .NET 10
- [ ] **Mock Objects Work:** Moq-based mocks function as expected
- [ ] **DI Tests Pass:** Dependency injection tests validate container configuration
- [ ] **No Flaky Tests:** Tests pass consistently across multiple runs

---

## Package Update Reference

### Overview

**Total Packages:** 22 unique packages across both projects  
**Updates Required:** 9 packages (40.9%)  
**Compatible (No Update):** 13 packages (59.1%)  
**Deprecated:** 1 package (Microsoft.Identity.Client)

### Packages Requiring Updates

| Package Name | Current Version | Target Version | Projects Affected | Update Reason | Priority |
|--------------|-----------------|----------------|-------------------|---------------|----------|
| **Microsoft.Azure.Functions.Worker** | 1.22.0 | 2.51.0 | NCS.DSS.Customer | .NET 10 support, major version upgrade | ?? Critical |
| **Microsoft.Azure.Functions.Worker.Sdk** | 1.17.4 | 2.0.7 | NCS.DSS.Customer | .NET 10 support, major version upgrade | ?? Critical |
| **Microsoft.Azure.Functions.Worker.ApplicationInsights** | 2.0.0 | 2.50.0 | NCS.DSS.Customer | Compatibility with Worker 2.x | ?? Critical |
| **Microsoft.Azure.Functions.Worker.Extensions.CosmosDB** | 3.0.9 | 4.14.0 | NCS.DSS.Customer | .NET 10 support, major version upgrade | ?? Critical |
| **Microsoft.Azure.Functions.Worker.Extensions.Http** | 3.2.0 | 3.3.0 | NCS.DSS.Customer | .NET 10 compatibility | ?? Critical |
| **Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore** | 1.3.2 | 2.1.0 | NCS.DSS.Customer | .NET 10 support, major version upgrade | ?? Critical |
| **Microsoft.Extensions.DependencyInjection** | 9.0.0 | 10.0.2 | NCS.DSS.Customer<br/>NCS.DSS.Customer.Tests | .NET 10 framework alignment | ?? Important |
| **Microsoft.ApplicationInsights.WorkerService** | 2.22.0 | 2.23.0 | NCS.DSS.Customer | .NET 10 compatibility | ?? Recommended |

**Update Strategy:**
- ? All updates applied atomically in single operation
- ? Azure Functions Worker packages must upgrade together (prevent version mismatch)
- ? Microsoft.Extensions.DependencyInjection updated to match .NET 10 runtime version

### Packages Remaining at Current Version (Compatible)

| Package Name | Version | Projects | Rationale |
|--------------|---------|----------|-----------|
| Azure.Messaging.ServiceBus | 7.17.5 | NCS.DSS.Customer | ? Fully compatible with .NET 10 |
| Azure.Search.Documents | 11.5.1 | NCS.DSS.Customer<br/>NCS.DSS.Customer.Tests | ? Fully compatible with .NET 10 |
| DFC.Functions.DI.Standard | 0.1.0 | NCS.DSS.Customer.Tests | ? Compatible (custom/internal package) |
| DFC.HTTP.Standard | 0.1.11 | NCS.DSS.Customer<br/>NCS.DSS.Customer.Tests | ? Compatible (custom/internal package) |
| DFC.JSON.Standard | 0.1.4 | NCS.DSS.Customer<br/>NCS.DSS.Customer.Tests | ? Compatible (custom/internal package) |
| DFC.Swagger.Standard | 0.1.36 | NCS.DSS.Customer | ? Compatible (custom/internal package) |
| Microsoft.Azure.Cosmos | 3.41.0 | NCS.DSS.Customer | ? Fully compatible with .NET 10 |
| Microsoft.Azure.WebJobs.Extensions.CosmosDB | 4.7.0 | NCS.DSS.Customer.Tests | ? Fully compatible with .NET 10 |
| Microsoft.NET.Test.Sdk | 17.10.0 | NCS.DSS.Customer.Tests | ? Fully compatible with .NET 10 |
| Moq | 4.20.70 | NCS.DSS.Customer.Tests | ? Fully compatible with .NET 10 |
| NUnit | 4.1.0 | NCS.DSS.Customer.Tests | ? Fully compatible with .NET 10 |
| NUnit3TestAdapter | 4.5.0 | NCS.DSS.Customer.Tests | ? Fully compatible with .NET 10 |
| System.Data.SqlClient | 4.8.6 | NCS.DSS.Customer<br/>NCS.DSS.Customer.Tests | ? Compatible (consider future migration to Microsoft.Data.SqlClient) |

### Deprecated Package

| Package Name | Version | Projects | Status | Action |
|--------------|---------|----------|--------|--------|
| **Microsoft.Identity.Client** | 4.61.3 | NCS.DSS.Customer | ?? Deprecated but compatible | Monitor for replacement guidance; consider migrating to Microsoft.Identity.Web or Azure.Identity in future release |

**Deprecated Package Notes:**
- **Current Status:** Package is marked as deprecated by maintainers but still functions correctly on .NET 10
- **Immediate Action:** None required for this upgrade
- **Future Action:** Monitor Microsoft documentation for recommended replacement package
- **Alternatives:** Microsoft.Identity.Web (for web apps) or Azure.Identity (for Azure SDK authentication)
- **Timeline:** Plan migration in subsequent sprint after .NET 10 stabilizes

### Package Update Groupings

#### Azure Functions Worker Ecosystem (Must Update Together)

**Critical:** These packages form a cohesive unit and must be upgraded atomically to prevent version mismatches:

1. Microsoft.Azure.Functions.Worker: 1.22.0 ? 2.51.0
2. Microsoft.Azure.Functions.Worker.Sdk: 1.17.4 ? 2.0.7
3. Microsoft.Azure.Functions.Worker.ApplicationInsights: 2.0.0 ? 2.50.0
4. Microsoft.Azure.Functions.Worker.Extensions.Http: 3.2.0 ? 3.3.0
5. Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore: 1.3.2 ? 2.1.0
6. Microsoft.Azure.Functions.Worker.Extensions.CosmosDB: 3.0.9 ? 4.14.0

**Impact:** Major version upgrades (1.x ? 2.x) may introduce breaking changes in function signatures, bindings, or hosting model.

#### Framework-Aligned Packages

**Important:** These packages align with .NET runtime version:

1. Microsoft.Extensions.DependencyInjection: 9.0.0 ? 10.0.2

**Impact:** Minor API surface changes; generally backward compatible.

#### Application Insights

**Recommended:** Telemetry and monitoring package:

1. Microsoft.ApplicationInsights.WorkerService: 2.22.0 ? 2.23.0

**Impact:** Minimal; primarily bug fixes and .NET 10 optimizations.

### Shared Package Considerations

**Microsoft.Extensions.DependencyInjection** is used by both projects:
- Must use same version (10.0.2) across both projects to prevent assembly conflicts
- Update in both NCS.DSS.Customer and NCS.DSS.Customer.Tests simultaneously

**Azure.Search.Documents** is used by both projects:
- No update required (11.5.1 is compatible)
- Both projects can remain at current version

### Package Update Validation

After updating packages, validate:
- [ ] No package version conflicts (run `dotnet list package --include-transitive`)
- [ ] All packages restore successfully (`dotnet restore`)
- [ ] No downgrade warnings in build output
- [ ] Azure Functions Worker packages are all at 2.x versions
- [ ] Microsoft.Extensions.DependencyInjection is 10.0.2 in both projects

---

## Breaking Changes Catalog

### Overview

**Total Breaking Changes Identified:** 3 behavioral API changes  
**Binary-Incompatible Changes:** 0  
**Source-Incompatible Changes:** 0  
**Behavioral Changes:** 3 (low impact)

**Risk Level:** ?? Low - No compilation errors expected; runtime validation required

### .NET 10 Framework Breaking Changes

#### 1. System.Uri Constructor Behavior Change

**Category:** ?? Behavioral Change  
**Severity:** Low  
**Impact Area:** URI parsing and construction

**Description:**
The `System.Uri` constructor behavior has changed in .NET 10 regarding how it handles certain edge cases in URI parsing, particularly around:
- Relative URI handling
- Invalid URI characters
- URI encoding/decoding

**Affected Code Patterns:**
```csharp
// Code that constructs URIs from user input or external sources
var uri = new Uri(userProvidedUrl);

// HTTP trigger endpoints that parse request URLs
public HttpResponseData Run([HttpTrigger] HttpRequestData req)
{
    var requestUri = req.Url; // May behave differently
}

// Code that manipulates or parses URI components
var baseUri = new Uri("https://api.example.com");
var relativeUri = new Uri(baseUri, "/path/to/resource");
```

**Projects Affected:**
- **NCS.DSS.Customer** - HTTP trigger functions that handle URI parsing

**Mitigation Strategy:**
1. Test all HTTP trigger endpoints with various URI inputs
2. Validate URI parsing logic with edge cases:
   - URLs with special characters
   - Relative vs absolute URIs
   - Malformed URIs
3. Add defensive validation for user-provided URLs
4. Check Application Insights logs for URI-related exceptions after deployment

**Detection:**
- Run integration tests for all HTTP endpoints
- Manual testing with various URL formats
- Monitor for `UriFormatException` in logs

**Likelihood of Impact:** Low (most common URI usage patterns are unaffected)

---

#### 2. System.Uri.#ctor(System.String) Specific Behavior

**Category:** ?? Behavioral Change  
**Severity:** Low  
**Impact Area:** URI string parsing

**Description:**
Specific changes to the `Uri(string)` constructor overload related to:
- Whitespace handling in URI strings
- Percent-encoding normalization
- Port number parsing

**Affected Code Patterns:**
```csharp
// URI construction from configuration or database values
var apiEndpoint = new Uri(configuration["ApiUrl"]);

// Service Bus or Cosmos DB connection URIs
var cosmosUri = new Uri(cosmosDbSettings.Endpoint);
```

**Projects Affected:**
- **NCS.DSS.Customer** - Configuration-based URI construction for Azure services

**Mitigation Strategy:**
1. Verify all URIs constructed from configuration settings
2. Test Azure service connections (Cosmos DB, Service Bus, Search)
3. Ensure no whitespace or invalid characters in configuration values
4. Add try-catch blocks around URI construction from external sources

**Detection:**
- Validate Functions startup (DI container initialization)
- Test connections to all Azure services
- Check for initialization errors in Application Insights

**Likelihood of Impact:** Very Low (Azure SDK connection strings are well-formed)

---

#### 3. Microsoft.Extensions.Hosting.HostBuilder Behavior Change

**Category:** ?? Behavioral Change  
**Severity:** Low  
**Impact Area:** Application host initialization

**Description:**
The `HostBuilder` class behavior has changed in .NET 10 regarding:
- Service provider initialization order
- Configuration loading sequence
- Default logging configuration

In the context of Azure Functions Worker, this affects how `IHostBuilder` is configured in `Program.cs`.

**Affected Code Patterns:**
```csharp
// Azure Functions Program.cs host configuration
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // DI registrations
    })
    .Build();

// Custom configuration loading
builder.ConfigureAppConfiguration(config =>
{
    // Configuration sources
});
```

**Projects Affected:**
- **NCS.DSS.Customer** - `Program.cs` host initialization

**Mitigation Strategy:**
1. Test Functions host startup thoroughly
2. Verify all services registered in DI container resolve correctly
3. Check logging configuration still works
4. Validate Application Insights integration
5. Ensure configuration sources load in expected order

**Detection:**
- Run Functions locally with `func start`
- Check for startup errors in console output
- Verify DI container resolves all services
- Test logging and Application Insights telemetry

**Likelihood of Impact:** Low (Azure Functions Worker SDK likely abstracts these changes)

---

### Azure Functions Worker 2.x Breaking Changes

#### Major Version Upgrade: 1.x ? 2.x

**Packages Affected:**
- Microsoft.Azure.Functions.Worker: 1.22.0 ? 2.51.0
- Microsoft.Azure.Functions.Worker.Sdk: 1.17.4 ? 2.0.7
- Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore: 1.3.2 ? 2.1.0
- Microsoft.Azure.Functions.Worker.Extensions.CosmosDB: 3.0.9 ? 4.14.0

**Potential Breaking Changes:**

##### A. Function Method Signatures

**Possible Impact:**
- Input/output binding types may have changed
- Trigger attribute parameters may have new requirements
- Return types for output bindings may differ

**Example Scenarios:**
```csharp
// HTTP Trigger - Check for signature changes
[Function("MyFunction")]
public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
{
    // Verify HttpRequestData and HttpResponseData still work
}

// Cosmos DB Trigger - Verify binding types
[Function("CosmosDBFunction")]
public void Run(
    [CosmosDBTrigger(/* parameters */)] IReadOnlyList<Document> documents)
{
    // Check if Document type is still supported
}
```

**Mitigation:**
- Compile project to identify signature mismatches
- Consult [Azure Functions Worker 2.x Migration Guide](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide)
- Update method signatures as needed

##### B. Middleware Configuration

**Possible Impact:**
- Middleware registration API may have changed
- Custom middleware interfaces may differ

**Mitigation:**
- Review any custom middleware implementations
- Update middleware registration in `Program.cs` if needed

##### C. Output Bindings

**Possible Impact:**
- Output binding return types may require updates
- Multi-output scenarios may have new patterns

**Mitigation:**
- Test all functions with output bindings
- Verify data is written correctly to output destinations

##### D. Configuration and Dependency Injection

**Possible Impact:**
- Service registration patterns may have changed
- Configuration binding may behave differently

**Mitigation:**
- Verify all DI services resolve correctly
- Test configuration binding to strongly-typed classes

**Reference Documentation:**
- [Azure Functions .NET Worker Migration](https://learn.microsoft.com/azure/azure-functions/migrate-version-3-version-4?tabs=net8-isolated-process)
- [Azure Functions Worker Release Notes](https://github.com/Azure/azure-functions-dotnet-worker/releases)

---

### Package-Specific Breaking Changes

#### Microsoft.Extensions.DependencyInjection (9.0 ? 10.0)

**Category:** Minor Version Update  
**Severity:** Very Low  
**Breaking Changes:** Minimal

**Potential Impacts:**
- Service lifetime validation may be stricter
- Keyed services feature (new in .NET 8) may have refinements

**Mitigation:**
- Verify DI container builds without errors
- Test all service resolutions
- Check for any DI-related warnings

**Likelihood of Impact:** Very Low (backward compatible)

---

#### Microsoft.ApplicationInsights.WorkerService (2.22 ? 2.23)

**Category:** Patch Version Update  
**Severity:** Very Low  
**Breaking Changes:** None expected

**Impacts:**
- Bug fixes and .NET 10 optimizations
- No API surface changes expected

**Mitigation:**
- Verify Application Insights telemetry still flows
- Check custom telemetry processors (if any)

**Likelihood of Impact:** Very Low

---

### Breaking Changes Resolution Strategy

#### Phase 1: Compile-Time Detection

1. **Build entire solution after package updates**
2. **Address compilation errors:**
   - Method signature mismatches
   - Type not found errors
   - Obsolete API usage warnings
3. **Review compiler warnings** for deprecated patterns

#### Phase 2: Runtime Validation

1. **Start Azure Functions locally**
2. **Test each function trigger:**
   - HTTP triggers with various request patterns
   - Cosmos DB trigger with sample data
   - Service Bus trigger with test messages
3. **Verify:**
   - Functions initialize without errors
   - DI container resolves all services
   - Logging and telemetry work
   - Connections to Azure services succeed

#### Phase 3: Automated Testing

1. **Run NUnit test suite** (NCS.DSS.Customer.Tests)
2. **Analyze test failures:**
   - URI-related test failures (System.Uri changes)
   - Hosting/DI test failures (HostBuilder changes)
   - Azure Functions-specific failures (Worker 2.x changes)
3. **Fix failing tests** or update application code

#### Phase 4: Manual Integration Testing

1. **Deploy to test environment** (if available)
2. **Execute smoke tests:**
   - All HTTP endpoints respond correctly
   - Background triggers process events
   - Application Insights shows telemetry
3. **Monitor for unexpected behavior** over 24-48 hours

---

### Quick Reference: Breaking Change Checklist

**Before Deployment:**
- [ ] Solution builds with 0 errors
- [ ] All NUnit tests pass
- [ ] Functions host starts locally without errors
- [ ] HTTP triggers respond to test requests
- [ ] Cosmos DB trigger processes sample data
- [ ] Service Bus trigger handles test messages
- [ ] Application Insights receives telemetry
- [ ] All DI services resolve correctly
- [ ] No URI parsing errors in logs
- [ ] Configuration loads successfully

**After Deployment:**
- [ ] Monitor Application Insights for exceptions
- [ ] Check function execution success rate
- [ ] Verify no performance degradation
- [ ] Validate authentication flows (if using Microsoft.Identity.Client)
- [ ] Confirm no unexpected behavioral changes

---

### Resources for Breaking Changes

**Official Documentation:**
- [.NET 10 Breaking Changes](https://learn.microsoft.com/dotnet/core/compatibility/10.0)
- [Azure Functions Worker Migration Guide](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide)
- [Azure Functions Worker GitHub Releases](https://github.com/Azure/azure-functions-dotnet-worker/releases)

**Search Strategies:**
- Search .NET GitHub issues for specific error messages
- Consult Azure Functions Worker changelog for 2.x changes
- Review Microsoft.Extensions.Hosting release notes for hosting changes

---

## Risk Management

### High-Level Assessment

**Overall Risk Level:** ?? **Low**

Both projects in this solution are rated as Low difficulty with minimal risk factors:

| Risk Factor | Status | Mitigation |
|-------------|--------|------------|
| Security Vulnerabilities | ? None | No action required |
| Binary-Incompatible APIs | ? None | No breaking code changes expected |
| Source-Incompatible APIs | ? None | No recompilation issues expected |
| Behavioral API Changes | ?? 3 found | Runtime testing required |
| Deprecated Packages | ?? 1 found | Monitor for replacement; currently compatible |
| Package Upgrade Complexity | ? Low | All packages have defined target versions |
| Dependency Conflicts | ? None | Linear dependency structure |

### Risk by Category

#### 1. Deprecated Package Risk (Low-Medium)

**Package:** Microsoft.Identity.Client 4.61.3

**Risk:** Package marked as deprecated by maintainers but still compatible with .NET 10

**Impact:** 
- Currently functions correctly on .NET 10
- May lose support in future releases
- Potential security vulnerabilities if not maintained

**Mitigation:**
- ? Leave at current version for this upgrade (4.61.3)
- ?? Monitor Microsoft's guidance for recommended replacement
- ?? Consider migrating to Microsoft.Identity.Web or Azure.Identity in future sprint
- ? Document in code that this package is deprecated
- ? Test authentication flows thoroughly after upgrade

**Timeline:** Address in subsequent release after .NET 10 upgrade stabilizes

#### 2. Behavioral API Changes Risk (Low)

**Affected APIs:**
1. **System.Uri** - Constructor behavior changes
2. **System.Uri Constructor** - Parameter handling changes  
3. **Microsoft.Extensions.Hosting.HostBuilder** - Hosting initialization changes

**Impact:**
- No compilation errors expected
- Potential runtime behavior differences
- May affect Azure Functions startup or URI handling logic

**Mitigation:**
- ? Execute comprehensive test suite (NCS.DSS.Customer.Tests)
- ? Manual testing of Azure Functions endpoints
- ? Verify URI parsing in HTTP triggers
- ? Test Functions host initialization
- ?? Monitor Application Insights for runtime exceptions after deployment

#### 3. Azure Functions Package Upgrade Risk (Low)

**Packages Being Updated Together:**
- Microsoft.Azure.Functions.Worker: 1.22.0 ? 2.51.0 (major version jump)
- Microsoft.Azure.Functions.Worker.Sdk: 1.17.4 ? 2.0.7 (major version jump)
- Microsoft.Azure.Functions.Worker.ApplicationInsights: 2.0.0 ? 2.50.0
- Microsoft.Azure.Functions.Worker.Extensions.Http: 3.2.0 ? 3.3.0
- Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore: 1.3.2 ? 2.1.0 (major version jump)
- Microsoft.Azure.Functions.Worker.Extensions.CosmosDB: 3.0.9 ? 4.14.0 (major version jump)

**Risk:** Multiple major version upgrades in core Functions Worker packages

**Impact:**
- Significant API surface changes possible
- Potential breaking changes in function signatures, bindings, or middleware
- Host.json configuration may need updates

**Mitigation:**
- ? Upgrade all Functions Worker packages atomically (prevents version mismatch)
- ? Review Azure Functions Worker 2.x migration guide
- ? Test all function triggers (HTTP, Cosmos, Service Bus)
- ? Validate dependency injection still works correctly
- ? Check host.json and local.settings.json compatibility
- ?? Refer to Breaking Changes Catalog for specific API changes

#### 4. Build and Compilation Risk (Very Low)

**Assessment Findings:**
- 0 Binary-incompatible API changes
- 0 Source-incompatible API changes
- All packages have successful resolution paths

**Mitigation:**
- ? Build entire solution after package updates
- ? Address any unexpected errors using Breaking Changes Catalog
- ? Leverage strong test coverage to catch regressions

### Contingency Plans

#### If Compilation Fails After Package Updates

**Scenario:** NuGet package conflicts or unexpected compilation errors

**Action Plan:**
1. Review error messages for specific package conflicts
2. Check for transitive dependency mismatches
3. Use `dotnet list package --include-transitive` to diagnose
4. Adjust package versions if necessary (document deviations)
5. Consult Breaking Changes Catalog for API-specific fixes

**Rollback:** Revert .csproj changes, restore NuGet packages to net8.0 versions

#### If Tests Fail After Upgrade

**Scenario:** Test failures due to behavioral API changes

**Action Plan:**
1. Categorize failures: URI handling, hosting, DI, or Azure Functions-specific
2. Review .NET 10 breaking changes documentation for affected APIs
3. Update test expectations or application code as needed
4. Re-run tests incrementally to isolate issues
5. Leverage Application Insights for runtime diagnostics

**Rollback:** Git revert to net8.0 state, reassess specific breaking changes

#### If Azure Functions Behavior Changes

**Scenario:** Functions runtime behaves differently (startup, bindings, triggers)

**Action Plan:**
1. Compare host.json configuration with Azure Functions Worker 2.x requirements
2. Review function method signatures for binding changes
3. Test locally with Azure Functions Core Tools
4. Check Application Insights for startup errors
5. Consult Azure Functions Worker v2 migration documentation

**Rollback:** Revert to net8.0, study Worker 2.x migration guides before retry

#### If Microsoft.Identity.Client Causes Issues

**Scenario:** Deprecated package fails or exhibits unexpected behavior

**Action Plan:**
1. Investigate specific authentication failures
2. Check for .NET 10-specific identity issues in Microsoft forums
3. Consider emergency migration to Microsoft.Identity.Web or Azure.Identity
4. Test authentication flows in isolation

**Rollback:** Revert to net8.0, plan separate identity package migration

### Risk Mitigation Summary

**Proactive Measures:**
- ? All-at-once strategy minimizes partial-upgrade risks
- ? Comprehensive test suite validates behavioral compatibility
- ? Linear dependency structure prevents cascade failures
- ? Single commit enables fast rollback

**Reactive Measures:**
- ?? Detailed breaking changes catalog for quick reference
- ?? Contingency plans for each risk category
- ?? Clear rollback procedures documented
- ?? Incremental testing approach to isolate failures

---

## Testing & Validation Strategy

### Overview

**Testing Approach:** Multi-level validation aligned with All-At-Once strategy

**Levels:**
1. **Build Validation** - Ensure solution compiles on .NET 10
2. **Automated Testing** - Execute NUnit test suite
3. **Local Integration Testing** - Manual validation of Azure Functions
4. **Smoke Testing** - Quick validation of critical functionality

**Success Criteria:** All tests pass, all functions execute correctly, no unexpected behavioral changes

---

### Phase 1: Build Validation

**Objective:** Verify solution compiles successfully on .NET 10 with updated packages

**Steps:**

1. **Restore NuGet Packages**
   ```bash
   dotnet restore NCS.DSS.Customer.sln
   ```
   **Expected:** All packages restore without conflicts or warnings

2. **Build NCS.DSS.Customer (Main Project)**
   ```bash
   dotnet build NCS.DSS.Customer\NCS.DSS.Customer.csproj --configuration Release
   ```
   **Expected:** 0 errors, minimal warnings

3. **Build NCS.DSS.Customer.Tests (Test Project)**
   ```bash
   dotnet build NCS.DSS.Customer.Tests\NCS.DSS.Customer.Tests.csproj --configuration Release
   ```
   **Expected:** 0 errors, minimal warnings

4. **Build Entire Solution**
   ```bash
   dotnet build NCS.DSS.Customer.sln --configuration Release
   ```
   **Expected:** 0 errors across both projects

**Validation Checklist:**
- [ ] No NuGet package restoration errors
- [ ] No package version conflicts (check `dotnet list package --include-transitive`)
- [ ] Zero compilation errors
- [ ] Zero warnings (or only acceptable warnings documented)
- [ ] Build output shows net10.0 target framework
- [ ] Azure Functions SDK detects Functions correctly

**If Build Fails:**
- Review compilation errors against Breaking Changes Catalog
- Check for missing using statements or namespace changes
- Verify all Azure Functions Worker packages are at 2.x versions
- Consult Azure Functions Worker 2.x migration documentation

---

### Phase 2: Automated Testing

**Objective:** Execute all automated tests to validate behavioral compatibility

**Test Project:** NCS.DSS.Customer.Tests

#### Test Execution

**Run All Tests:**
```bash
dotnet test NCS.DSS.Customer.Tests\NCS.DSS.Customer.Tests.csproj --configuration Release --logger "console;verbosity=detailed"
```

**Expected Results:**
- All tests discovered by NUnit runner
- 100% test pass rate
- No test framework errors (NUnit, Moq)

**Alternative Test Execution:**
- Visual Studio Test Explorer: Run All Tests
- `func` CLI: Tests execute via test runner integration

#### Test Analysis

**If Tests Pass (100%):**
- ? Proceed to local integration testing
- ? Document test run results
- ? Capture test coverage metrics (if available)

**If Tests Fail:**

1. **Categorize Failures:**
   - **URI-related failures** ? System.Uri behavioral change
   - **DI/Hosting failures** ? HostBuilder behavioral change
   - **Azure Functions failures** ? Worker 2.x breaking change
   - **Mock failures** ? Moq compatibility issue (unlikely)

2. **Analyze Root Cause:**
   - Check test assertion expectations vs actual behavior
   - Review error messages and stack traces
   - Compare against Breaking Changes Catalog

3. **Remediation Options:**
   - **Update test assertions** if behavioral change is expected/acceptable
   - **Fix application code** if behavior is unintended
   - **Update mocks** if Azure Functions Worker 2.x changed interfaces

4. **Retest After Fixes:**
   - Run specific failing tests: `dotnet test --filter "FullyQualifiedName~TestName"`
   - Verify fixes don't break other tests
   - Re-run full suite to confirm 100% pass rate

#### Test Coverage Validation

**Key Test Areas:**
- [ ] **HTTP Trigger Tests:** All HTTP function tests pass
- [ ] **Cosmos DB Trigger Tests:** All Cosmos trigger tests pass
- [ ] **Service Bus Tests:** All Service Bus trigger tests pass
- [ ] **Dependency Injection Tests:** DI container resolution tests pass
- [ ] **URI Handling Tests:** URL parsing/construction tests pass
- [ ] **Configuration Tests:** Settings/configuration binding tests pass
- [ ] **Mocking Tests:** Moq-based tests execute correctly

---

### Phase 3: Local Integration Testing

**Objective:** Manually validate Azure Functions execute correctly in local runtime

#### Azure Functions Local Runtime

**Prerequisites:**
- Azure Functions Core Tools v4 installed
- Local storage emulator or Azure storage connection (if needed)
- Sample test data for triggers

**Start Functions Locally:**
```bash
cd NCS.DSS.Customer
func start
```

**Expected Behavior:**
- Functions host initializes without errors
- All functions discovered and loaded
- HTTP triggers listening on local endpoints
- Application Insights connection established (if configured)
- No startup exceptions in console output

#### Manual Test Scenarios

##### 1. HTTP Trigger Validation

**For Each HTTP-Triggered Function:**

- [ ] **Basic Request:** Send simple GET/POST request
  - Use Postman, curl, or browser
  - Verify response status code
  - Check response body format

- [ ] **URI Edge Cases:** Test URI parsing with:
  - URLs with query parameters
  - URLs with special characters
  - Relative vs absolute paths
  - Malformed URLs (should handle gracefully)

- [ ] **Authentication:** Test auth flows (if using Microsoft.Identity.Client)
  - Verify token validation works
  - Check authorization policies

**Example:**
```bash
curl -X GET "http://localhost:7071/api/MyFunction?param=value"
```

##### 2. Cosmos DB Trigger Validation

**Test Scenario:**
- [ ] Insert/update document in Cosmos DB container
- [ ] Verify trigger fires within expected timeframe
- [ ] Check function processes document correctly
- [ ] Validate output/side effects

**Monitor:**
- Console output for trigger execution logs
- Application Insights for trigger telemetry

##### 3. Service Bus Trigger Validation

**Test Scenario:**
- [ ] Send test message to Service Bus queue/topic
- [ ] Verify trigger processes message
- [ ] Check message handling logic
- [ ] Validate message completion/abandonment

**Tools:**
- Azure Portal (Service Bus Explorer)
- Azure Service Bus SDK test scripts

##### 4. Dependency Injection Validation

**Verify:**
- [ ] All services registered in DI container resolve correctly
- [ ] Scoped services behave as expected
- [ ] Singleton services maintain state correctly
- [ ] Transient services create new instances

**Test Method:**
- Add logging to service constructors
- Verify service instantiation in function execution
- Check for DI-related exceptions

##### 5. Configuration Validation

**Verify:**
- [ ] `host.json` settings apply correctly
- [ ] `local.settings.json` values load
- [ ] Environment variables accessible
- [ ] Connection strings resolve

**Check:**
- Function timeout settings
- Retry policies
- Logging levels
- Application Insights instrumentation key

##### 6. Application Insights Validation

**Verify:**
- [ ] Telemetry flows to Application Insights
- [ ] Custom telemetry (if any) still works
- [ ] Request tracking functions correctly
- [ ] Dependency tracking captures Azure service calls
- [ ] Exception tracking captures errors

**Tools:**
- Application Insights Live Metrics
- Azure Portal Application Insights query explorer

---

### Phase 4: Smoke Testing (Post-Deployment)

**Objective:** Quick validation after deploying to test/staging environment

**Timeline:** Execute immediately after deployment

#### Critical Path Tests

**Priority 1 (Must Verify):**
- [ ] All HTTP endpoints return 200 OK (or expected status)
- [ ] Cosmos DB trigger processes new documents
- [ ] Service Bus trigger processes messages
- [ ] Application Insights receives telemetry
- [ ] No startup/initialization errors

**Priority 2 (Should Verify):**
- [ ] Authentication flows work correctly
- [ ] Database read/write operations succeed
- [ ] External API calls function correctly
- [ ] Response times within acceptable range

**Priority 3 (Nice to Verify):**
- [ ] Logging verbosity appropriate
- [ ] No unexpected warnings in logs
- [ ] Memory/CPU usage within normal range

#### Monitoring Checklist

**24-Hour Post-Deployment:**
- [ ] Monitor Application Insights for:
  - Exception rate (should be zero or low)
  - Request success rate (should be >99%)
  - Dependency failures (should be minimal)
  - Average response times (should be comparable to pre-upgrade)
- [ ] Check Azure Functions execution logs for errors
- [ ] Review any alerts triggered

**1-Week Post-Deployment:**
- [ ] No regression in performance metrics
- [ ] No increase in error rates
- [ ] No unexpected behavioral changes reported
- [ ] Authentication/authorization working correctly (if using Microsoft.Identity.Client)

---

### Comprehensive Validation Checklist

**Before Marking Upgrade Complete:**

#### Build & Compilation
- [ ] Solution builds with 0 errors on .NET 10
- [ ] Zero warnings (or only documented acceptable warnings)
- [ ] All NuGet packages restore successfully
- [ ] No package version conflicts

#### Automated Tests
- [ ] All NUnit tests pass (100% pass rate)
- [ ] No test framework errors
- [ ] Test coverage maintained or improved

#### Local Runtime
- [ ] Azure Functions host starts without errors
- [ ] All functions load and execute correctly
- [ ] HTTP triggers respond to requests
- [ ] Cosmos DB trigger processes documents
- [ ] Service Bus trigger processes messages
- [ ] Dependency injection works correctly
- [ ] Configuration loads successfully
- [ ] Application Insights captures telemetry

#### Code Quality
- [ ] No obsolete API usage (except documented deprecated packages)
- [ ] Code follows project standards
- [ ] No new code analysis warnings
- [ ] Maintained or improved code coverage

#### Behavioral Compatibility
- [ ] URI parsing behaves as expected
- [ ] HostBuilder initialization succeeds
- [ ] Azure Functions Worker 2.x changes accommodated
- [ ] No unexpected runtime exceptions
- [ ] Performance within acceptable range

#### Documentation
- [ ] Upgrade changes documented
- [ ] Breaking changes noted
- [ ] Known issues documented
- [ ] Deprecated package (Microsoft.Identity.Client) noted for future action

---

### Rollback Criteria

**Trigger Rollback If:**
- Critical HTTP endpoints fail (>10% error rate)
- Automated tests have <90% pass rate with no clear fix
- Azure Functions runtime fails to initialize
- Data corruption or loss detected
- Performance degradation >50%
- Security vulnerabilities introduced

**Rollback Procedure:**
1. Revert Git commit: `git revert <commit-sha>`
2. Restore NuGet packages to .NET 8 versions
3. Rebuild and redeploy
4. Validate .NET 8 behavior restored
5. Analyze root cause before retry

---

### Testing Tools Reference

**Required:**
- .NET 10 SDK CLI (`dotnet test`, `dotnet build`)
- Azure Functions Core Tools v4 (`func start`)
- NUnit Test Runner (Visual Studio Test Explorer or CLI)

**Recommended:**
- Postman or curl (HTTP endpoint testing)
- Azure Portal (Service Bus, Cosmos DB, Application Insights)
- Azure Storage Explorer (if using storage triggers)
- Application Insights Live Metrics

**Optional:**
- Performance profiling tools
- Code coverage tools (coverlet, dotCover)
- Load testing tools (for performance validation)

---

## Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | Dependencies | Risk | Key Factors |
|---------|-----------|--------------|------|-------------|
| **NCS.DSS.Customer** | ?? Low | 0 projects, 16 packages | ?? Low | Azure Functions Worker package upgrades; 3 behavioral API changes |
| **NCS.DSS.Customer.Tests** | ?? Low | 1 project, 9 packages | ?? Low | Minimal package updates; standard NUnit test project |

### Phase Complexity Assessment

#### Phase 1: Atomic Upgrade

**Complexity:** ?? **Low-Medium**

**Factors:**
- **Project Updates:** Low - 2 simple TargetFramework changes
- **Package Updates:** Medium - 9 packages across both projects, including major version jumps for Azure Functions Worker packages
- **Breaking Changes:** Low - Only 3 behavioral API changes, no binary incompatibilities
- **Build Resolution:** Low - Clear dependency order (Customer ? Tests)

**Relative Effort Distribution:**
- TargetFramework Updates: 10%
- Package Version Updates: 20%
- Dependency Restoration: 10%
- Compilation Error Fixes: 30%
- Verification Build: 10%
- Issue Resolution: 20%

**Dependency Ordering:**
1. NCS.DSS.Customer (must build first)
2. NCS.DSS.Customer.Tests (depends on Customer)

#### Phase 2: Test Validation

**Complexity:** ?? **Low**

**Factors:**
- **Test Execution:** Low - Standard NUnit test runner
- **Test Failure Risk:** Low - No binary-incompatible changes
- **Behavioral Validation:** Medium - Need to verify URI and hosting changes
- **Coverage:** Good - Dedicated test project exists

**Relative Effort Distribution:**
- Test Execution: 30%
- Failure Analysis: 40%
- Test/Code Fixes: 20%
- Re-validation: 10%

### Resource Requirements

#### Skill Levels Required

**Phase 1 (Atomic Upgrade):**
- .NET Framework Migration: Intermediate
- NuGet Package Management: Intermediate
- Azure Functions Worker Knowledge: Intermediate-Advanced (due to v1?v2 migration)
- Build/Compilation Troubleshooting: Intermediate

**Phase 2 (Test Validation):**
- NUnit Testing: Intermediate
- .NET API Behavioral Changes: Intermediate
- Azure Functions Testing: Intermediate

#### Parallel Capacity

**Not Applicable:** All-at-once strategy with 2-project linear dependency chain requires sequential execution. No parallelization opportunities.

**Build Order:**
1. Update all project files and packages
2. Build NCS.DSS.Customer
3. Build NCS.DSS.Customer.Tests
4. Run tests

### Overall Complexity Rating

**Solution-Wide Complexity:** ?? **Low**

**Justification:**
- Small solution (2 projects, 3,803 LOC)
- Straightforward dependency structure
- Well-defined package upgrades
- Minimal breaking changes
- Strong test coverage
- No legacy .NET Framework complexity

**Complexity Drivers:**
1. **Azure Functions Worker v1?v2 Migration** (Medium complexity)
   - Multiple major version package updates
   - Potential host.json configuration changes
   - Function signature/binding changes possible
   
2. **Deprecated Package** (Low complexity)
   - Microsoft.Identity.Client still works on .NET 10
   - No immediate action required
   - Monitor for future replacement

3. **Behavioral API Changes** (Low complexity)
   - System.Uri changes unlikely to affect most code
   - HostBuilder changes may require startup validation
   - Covered by test suite

**Effort Estimate Approach:**

This plan uses **relative complexity ratings** (Low/Medium/High) rather than time estimates because:
- Actual duration depends on developer experience, environment setup, and unforeseen issues
- Complexity ratings focus on technical difficulty and scope
- Enables better prioritization and resource allocation

**Relative Effort Comparison:**
- Phase 1 (Atomic Upgrade): **Medium** effort (package updates + compilation fixes)
- Phase 2 (Test Validation): **Low** effort (standard test execution + minor fixes)

Total relative effort: **Low-Medium** for entire upgrade

---

## Source Control Strategy

### Branching Strategy

**Main Branch:** `develop`  
**Upgrade Branch:** `upgrade-to-NET10` ? (currently active)  
**Target Merge Branch:** `develop`

**Branch Structure:**
```
develop (source branch)
  ??? upgrade-to-NET10 (upgrade work branch) ? current
```

**Approach:** Feature branch workflow with single atomic upgrade commit

---

### All-At-Once Source Control Approach

**Recommended Strategy:** **Single Atomic Commit**

**Rationale:**
- All-at-once upgrade means all changes are interdependent
- Single commit makes rollback trivial (`git revert`)
- No intermediate broken states in history
- Clear upgrade boundary for future reference

**Commit Structure:**

```
git add .
git commit -m "Upgrade solution to .NET 10

- Update both projects to net10.0 target framework
- Upgrade 9 NuGet packages to .NET 10-compatible versions
- Azure Functions Worker 1.x ? 2.x migration
- All tests passing
- Builds with 0 errors

Breaking changes addressed:
- System.Uri behavioral changes validated
- HostBuilder changes verified
- Azure Functions Worker 2.x migration complete

Packages updated:
- Microsoft.Azure.Functions.Worker: 1.22.0 ? 2.51.0
- Microsoft.Azure.Functions.Worker.Sdk: 1.17.4 ? 2.0.7
- Microsoft.Azure.Functions.Worker.ApplicationInsights: 2.0.0 ? 2.50.0
- Microsoft.Azure.Functions.Worker.Extensions.Http: 3.2.0 ? 3.3.0
- Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore: 1.3.2 ? 2.1.0
- Microsoft.Azure.Functions.Worker.Extensions.CosmosDB: 3.0.9 ? 4.14.0
- Microsoft.ApplicationInsights.WorkerService: 2.22.0 ? 2.23.0
- Microsoft.Extensions.DependencyInjection: 9.0.0 ? 10.0.2

Known issues:
- Microsoft.Identity.Client 4.61.3 is deprecated but compatible; plan future migration"
```

**Commit Guidelines:**
- ? Descriptive commit message with upgrade summary
- ? List key package version changes
- ? Note breaking changes addressed
- ? Document known issues/deprecations
- ? Include validation status (tests passing, builds clean)

---

### Alternative: Checkpoint Commits (If Needed)

**Use checkpoint commits only if:**
- Upgrade process is interrupted (e.g., end of day, blocking issue found)
- Need to preserve intermediate progress
- Long compilation fix process requires iterative commits

**Checkpoint Structure:**

```bash
# Checkpoint 1: Project files updated
git add *.csproj
git commit -m "WIP: Update TargetFramework to net10.0 in both projects"

# Checkpoint 2: Packages updated
git add *.csproj
git commit -m "WIP: Update NuGet packages to .NET 10 versions"

# Checkpoint 3: Compilation errors fixed
git add NCS.DSS.Customer/**/*.cs
git commit -m "WIP: Fix compilation errors from Azure Functions Worker 2.x"

# Final: Tests passing
git add NCS.DSS.Customer.Tests/**/*.cs
git commit -m "Fix: Update tests for .NET 10 behavioral changes"

# Squash before merge (if using checkpoints)
git rebase -i develop  # Squash all WIP commits into single commit
```

**Important:** If using checkpoint commits, **squash before merging** to maintain clean history.

---

### Commit Frequency

**Recommended:** Single commit for entire upgrade (preferred for All-At-Once)

**Alternative Approaches:**

| Approach | When to Use | Pros | Cons |
|----------|-------------|------|------|
| **Single Atomic Commit** | Small upgrades, clear process | Clean history, easy rollback, clear boundary | Loses intermediate progress if interrupted |
| **Checkpoint Commits (Squashed)** | Long/complex upgrades, multiple sessions | Preserves work-in-progress, flexible | Requires squash before merge, more complex |
| **Per-Phase Commits** | Very complex upgrades with distinct phases | Clear phase boundaries, partial rollback | Not recommended for 2-project solution |

**For This Upgrade:** Use **Single Atomic Commit** (recommended)

---

### Commit Message Format

**Template:**

```
Upgrade solution to .NET 10

Summary:
- [Brief description of changes]
- [Key migrations performed]

Projects Updated:
- NCS.DSS.Customer: net8.0 ? net10.0
- NCS.DSS.Customer.Tests: net8.0 ? net10.0

Packages Updated:
- [Package 1]: [old] ? [new]
- [Package 2]: [old] ? [new]
...

Breaking Changes Addressed:
- [Change 1 and mitigation]
- [Change 2 and mitigation]

Validation:
- ? All tests passing
- ? Builds with 0 errors
- ? Local Functions runtime validated

Known Issues:
- [Any deferred issues or deprecation warnings]
```

---

### Review and Merge Process

#### Pull Request Requirements

**Title:** `Upgrade NCS.DSS.Customer solution to .NET 10`

**Description:**

```markdown
## Overview
This PR upgrades the NCS.DSS.Customer Azure Functions solution from .NET 8 to .NET 10 (LTS).

## Changes
- **Projects:** 2 projects upgraded to net10.0
- **Packages:** 9 packages updated to .NET 10-compatible versions
- **Breaking Changes:** 3 behavioral API changes addressed
- **Migration:** Azure Functions Worker 1.x ? 2.x

## Testing
- ? All NUnit tests passing (100% pass rate)
- ? Solution builds with 0 errors
- ? Azure Functions tested locally (all triggers working)
- ? Application Insights telemetry validated

## Breaking Changes
- **System.Uri:** Behavioral changes validated via tests
- **HostBuilder:** Functions host initialization verified
- **Azure Functions Worker 2.x:** All function signatures compatible

## Package Updates
[See plan.md Package Update Reference section for complete list]

Key updates:
- Microsoft.Azure.Functions.Worker: 1.22.0 ? 2.51.0
- Microsoft.Azure.Functions.Worker.Sdk: 1.17.4 ? 2.0.7
- Microsoft.Extensions.DependencyInjection: 9.0.0 ? 10.0.2

## Known Issues / Future Work
- **Microsoft.Identity.Client** is deprecated but compatible; plan future migration to Microsoft.Identity.Web or Azure.Identity

## Rollback Plan
Single atomic commit enables simple rollback: `git revert <commit-sha>`

## Checklist
- [ ] Code builds without errors
- [ ] All tests pass
- [ ] Breaking changes documented
- [ ] Known issues documented
- [ ] Tested locally with Azure Functions runtime
```

#### PR Review Checklist

**Reviewers Should Verify:**
- [ ] `.csproj` files correctly target net10.0
- [ ] All package versions match plan.md specifications
- [ ] No unintended package downgrades
- [ ] Commit message is descriptive
- [ ] Tests pass in CI/CD pipeline (if configured)
- [ ] No new compiler warnings introduced
- [ ] Breaking changes mitigations are sound

#### Merge Criteria

**Merge to `develop` when:**
- ? All automated tests pass (NUnit suite)
- ? Code review approved by at least 1 reviewer
- ? Solution builds successfully with 0 errors
- ? Breaking changes documented and addressed
- ? Known issues/deprecations documented
- ? Local runtime validation complete

**Merge Method:** Squash and merge (if using checkpoint commits) or standard merge (if single commit)

---

### Post-Merge Actions

**After Merge to `develop`:**

1. **Tag Release (Optional):**
   ```bash
   git tag -a v1.0-net10 -m "Upgraded to .NET 10"
   git push origin v1.0-net10
   ```

2. **Update Documentation:**
   - Update README.md with .NET 10 requirement
   - Update deployment guides with new SDK requirement
   - Document any new Azure Functions Worker 2.x behaviors

3. **Notify Team:**
   - Announce .NET 10 upgrade completion
   - Share known issues (Microsoft.Identity.Client deprecation)
   - Provide migration timeline if needed

4. **CI/CD Pipeline:**
   - Update build agents to .NET 10 SDK (if needed)
   - Verify automated deployment pipeline works
   - Test staging deployment

5. **Delete Upgrade Branch:**
   ```bash
   git branch -d upgrade-to-NET10
   git push origin --delete upgrade-to-NET10
   ```

---

### Rollback Procedure

**If Critical Issues Found After Merge:**

#### Option 1: Revert Commit (Recommended)

```bash
# Identify upgrade commit SHA
git log --oneline

# Revert the upgrade commit
git revert <upgrade-commit-sha>

# Push revert
git push origin develop
```

**Effect:** Creates new commit that undoes upgrade; preserves history

#### Option 2: Hard Reset (Emergency Only)

```bash
# DANGER: Only if revert not possible
git reset --hard <commit-before-upgrade>
git push --force origin develop
```

**?? Warning:** Force push affects all team members; coordinate carefully

#### Post-Rollback Actions

1. **Analyze Root Cause:**
   - What issue triggered rollback?
   - Was it missed in testing?
   - What additional validation needed?

2. **Update Plan:**
   - Document discovered issues in plan.md
   - Add specific test scenarios
   - Adjust migration approach if needed

3. **Retry Upgrade:**
   - Create new branch: `upgrade-to-NET10-v2`
   - Apply lessons learned
   - Re-execute upgrade with enhanced validation

---

### Source Control Best Practices Summary

**For All-At-Once .NET 10 Upgrade:**

? **DO:**
- Use single atomic commit (preferred)
- Write detailed commit message with package versions
- Document breaking changes in commit message
- Squash checkpoint commits before merge
- Tag release after successful merge
- Test thoroughly before committing

? **DON'T:**
- Commit broken/non-building code
- Leave WIP commits unsquashed in history
- Force push without team coordination
- Merge without test validation
- Forget to document deprecated packages

**Goal:** Clean, revertible, well-documented upgrade in source control history

---

## Success Criteria

### Technical Criteria

**The migration is complete when all of the following are true:**

#### 1. Framework Upgrade
- [x] **All Projects Upgraded:** Both projects target net10.0
  - ? NCS.DSS.Customer.csproj: `<TargetFramework>net10.0</TargetFramework>`
  - ? NCS.DSS.Customer.Tests.csproj: `<TargetFramework>net10.0</TargetFramework>`
- [x] **No Mixed Framework Versions:** All projects on same .NET version
- [x] **.NET 10 SDK Verified:** Development machines have .NET 10 SDK installed

#### 2. Package Updates
- [x] **All Required Packages Updated:** 9 packages upgraded to target versions
  - ? Microsoft.Azure.Functions.Worker: 2.51.0
  - ? Microsoft.Azure.Functions.Worker.Sdk: 2.0.7
  - ? Microsoft.Azure.Functions.Worker.ApplicationInsights: 2.50.0
  - ? Microsoft.Azure.Functions.Worker.Extensions.Http: 3.3.0
  - ? Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore: 2.1.0
  - ? Microsoft.Azure.Functions.Worker.Extensions.CosmosDB: 4.14.0
  - ? Microsoft.ApplicationInsights.WorkerService: 2.23.0
  - ? Microsoft.Extensions.DependencyInjection: 10.0.2 (both projects)
- [x] **No Package Conflicts:** `dotnet list package --include-transitive` shows no version conflicts
- [x] **Compatible Packages Unchanged:** 13 packages remain at current versions (as intended)

#### 3. Build Success
- [x] **Zero Compilation Errors:** `dotnet build NCS.DSS.Customer.sln` succeeds with 0 errors
- [x] **Zero Warnings (or Documented):** Build produces 0 warnings or only acceptable warnings documented
- [x] **Release Configuration Builds:** Both Debug and Release configurations build successfully
- [x] **All Projects Build:** Both NCS.DSS.Customer and NCS.DSS.Customer.Tests build independently and together

#### 4. Automated Testing
- [x] **All Tests Pass:** NUnit test suite achieves 100% pass rate
- [x] **No Test Framework Errors:** NUnit, Moq, and test adapter work correctly on .NET 10
- [x] **Test Discovery Works:** All tests discovered by test runner
- [x] **Test Coverage Maintained:** Code coverage maintained or improved from .NET 8 baseline

#### 5. Runtime Validation
- [x] **Azure Functions Host Starts:** `func start` initializes without errors
- [x] **All Functions Load:** Azure Functions runtime discovers and loads all function definitions
- [x] **HTTP Triggers Respond:** All HTTP-triggered functions respond to requests
- [x] **Cosmos DB Trigger Works:** Cosmos DB trigger processes document changes
- [x] **Service Bus Trigger Works:** Service Bus trigger processes messages
- [x] **No Startup Exceptions:** Functions host initializes without exceptions in logs

#### 6. Dependency Injection
- [x] **DI Container Builds:** Service provider initializes without errors
- [x] **All Services Resolve:** All registered services resolve correctly
- [x] **Scoped Services Work:** Scoped services behave as expected per request
- [x] **Singleton Services Work:** Singleton services maintain state correctly

#### 7. Configuration
- [x] **Configuration Loads:** `host.json` and `local.settings.json` load successfully
- [x] **Connection Strings Resolve:** Azure service connection strings accessible
- [x] **Environment Variables Work:** Configuration bindings function correctly
- [x] **Application Insights Configured:** Instrumentation key/connection string configured

#### 8. Behavioral Compatibility
- [x] **System.Uri Changes Validated:** URI parsing behaves as expected (no regressions)
- [x] **HostBuilder Changes Verified:** Functions host initialization succeeds
- [x] **Azure Functions Worker 2.x Compatible:** Function signatures, bindings, and middleware work correctly
- [x] **No Unexpected Runtime Exceptions:** Application Insights shows no new exception patterns

#### 9. Security & Vulnerabilities
- [x] **No New Vulnerabilities:** Package updates don't introduce security issues
- [x] **Deprecated Package Acknowledged:** Microsoft.Identity.Client deprecation documented for future action
- [x] **Authentication Works:** Identity/auth flows function correctly (if applicable)

#### 10. Performance
- [x] **Performance Acceptable:** No significant performance degradation (>20% regression)
- [x] **Cold Start Time Reasonable:** Azure Functions cold start within acceptable range
- [x] **Memory Usage Normal:** No memory leaks or excessive memory consumption
- [x] **Response Times Maintained:** HTTP trigger response times comparable to .NET 8

#### 11. Telemetry & Logging
- [x] **Application Insights Receives Telemetry:** Requests, dependencies, exceptions logged
- [x] **Custom Telemetry Works:** Any custom telemetry processors function correctly
- [x] **Logging Levels Correct:** Logging configuration produces appropriate output
- [x] **No Telemetry Gaps:** All expected telemetry types (requests, dependencies, traces) flowing

---

### Quality Criteria

**Code quality and project standards maintained:**

#### 1. Code Quality
- [x] **No Code Analysis Warnings:** Static code analysis shows no new warnings
- [x] **No Obsolete API Usage:** Code doesn't use obsolete APIs (except documented deprecated packages)
- [x] **Code Style Maintained:** Code follows project conventions and style guidelines
- [x] **No Technical Debt Added:** Upgrade doesn't introduce workarounds or hacks

#### 2. Documentation
- [x] **Upgrade Documented:** Changes documented in commit message and plan.md
- [x] **Breaking Changes Noted:** All behavioral changes documented
- [x] **Known Issues Documented:** Deprecated package and any issues noted
- [x] **Migration Path Clear:** Plan provides clear steps for future reference

#### 3. Test Quality
- [x] **Test Coverage Maintained:** Code coverage percentage maintained or improved
- [x] **Tests Updated Appropriately:** Tests reflect .NET 10 behavioral changes
- [x] **No Flaky Tests:** Tests pass consistently (not intermittent)
- [x] **Test Assertions Meaningful:** Test assertions validate correct behavior

---

### Process Criteria

**Upgrade process followed correctly:**

#### 1. All-At-Once Strategy Followed
- [x] **Both Projects Upgraded Simultaneously:** No incremental multi-targeting approach used
- [x] **Atomic Package Updates:** All 9 package updates applied in single operation
- [x] **Single Upgrade Phase:** No intermediate .NET versions (direct 8?10)
- [x] **Dependency Order Respected:** NCS.DSS.Customer built before NCS.DSS.Customer.Tests

#### 2. Source Control Process
- [x] **Correct Branch Used:** upgrade-to-NET10 branch created and used
- [x] **Commit Strategy Followed:** Single atomic commit (or checkpoint commits squashed)
- [x] **Descriptive Commit Message:** Commit includes package versions and breaking changes
- [x] **Pull Request Created:** PR created with appropriate description and checklist

#### 3. All-At-Once Strategy Principles Applied
- [x] **No Partial Upgrades:** Both projects upgraded together (not sequentially)
- [x] **Single Validation Phase:** Testing performed after complete upgrade (not per-project)
- [x] **Atomic Commit:** Single commit or squashed commits for entire upgrade
- [x] **No Intermediate States:** Solution doesn't exist in half-upgraded state

---

### Acceptance Criteria Checklist

**Final verification before declaring upgrade complete:**

**Phase 1: Atomic Upgrade Complete**
- [ ] Both .csproj files updated to net10.0
- [ ] All 9 package updates applied with correct versions
- [ ] Solution builds with 0 errors
- [ ] No package version conflicts
- [ ] All compilation errors resolved

**Phase 2: Test Validation Complete**
- [ ] All NUnit tests pass (100% pass rate)
- [ ] Test failures analyzed and resolved
- [ ] No test framework compatibility issues

**Phase 3: Runtime Validation Complete**
- [ ] Azure Functions host starts locally
- [ ] All HTTP triggers tested manually
- [ ] Cosmos DB trigger tested
- [ ] Service Bus trigger tested
- [ ] Application Insights receiving telemetry
- [ ] No startup or runtime exceptions

**Phase 4: Quality Gates Passed**
- [ ] No new code analysis warnings
- [ ] Performance within acceptable range
- [ ] Security vulnerabilities addressed
- [ ] Documentation updated

**Phase 5: Source Control Complete**
- [ ] Single atomic commit created (or checkpoints squashed)
- [ ] Descriptive commit message with package versions
- [ ] Pull request created and reviewed
- [ ] Breaking changes and known issues documented

---

### Known Acceptable Deviations

**The following are acceptable and don't block upgrade completion:**

1. **Microsoft.Identity.Client Deprecation:**
   - Package marked as deprecated but still compatible with .NET 10
   - No replacement action required immediately
   - Future migration planned for subsequent release

2. **Minor Performance Variations:**
   - Cold start time may differ slightly (±10%) due to .NET runtime changes
   - Small variations in memory usage acceptable if within normal range

3. **Non-Breaking Warnings:**
   - Informational warnings about deprecated features (if documented)
   - Nullable reference type warnings (if project doesn't enforce nullable context)

---

### Post-Upgrade Monitoring

**After upgrade is deemed complete, monitor for 1 week:**

#### Daily Checks (First 3 Days)
- [ ] Application Insights exception count (should be low/zero)
- [ ] Function execution success rate (should be >99%)
- [ ] Average response times (should match .NET 8 baseline)
- [ ] Cold start times (should be comparable)

#### Weekly Check (Day 7)
- [ ] No regression in key metrics
- [ ] No new bug reports related to upgrade
- [ ] Team confirms no unexpected behavior
- [ ] Authentication/authorization working correctly (if using Microsoft.Identity.Client)

#### Final Sign-Off

**Upgrade is fully successful when:**
- All technical criteria met ?
- All quality criteria met ?
- All process criteria met ?
- 1-week monitoring period complete with no critical issues ?
- Team sign-off obtained ?

---

### Rollback Criteria (When Success Criteria NOT Met)

**Trigger rollback if:**
- [ ] Solution doesn't build after package updates
- [ ] >10% of tests fail with no clear resolution path
- [ ] Azure Functions runtime fails to initialize
- [ ] Critical functionality broken (HTTP endpoints, triggers)
- [ ] Performance degradation >50%
- [ ] Security vulnerabilities introduced
- [ ] Data corruption detected

**Rollback Process:** See "Source Control Strategy" section for revert procedure.

---

### Success Declaration

**Official Upgrade Completion:**

When all checkboxes in "Acceptance Criteria Checklist" are marked:

? **The .NET 10 upgrade is complete and successful.**

**Next Steps:**
1. Merge pull request to `develop`
2. Tag release (optional): `v1.0-net10`
3. Update team documentation
4. Plan future work (Microsoft.Identity.Client migration)
5. Celebrate! ??
