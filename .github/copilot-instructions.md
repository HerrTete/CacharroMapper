# Versioning policy (CacharroMapper)

We use SemVer: MAJOR.MINOR.PATCH.

## Patch bump rule
If a change is going to be merged into `main` and it changes **production source code** under `src/`, then the agent MUST increment the PATCH version (the last number) by +1.

### What counts as "production source"
- Any changes under: `src/**`
- Except: tests and non-production assets (do not trigger a version bump):
  - `src/**/Tests/**`
  - `src/**/*.Tests.csproj`
  - `src/**/*Test*/**` (common test folder patterns)

### Where the version is stored
- The version is stored in the `.csproj` file(s) using one of these elements:
  - `<Version>MAJOR.MINOR.PATCH</Version>` (preferred)
  - or `<AssemblyVersion>...</AssemblyVersion>` / `<FileVersion>...</FileVersion>` if that is what the project uses.

### How to bump
- Only bump PATCH: e.g. `1.4.12` -> `1.4.13`
- Do NOT change MAJOR or MINOR unless explicitly requested.
- The version bump must be part of the same change/PR that modifies production source.

### PR description
- If bump applied: include `Version bump: patch`
- If no bump needed: include `No version bump (no production source change)`
