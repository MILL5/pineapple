# Project installation

Vendored from the existing `/Users/rich/.agents/skills/impeccable` installation on 2026-11-02: skill version 4.8.0, pinned engine 0.1.18. The skill, references, shipped agent definitions, launcher scripts, browser support, and data files are included. Native binaries and machine state are excluded.

The portable launcher uses an installed compatible engine or obtains its pinned platform release and verifies it against `scripts/engine-checksums.txt`. Native engines remain in the user cache or ignored `scripts/bin/`; they are not source artifacts. `scripts/impeccable --version` returned `4.8.0` during installation verification.

Project product context is `PRODUCT.md`; visual tokens are `DESIGN.md`, extended by `.impeccable/design.json`. Existing architecture and Goldfish review records remain separate.
