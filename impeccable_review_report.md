# Pineapple Impeccable review

- Date: 2026-11-02.
- Target: `docs/`, documentation website; visitor mode: Read.
- Method: two isolated assessments. Assessment A reviewed design and actual rendered pages without seeing detector output. Assessment B independently ran source and live-browser detectors. Findings were combined only after both returned.
- Final verdict: all five material findings resolved in the rendered implementation. The incumbent white/lavender/purple identity is preserved. This is local documentation acceptance, not production deployment or API correctness certification.

## Design specificity and overall impression

Pineapple's fruit mark, concrete .NET utilities, namespaces, package command, and C# examples make the site recognizable as this library. Conventional documentation structure is appropriate to the reading task. The principal weaknesses were narrow-screen API discovery and overly wide prose, rather than a need to rebrand.

The install command supplies an early confidence point. Mobile callout overflow, hidden API metadata, and deprecated material before a reader's intended destination interrupted that path. Supported utilities now lead the homepage and compatibility messaging follows the first successful examples.

## Heuristic baseline

Scores are the original independent Assessment A, on a 0–4 scale; 27/40. The final pass checked the named fixes rather than repeating all ten heuristics, so no final numerical score is claimed.

| Heuristic | Score | Original evidence |
| --- | --- | --- |
| Visibility of status | 3 | Breadcrumbs present; nested guides lost header current state. |
| Match real world | 3 | Developer terminology fits; migration notice preceded installation. |
| Control and freedom | 3 | Standard links and recovery paths; no traps observed. |
| Consistency | 3 | Coherent type/colors; card hover area exceeded link area. |
| Error prevention | 2 | Deprecations labeled in guides but initially advertised without qualification. |
| Recognition over recall | 2 | Flat catalog and missing article jump links. |
| Flexibility and efficiency | 2 | Install command available; no direct jump to supported Parallelism. |
| Minimalist design | 3 | Quiet design; repeated ecosystem/author material lengthens articles. |
| Error recovery | 3 | 404 links to home, guides, and FAQ. |
| Help and documentation | 3 | Concrete examples, FAQ, migration guide; mobile index impeded comparison. |

## Material findings and implemented fixes

| ID | Priority | Finding | Implementation and final disposition |
| --- | --- | --- | --- |
| I1 | P1 | Blockquote margins squeezed mobile callouts; a long API identifier crossed its border. | `docs/assets/css/site.css` resets callout margins and permits inline text to wrap while preserving preformatted code scrolling. Final 390px screenshot confirms the identifier stays inside the warning. Resolved. |
| I2 | P1 | Three-column guide index hid key types offscreen on mobile. | `docs/guides/index.md` is now a semantic list with visible guide, namespace, and key-type metadata. No table panning is needed. Resolved. |
| I3 | P2 | Whole cards changed appearance on hover but only headings were links. | Existing feature tiles are single descriptive anchors with explicit focus outlines; clicking the Parallelism description was verified. Resolved. |
| I4 | P2 | Nested guides lost current navigation state; Parallelism tile landed before deprecated timeout material. | Guides now has ancestor `aria-current="location"`; the tile targets `#parallelism`, and the article has jump links. The target section appeared at the viewport top after navigation. Resolved. |
| I5 | P2 | Migration diagnostics appeared before first installation success and legacy categories dominated introductory copy. | Compatibility notice moved after initial examples; homepage introduction leads with supported utilities and names .NET11 RC. Existing migration and deprecated-API documentation remains accessible. Resolved. |

Additional detector-backed typography fixes reduce the prose measure from 46rem to 68ch, constrain homepage body paragraphs, balance headings, and use pretty paragraph wrapping. The generic generated “Note” eyebrow was removed; warning text itself states the deprecation. Preformatted code remains horizontally scrollable rather than being broken into invalid code lines.

## Detector evidence and false positives

- Broad filesystem scan `detect --json docs/`: 18 warnings. Seventeen `flat-type-hierarchy` findings occurred in generated `docs/_site` pages because the scanner could not resolve the Jekyll stylesheet URL. One `broken-image` finding belonged to a third-party kramdown vendor test fixture. These were false positives, not application findings.
- Narrow source scan `detect --json docs/_layouts/`: empty findings, exit 0. Liquid stylesheet paths remained unresolved, so source color/custom-property coverage was incomplete.
- Live injected detector, three pages: six initial findings — four `line-length`, one `unbalanced-heading`, one `straggler`. Home had two long prose runs; ShortGuid had two long runs, a short heading ending, and a single-word final line. Getting started had no live detector finding. The live findings were measured on desktop; mobile screenshots carried those annotations and are not fresh mobile detector measurements.
- Actual computed body text was 16px, H1 44px desktop and 32px mobile; stylesheet responses were HTTP200, disproving the filesystem hierarchy warnings.
- Following the skill's single-detector review method, acceptance used recaptured pages and targeted checks rather than rerunning the detector until its findings disappeared. No claim of a zero-finding final detector scan is made.

## Cognitive load and persona checks

The initial review identified moderate load from an ungrouped utility catalog and competing ecosystem links. Catalog breadth is useful reference content and was retained. The metadata list makes each guide understandable in place; supported APIs and a separate migration route guide the first-time reader.

- Returning developer: full tile click area and direct Parallelism anchor remove an unnecessary search.
- First-time adopter: first examples precede compatibility diagnostics; modern target status is explicit.
- Mobile reader: complete API metadata and callout text stay within the viewport.

No new customer, benchmark, endorsement, or adoption claim was invented. Existing download/author/ecosystem claims remain incumbent content; this review did not independently validate them.

## Final local verification

- Strict production Jekyll build passed with locked gems.
- 17 generated HTML pages, 277 local links/assets including fragment targets, and 34 JSON-LD blocks validated; zero broken local targets or malformed structured-data blocks.
- Independent final Assessment A inspected desktop1280 and mobile390, confirmed all five material findings resolved, clicked a card's descriptive text, and verified its anchored destination.
- A separate final browser matrix covered home, getting started, guide index, ShortGuid, and timeouts/backoff at 1280px, 390px, and 320px in light and dark themes. All 30 views passed page-overflow, one-main-H1, and body-text-contrast checks; descendant Guides state and keyboard skip-link focus also passed.
- Portable project-local Impeccable4.8.0 installation returned its version successfully; native binaries and machine state are excluded. Product context, token-bearing design document, extension sidecar, and docs surface brief record the finished implementation.

## Evidence and limits

Native browser creation failed with “The browser access host is unavailable.” The fallback used independent Playwright CLI browser sessions and actual local Jekyll pages. Assessment B confirmed mutable script injection and live detector execution on three pages; no native user-visible [Human] overlay was available or claimed. Its detector live server was stopped with `--keep-inject` and no injected script was written into repository HTML.

Final screenshots: `/tmp/pineapple-a-final-{mobile,guides,detail,desktop,desktop-detail,parallelism}.png` and `/tmp/pineapple-impeccable-final-shortguid-{light,dark}.png`. Local logs: `/tmp/pineapple-impeccable-build.log`, `/tmp/pineapple-impeccable-link-validation.log`, and `/tmp/pineapple-impeccable-final-browser.log`. These are session evidence and may be removed by temporary-file cleanup.

Remaining minor observations: mobile header wraps GitHub onto another row; long code examples intentionally scroll horizontally; repeated author/ecosystem material remains. These do not block the reviewed documentation tasks. No assistive-technology certification, exhaustive keyboard audit, production deployment, or library/API runtime acceptance is claimed by this UI review.

Questions skipped: the owner explicitly directed implementation of all review findings. No scope or aesthetic decision remained unresolved; the incumbent identity was preserved.
