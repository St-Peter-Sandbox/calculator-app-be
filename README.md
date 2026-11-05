# calculator-app-backend

Created by ALPHACI as a .NET service standalone repository.

This starter already includes source files, package scripts, TypeScript, ESLint, Jest coverage, SonarQube metadata, branch protections, and ALPHACI workflow files that match the selected stack. Use it as the first working baseline, then replace the starter code with your application code.

## Project structure

- `src/<Project>/Program.cs` boots the minimal API and exposes `/health`, which the production gate probes after each deployment.
- `tests/<Project>.Tests/` hosts the application in memory with `WebApplicationFactory`, so coverage reflects code that actually runs.
- `.editorconfig` is what `dotnet format` checks against; the pipeline runs it at warning severity on uat and main.
- `global.json` pins the SDK so local builds and CI resolve the same version.

## Branch strategy

| Branch  | Purpose |
|---------|---------|
| main    | Production - pull request required |
| uat     | Integration and test - pull request required |
| dev     | Development integration - pull request required |

Nobody pushes to `dev`, `uat` or `main` directly: open a pull request from a feature branch into `dev`, and promote `dev` to `uat` with a pull request. `main` is reached only through the QA-approved merge in ALPHACI.

## CI/CD

Workflow files live in `.github/workflows/`. The CI pipeline runs on `dev`, `uat` and `main` and on pull requests into them. Open a pull request into `dev` to trigger your first run.

## Getting started

```bash
dotnet restore
dotnet format --verify-no-changes
dotnet build --configuration Release --no-restore
dotnet test --collect:"XPlat Code Coverage"
```

Create a feature branch, open a pull request into `dev`, and let ALPHACI promote green changes through `uat` to `main`.

## API quiz

Check how well you know the calculator API. Open a question to see the answer.

<details>
<summary>1. You send POST /api/calculate with { "operand1": 5, "operand2": 3, "operation": "add" }. What comes back?</summary>

**HTTP 200** with `result: 8`, `isSuccess: true` and `equation: "5 + 3 = 8"`. The calculation is also added to the history.

</details>

<details>
<summary>2. What happens when you divide 20 by 0?</summary>

**HTTP 400** with `isSuccess: false` and `errorMessage: "Cannot divide by zero"`. The failed calculation is left out of the history.

</details>

<details>
<summary>3. Which values does the operation field accept?</summary>

Names or symbols: `add`, `+`, `plus`; `subtract`, `-`, `minus`; `multiply`, `*`, `x`, `×`, `times`; `divide`, `/`, `÷`. Anything else returns HTTP 400 with *Unsupported operation*.

</details>

<details>
<summary>4. In what order does GET /api/history return calculations?</summary>

**Newest first.** Each entry has an `id`, the `equation`, the `result` and a UTC `timestamp`.

</details>

<details>
<summary>5. What does DELETE /api/history do, and what does it return?</summary>

It clears every saved calculation and returns **HTTP 204 No Content**.

</details>

<details>
<summary>6. Does the history survive a restart?</summary>

**No.** The history lives in memory inside the running service, so a restart or a new deployment starts with an empty list.

</details>

<details>
<summary>7. What does GET /health answer, and who relies on it?</summary>

`{ "status": "ok", "service": "calculator-app-backend" }`. The production gate probes it after every deployment, so it has to stay fast and dependency free.

</details>