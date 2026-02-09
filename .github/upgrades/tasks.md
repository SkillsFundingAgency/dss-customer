# .NET 10 Upgrade Execution Tasks

**Solution:** NCS.DSS.Customer  
**Strategy:** All-At-Once Atomic Upgrade  
**Target Framework:** net10.0  
**Status:** Ready for Execution

---

## Progress Dashboard

**Overall Progress:** 1/12 tasks complete (8%) ![8%](https://progress-bar.xyz/8)

### Phase Summary
- **Phase 0: Prerequisites** - 1/2 tasks complete
- **Phase 1: Atomic Upgrade** - 0/6 tasks complete
- **Phase 2: Test Validation** - 0/2 tasks complete
- **Phase 3: Final Validation** - 0/2 tasks complete

**Last Updated:** Not started  
**Current Task:** TASK-001

---

## Task List

### Phase 0: Prerequisites

#### [?] TASK-001: Verify .NET 10 SDK Installation *(Completed: 2026-02-09 17:04)*
**Priority:** Critical  
**Estimated Effort:** Low

**Actions:**
- [?] (1) Run `dotnet --list-sdks` to verify .NET 10 SDK is installed
        Expected: SDK version 10.0.x appears in list
- [?] (2) If missing, download and install .NET 10 SDK from https://dotnet.microsoft.com/download/dotnet/10.0
- [?] (3) Verify installation by running `dotnet --version`
        Expected: Version 10.0.x or higher

**Validation:**
- [?] .NET 10 SDK appears in `dotnet --list-sdks` output
- [?] `dotnet --version` returns 10.0.x or higher

**References:** plan.md § Prerequisites

---

#### [ ] TASK-002: Verify Branch and Repository State
**Priority:** Critical  
**Estimated Effort:** Low

**Actions:**
- [ ] (1) Verify current branch is `upgrade-to-NET10`
        Run: `git branch --show-current`
        Expected: upgrade-to-NET10
- [ ] (2) Verify working directory is clean (all assessment files committed)
        Run: `git status`
        Expected: No uncommitted changes or only tasks.md untracked
- [ ] (3) If needed, commit tasks.md: `git add .github/upgrades/tasks.md && git commit -m "Add execution tasks for .NET 10 upgrade"`

**Validation:**
- [ ] On correct branch (upgrade-to-NET10)
- [ ] No uncommitted changes (except tasks.md if needed)

**References:** plan.md § Source Control Strategy

---

### Phase 1: Atomic Upgrade

#### [ ] TASK-003: Update NCS.DSS.Customer Project to net10.0
**Priority:** Critical  
**Estimated Effort:** Low  
**Dependencies:** TASK-001, TASK-002

**Actions:**
- [ ] (1) Open `NCS.DSS.Customer\NCS.DSS.Customer.csproj` in editor
- [ ] (2) Locate `<TargetFramework>net8.0</TargetFramework>` element
- [ ] (3) Change to `<TargetFramework>net10.0</TargetFramework>`
- [ ] (4) Save file
- [ ] (5) Verify change: Confirm line now reads `<TargetFramework>net10.0</TargetFramework>`

**Validation:**
- [ ] File contains `<TargetFramework>net10.0</TargetFramework>`
- [ ] No other changes made to project file

**References:** plan.md § NCS.DSS.Customer § Technology/Framework Update

---

#### [ ] TASK-004: Update NCS.DSS.Customer.Tests Project to net10.0
**Priority:** Critical  
**Estimated Effort:** Low  
**Dependencies:** TASK-003

**Actions:**
- [ ] (1) Open `NCS.DSS.Customer.Tests\NCS.DSS.Customer.Tests.csproj` in editor
- [ ] (2) Locate `<TargetFramework>net8.0</TargetFramework>` element
- [ ] (3) Change to `<TargetFramework>net10.0</TargetFramework>`
- [ ] (4) Save file
- [ ] (5) Verify change: Confirm line now reads `<TargetFramework>net10.0</TargetFramework>`

**Validation:**
- [ ] File contains `<TargetFramework>net10.0</TargetFramework>`
- [ ] No other changes made to project file

**References:** plan.md § NCS.DSS.Customer.Tests § Technology/Framework Update

---

#### [ ] TASK-005: Update NuGet Packages in NCS.DSS.Customer
**Priority:** Critical  
**Estimated Effort:** Medium  
**Dependencies:** TASK-003

**Actions:**
- [ ] (1) Update Microsoft.Azure.Functions.Worker from 1.22.0 to 2.51.0
- [ ] (2) Update Microsoft.Azure.Functions.Worker.Sdk from 1.17.4 to 2.0.7
- [ ] (3) Update Microsoft.Azure.Functions.Worker.ApplicationInsights from 2.0.0 to 2.50.0
- [ ] (4) Update Microsoft.Azure.Functions.Worker.Extensions.Http from 3.2.0 to 3.3.0
- [ ] (5) Update Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore from 1.3.2 to 2.1.0
- [ ] (6) Update Microsoft.Azure.Functions.Worker.Extensions.CosmosDB from 3.0.9 to 4.14.0
- [ ] (7) Update Microsoft.ApplicationInsights.WorkerService from 2.22.0 to 2.23.0
- [ ] (8) Update Microsoft.Extensions.DependencyInjection from 9.0.0 to 10.0.2
- [ ] (9) Save `NCS.DSS.Customer.csproj` file

**Validation:**
- [ ] All 8 package versions updated correctly in .csproj file
- [ ] No unintended package changes
- [ ] File saved successfully

**References:** plan.md § Package Update Reference

---

#### [ ] TASK-006: Update NuGet Packages in NCS.DSS.Customer.Tests
**Priority:** Critical  
**Estimated Effort:** Low  
**Dependencies:** TASK-004, TASK-005

**Actions:**
- [ ] (1) Update Microsoft.Extensions.DependencyInjection from 9.0.0 to 10.0.2
- [ ] (2) Save `NCS.DSS.Customer.Tests.csproj` file

**Validation:**
- [ ] Microsoft.Extensions.DependencyInjection version is 10.0.2
- [ ] No other package changes
- [ ] File saved successfully

**References:** plan.md § Package Update Reference

---

#### [ ] TASK-007: Restore NuGet Packages and Build Solution
**Priority:** Critical  
**Estimated Effort:** Medium  
**Dependencies:** TASK-005, TASK-006

**Actions:**
- [ ] (1) Restore NuGet packages for entire solution
        Run: `dotnet restore NCS.DSS.Customer.sln`
        Expected: All packages restore successfully without errors
- [ ] (2) Build NCS.DSS.Customer project
        Run: `dotnet build NCS.DSS.Customer\NCS.DSS.Customer.csproj --configuration Release`
        Expected: Build succeeds with 0 errors
- [ ] (3) Build NCS.DSS.Customer.Tests project
        Run: `dotnet build NCS.DSS.Customer.Tests\NCS.DSS.Customer.Tests.csproj --configuration Release`
        Expected: Build succeeds with 0 errors
- [ ] (4) Build entire solution
        Run: `dotnet build NCS.DSS.Customer.sln --configuration Release`
        Expected: Build succeeds with 0 errors across both projects

**Validation:**
- [ ] NuGet restore completed without errors or warnings
- [ ] NCS.DSS.Customer builds with 0 errors
- [ ] NCS.DSS.Customer.Tests builds with 0 errors
- [ ] Full solution build succeeds with 0 errors
- [ ] Build output confirms net10.0 target framework

**If Build Fails:**
- Review errors against Breaking Changes Catalog (plan.md)
- Check for Azure Functions Worker 2.x signature changes
- Verify all package versions are correct
- Consult plan.md § Breaking Changes Catalog for mitigation strategies

**References:** plan.md § Migration Strategy § Execution Sequence

---

#### [ ] TASK-008: Fix Compilation Errors (If Any)
**Priority:** Critical  
**Estimated Effort:** Variable (depends on errors found)  
**Dependencies:** TASK-007

**Condition:** Only execute if TASK-007 build fails

**Actions:**
- [ ] (1) Analyze compilation error messages from TASK-007
- [ ] (2) Categorize errors:
        - Azure Functions Worker 2.x API changes
        - System.Uri behavioral changes
        - HostBuilder changes
        - Package dependency conflicts
- [ ] (3) For each error, consult plan.md § Breaking Changes Catalog for resolution
- [ ] (4) Apply code fixes as needed:
        - Update function signatures for Worker 2.x
        - Update middleware registration if needed
        - Fix any binding type mismatches
        - Update DI service registration if required
- [ ] (5) Rebuild solution after each fix
        Run: `dotnet build NCS.DSS.Customer.sln --configuration Release`
- [ ] (6) Repeat until build succeeds with 0 errors

**Validation:**
- [ ] All compilation errors resolved
- [ ] Solution builds with 0 errors
- [ ] No new warnings introduced (or warnings documented as acceptable)

**References:** 
- plan.md § Breaking Changes Catalog
- plan.md § Azure Functions Worker 2.x Breaking Changes

---

### Phase 2: Test Validation

#### [ ] TASK-009: Run Automated Test Suite
**Priority:** Critical  
**Estimated Effort:** Low  
**Dependencies:** TASK-007 (or TASK-008 if executed)

**Actions:**
- [ ] (1) Execute all NUnit tests
        Run: `dotnet test NCS.DSS.Customer.Tests\NCS.DSS.Customer.Tests.csproj --configuration Release --logger "console;verbosity=detailed"`
        Expected: All tests pass (100% pass rate)
- [ ] (2) Review test results summary
        Note: Total tests, passed, failed, skipped
- [ ] (3) If any tests fail, proceed to TASK-010

**Validation:**
- [ ] All tests discovered by NUnit runner
- [ ] 100% test pass rate (all tests passed, 0 failed)
- [ ] No test framework errors (NUnit, Moq work correctly)

**If Tests Fail:**
- Proceed to TASK-010 for failure analysis and remediation

**References:** plan.md § Testing & Validation Strategy § Phase 2

---

#### [ ] TASK-010: Fix Test Failures (If Any)
**Priority:** Critical  
**Estimated Effort:** Variable (depends on failures)  
**Dependencies:** TASK-009

**Condition:** Only execute if TASK-009 has test failures

**Actions:**
- [ ] (1) Categorize test failures by root cause:
        - URI-related failures (System.Uri behavioral changes)
        - DI/Hosting failures (HostBuilder changes)
        - Azure Functions Worker 2.x changes
        - Mock/test framework issues
- [ ] (2) For each failure category:
        - Review test error messages and stack traces
        - Consult plan.md § Breaking Changes Catalog
        - Determine if issue is in application code or test expectations
- [ ] (3) Apply fixes:
        - Update test assertions if .NET 10 behavior is expected/correct
        - Fix application code if behavior is unintended regression
        - Update mocks if Worker 2.x changed interfaces
- [ ] (4) Re-run tests after each fix:
        Run: `dotnet test NCS.DSS.Customer.Tests --filter "FullyQualifiedName~<TestName>"`
- [ ] (5) Once all fixes applied, run full test suite:
        Run: `dotnet test NCS.DSS.Customer.Tests\NCS.DSS.Customer.Tests.csproj --configuration Release`
- [ ] (6) Verify 100% pass rate achieved

**Validation:**
- [ ] All test failures resolved
- [ ] 100% test pass rate achieved
- [ ] No new test failures introduced by fixes
- [ ] Tests pass consistently (re-run to verify not flaky)

**References:** plan.md § Breaking Changes Catalog

---

### Phase 3: Final Validation

#### [ ] TASK-011: Verify Azure Functions Local Runtime
**Priority:** High  
**Estimated Effort:** Medium  
**Dependencies:** TASK-009 (or TASK-010 if executed)

**Actions:**
- [ ] (1) Start Azure Functions locally
        Run: `cd NCS.DSS.Customer && func start`
        Expected: Functions host initializes without errors
- [ ] (2) Verify startup output:
        - All functions discovered and loaded
        - HTTP triggers listening on local endpoints
        - No startup exceptions in console
        - Application Insights connection established (if configured)
- [ ] (3) Test HTTP trigger endpoints manually:
        - Send GET/POST requests using Postman/curl
        - Verify response status codes
        - Test with query parameters and special characters
- [ ] (4) Test Cosmos DB trigger (if applicable):
        - Insert/update test document in Cosmos DB
        - Verify trigger fires and processes document
- [ ] (5) Test Service Bus trigger (if applicable):
        - Send test message to Service Bus queue/topic
        - Verify trigger processes message
- [ ] (6) Verify Application Insights telemetry:
        - Check Application Insights Live Metrics (if configured)
        - Confirm telemetry flows correctly
- [ ] (7) Stop Functions host (Ctrl+C)

**Validation:**
- [ ] Functions host starts without errors
- [ ] All functions loaded successfully
- [ ] HTTP triggers respond correctly
- [ ] Cosmos DB trigger processes documents (if applicable)
- [ ] Service Bus trigger processes messages (if applicable)
- [ ] No runtime exceptions in console output
- [ ] Application Insights receives telemetry (if configured)

**References:** plan.md § Testing & Validation Strategy § Phase 3

---

#### [ ] TASK-012: Commit Atomic Upgrade
**Priority:** Critical  
**Estimated Effort:** Low  
**Dependencies:** TASK-011

**Actions:**
- [ ] (1) Review all changes:
        Run: `git status`
        Expected: Modified .csproj files, tasks.md, execution_log.md
- [ ] (2) Stage all changes:
        Run: `git add .`
- [ ] (3) Commit with detailed message:
        ```
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
- [ ] (4) Verify commit:
        Run: `git log -1`
        Confirm commit message and files included

**Validation:**
- [ ] All changes committed to upgrade-to-NET10 branch
- [ ] Commit message includes package versions and breaking changes
- [ ] No uncommitted changes remain (except execution_log.md updates)

**References:** plan.md § Source Control Strategy

---

## Execution Notes

### Build Failure Troubleshooting

If TASK-007 build fails, common issues and solutions:

1. **Azure Functions Worker 2.x Signature Changes:**
   - Check function method signatures (HttpRequestData, HttpResponseData types)
   - Update trigger attribute parameters if needed
   - Consult: https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide

2. **Package Version Conflicts:**
   - Run: `dotnet list package --include-transitive`
   - Look for version conflicts in transitive dependencies
   - Ensure all Functions Worker packages are at 2.x versions

3. **Missing Namespaces:**
   - Add using statements for moved types
   - Check if Azure Functions Worker 2.x changed namespaces

### Test Failure Troubleshooting

If TASK-009 tests fail, categorize by:

1. **URI-related failures:** System.Uri behavioral changes
2. **DI/Hosting failures:** HostBuilder initialization differences
3. **Functions Worker failures:** Worker 2.x API changes
4. **Mock failures:** Verify Moq is compatible (should be)

### Success Criteria

Upgrade is complete when:
- [x] All tasks marked complete
- [x] Solution builds with 0 errors
- [x] All tests pass (100% pass rate)
- [x] Azure Functions start and run locally
- [x] All changes committed to git

---

## Rollback Plan

If critical issues arise:

1. **Revert commit:**
   ```bash
   git log --oneline  # Find commit SHA
   git revert <commit-sha>
   ```

2. **Restore to .NET 8:**
   - Packages will be restored to net8.0 versions
   - Rebuild and verify

3. **Analyze root cause before retry**

---

**Ready to begin execution. Please confirm to proceed.**
