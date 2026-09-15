# Changelog

All notable changes to this workspace will be documented here. The format is
based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this
project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed - 2026-08-19 - Workspace Reorganization

**Scope:** Full restructure of the workspace into a layered, single-responsibility layout. Source code, docs, planning, docker, reports, tests, deployment, archive and examples each now have a clearly bounded home.

Highlights:

- **Source code** moved from `PlanTrienKhaiVaCode/03.BACKEND/` and `04.FRONTEND/` to canonical `src/backend/` and `src/frontend/`.
- **Docker** consolidated into a single canonical tree at `/docker/` with `aibf_` prefix; duplicate tree under `PlanTrienKhaiVaCode/02.INFRASTRUCTURE/` removed.
- **Reports** consolidated into `reports/01.ThesisMaster_DeAn/`, `reports/02.ThesisInternship_ThucTap/`, `reports/03.ThesisTKTC_AI_BTC/`.
- **Report sources and AI-generated helpers** separated into `chapters/`, `outputs/`, `tools/`, `verify_scripts/`, and `references/`.
- **Archive** at `archive/` holds previous DOCX versions, stray Python tooling, and a reference PDF from another student.
- **Tests** skeleton created (folder layout only; no test cases yet).
- **Deployment** skeleton created (Kubernetes overlays + Helm placeholder).
- **Shared** folder created for cross-cutting concerns (api-contracts).

### Added

- `.gitignore`, `.editorconfig`, `.dockerignore` at root.
- `AGENTS.md` and concise root `README.md` as stable entry points for people and AI agents.
- `docs/00.INDEX.md` (master docs index).
- `docs/12.AUDIT/` for project health audit reports (formerly `DIEM_BAT_HOP_LY.md` at root).
- `docs/13.AI_PLATFORM_VISION/` for the long-form vision / requirements document (formerly `YeuCau.txt` at root).
- `docs/enterprise-ai-platform/` consolidated from `PlanTrienKhaiVaCode/05.DOCUMENTATION/` and `06.ENTERPRISE_AI_PLATFORM/`.
- `planning/` directory replacing `PlanTrienKhaiVaCode/`.
- `examples/documents/` placeholder for sample RAG fixtures.

### Fixed

- `docker/init.sql` — duplicate `IF NOT EXISTS IF NOT EXISTS "vector"` syntax error.
- Docker container/project naming unified to `aibf_` prefix.
- `docker-compose.yml` paths corrected to new `src/` layout.
- `MinIOConfig` env binding casing made consistent.

### Notes

- This is a **structural** change. The known compile errors in the .NET backend
  (see [docs/12.AUDIT/DIEM_BAT_HOP_LY.md](docs/12.AUDIT/DIEM_BAT_HOP_LY.md)) are
  out of scope here and remain on the backlog.
- Git rename detection was preserved by using `git mv` for whole-folder moves.
- Working branch: `reorganize/2026-08-structure`.

[Unreleased]: https://example.invalid/doanthacsi/compare/main...reorganize/2026-08-structure
