---
name: "Pineapple documentation"
description: "The incumbent white, lavender, and purple technical documentation system."
colors:
  bg: "#fbfbfd"
  surface: "#ffffff"
  text: "#1b1a22"
  muted: "#5d5b6b"
  border: "#e6e4ee"
  accent: "#5b3fd6"
  accent-hover: "#4a2fc0"
  accent-ink: "#ffffff"
  accent-soft: "#efebff"
  focus: "#9a6a00"
  code-bg: "#f3f2f8"
  syn-keyword: "#5b3fd6"
  syn-string: "#0f766e"
  syn-type: "#a1580b"
  syn-number: "#b4235a"
  dark-bg: "#121019"
  dark-surface: "#1b1824"
  dark-text: "#ecebf3"
  dark-muted: "#a6a3b8"
  dark-border: "#2c2838"
  dark-accent: "#a593ff"
  dark-accent-hover: "#bcafff"
  dark-accent-ink: "#15111f"
  dark-accent-soft: "#261f3d"
  dark-code-bg: "#211d2c"
  dark-focus: "#f2b51c"
  dark-syn-keyword: "#b9a8ff"
  dark-syn-string: "#6fd3c4"
  dark-syn-type: "#f2c25c"
  dark-syn-number: "#ff8fb4"
typography:
  body:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "1rem"
    lineHeight: 1.6
  lead:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "1.2rem"
    lineHeight: 1.6
  card-title:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: "1.05rem"
    fontWeight: 700
    lineHeight: 1.25
  metadata:
    fontFamily: "ui-sans-serif, system-ui, -apple-system, \"Segoe UI\", Roboto, sans-serif"
    fontSize: ".875rem"
    lineHeight: 1.6
  code:
    fontFamily: "ui-monospace, \"SF Mono\", \"Cascadia Code\", Menlo, Consolas, monospace"
    fontSize: ".9em"
  code-block:
    fontFamily: "ui-monospace, \"SF Mono\", \"Cascadia Code\", Menlo, Consolas, monospace"
    fontSize: ".9em"
    lineHeight: 1.45
rounded:
  inline-code: "4px"
  surface: "10px"
  author: "14px"
spacing:
  half: ".5rem"
  three-quarter: ".75rem"
  unit: "1rem"
  gutter: "1.25rem"
  section: "2rem"
  heading: "2.5rem"
  group: "3rem"
components:
  button-primary:
    backgroundColor: "{colors.accent}"
    textColor: "{colors.accent-ink}"
    rounded: "{rounded.surface}"
    padding: ".65rem 1.1rem"
  button-primary-hover:
    backgroundColor: "{colors.accent-hover}"
  button-secondary:
    backgroundColor: "transparent"
    textColor: "{colors.text}"
    rounded: "{rounded.surface}"
    padding: ".65rem 1.1rem"
  button-secondary-hover:
    backgroundColor: "{colors.accent-soft}"
  card:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.text}"
    rounded: "{rounded.surface}"
    padding: "1.1rem 1.2rem"
  callout:
    backgroundColor: "{colors.surface}"
    rounded: "{rounded.surface}"
    padding: ".75rem 1rem"
  code-block:
    backgroundColor: "{colors.code-bg}"
    rounded: "{rounded.surface}"
    padding: "1rem"
---

# Design System: Pineapple documentation

## Overview

**Creative North Star: "The Technical Reading Desk"**

Pineapple documentation uses quiet surfaces, purple links, lavender code backgrounds, and bordered containers to make package installation and API examples easy to scan. The existing pineapple and author marks carry the identity; interface decoration remains restrained.

This is a source-derived description of the incumbent build, not a newly commissioned brand metaphor. PRODUCT.md establishes the technical audience and preservation requirement; no separate visual interview or anti-reference was supplied.

**Key Characteristics:**
- Light and automatic dark palettes share semantic CSS variables.
- Long-form reading uses a bounded measure; indexes can use a wider grid.
- Borders and tonal contrast separate content without general elevation.

Evidence: `assets/css/site.css`, `_layouts/default.html`, `index.md`, `guides/index.md`, and `_includes/faq.html`. Values describe the current source; this document does not replace the incumbent identity.

## Colors

The primary purple sits against white and pale lavender, with an automatic dark equivalent for every semantic color. Frontmatter records the exact source values; `dark-` entries correspond to the same CSS variables inside the dark media query.

### Primary

- **Accent purple:** links, primary actions, card headings, and selected browser affordances.
- **Hover purple:** primary-action hover; **soft lavender:** secondary-action hover and selection.
- **Accent ink:** text on primary actions, including its distinct dark-theme value.

### Neutral

- **Page, surface, text, muted, and border:** page canvas, panels, readable content, supporting copy, and separators respectively.
- **Code background:** inline and block examples. Syntax keyword, string, type, and number colors annotate code; they are not additional interface accents.
- **Focus amber:** high-visibility keyboard outline, independently themed.

**The Semantic Theme Rule.** Use the existing semantic custom properties so light and dark treatments change together.

## Typography

**Body Font:** the platform sans stack recorded in frontmatter.
**Label/Mono Font:** the platform monospace stack recorded in frontmatter.

Character is direct and technical. There is no separately sourced display face. The existing page heading uses the body stack at `clamp(2rem, 4vw, 2.75rem)`, bold browser heading weight, line height (1.25), and tracking (-.01em). This is an observed incumbent exception, not a normative display-font token for future surfaces.

### Hierarchy

- **Body:** browser-default base size (normally 16px), inherited line height; frontmatter's (1rem) expresses that relative base rather than a forced root size.
- **Lead:** larger muted introduction using the body family.
- **Section headings:** balanced wrapping and line height (1.25); h2/h3 sizes and bold weight use browser defaults unless component CSS overrides them. Section spacing is explicit: h2 margin-top (2.5rem), padding-top (.5rem), h3 margin-top (1.75rem).
- **Card and FAQ titles:** compact bold headings from the card-title role. Guide-index titles use (1.1rem).
- **Metadata and breadcrumb:** smaller muted body copy.
- **Code:** relative monospace sizing, block line height as recorded above. Inline code nests at (.9em), including inside highlighted blocks where nested code inherits the relative rule.
- **Author links:** local uppercase monospace treatment (.75rem), tracking (.1em); not a page-heading label pattern.

**The Reading Measure Rule.** Keep long-form articles and introductory prose within the observed reading measure; wide indexes retain their explicit exception.

## Layout

Centered wrappers cap at (72rem), with horizontal gutters (1.25rem). Main content begins after (2rem) and ends with (4rem) of padding. Standard articles cap at (68ch). Wide articles remove that cap while direct paragraphs and leads retain it. These are current CSS values, not a universal full-width doctrine.

The wrapping header uses a minimum height (4rem), gap (1rem), and navigation gap (1.25rem). Cards auto-fill columns with minimum width (15rem), gap (1rem). The related-project grid uses minimum width (17rem), with horizontal gap (2rem) and ruled rows. Guide indexes use stacked, separated entries rather than a metadata table.

At maximum width (640px), the hero becomes one column and its decorative duplicate logo disappears; the author panel becomes a centered single column with padding (2rem 1.5rem); the header gains vertical padding (.5rem) and navigation uses gap (.25rem 1rem) with link padding (.4rem 0). Code blocks and tables scroll locally; long body strings can wrap anywhere.

## Elevation & Depth

Ordinary surfaces have no box shadow. Pale backgrounds and one-pixel borders supply separation. Cards change border color on hover or focus within using a transition (.15s ease-out). Buttons change background without an explicit animated transition.

### Shadow Vocabulary

- **Author identity halo:** `0 0 0 2px rgba(255, 106, 26, .3), 0 10px 24px rgba(0, 0, 0, .18)` surrounds only the circular author mark. The local black background (#151515) and orange halo are incumbent asset framing, not general palette primitives.

**The Border First Rule.** Use borders and surface contrast for ordinary containers; the author avatar halo remains a local identity treatment.

## Shapes

The surface radius is shared by actions, cards, callouts, and code blocks. Inline code uses the smaller radius. The author panel alone has the larger author radius; its avatar is circular (50%). Borders are (1px) and tables collapse their borders into horizontal rules. Focus uses an outline (3px solid focus amber) with offset (2px), including card links and FAQ summaries.

## Components

### Actions

Navigation anchors styled as buttons, with semibold weight (600). Primary actions use accent and accent ink; secondary actions are transparent with text-colored ink and a border. Hover uses the recorded primary-hover or secondary-hover variant. Both retain the global keyboard outline. There is no disabled, loading, or pressed-state design in the source.

### Cards / Containers

Linked cards combine the shared surface radius, surface fill, border, recorded padding, compact accent heading, and muted description (.95rem). The entire card is an anchor. Hover and focus within accent the border; focus-visible adds the outline. Callouts use the same flat surface vocabulary with their own recorded padding.

### Navigation

Header links use muted ink and medium weight (500); hover and any `aria-current` state switch to text ink. The layout marks exact routes as `page` and descendant guides as `location`. Links wrap at narrow widths. Breadcrumbs use the metadata size; footer links use text ink and turn accent on hover. The skip link becomes visible above the page when focused.

### Code and tables

Inline code has a softly rounded fill and padding (.1em .35em). Blocks add a border, shared radius, recorded padding, and overflow scrolling. Syntax classes use the semantic syntax colors; comments are muted and italic. Tables use (.95rem) text, cell padding (.6rem .75rem), and bottom borders, with surface-filled headers.

### FAQ and guide index

FAQ uses native `details` and `summary`, bottom dividers, and padding (.75rem 0); the first item opens in the template. Its existing generated plus/minus characters are implementation evidence, not an icon-system specification. The guide index uses list items with top borders, padding (1rem 0), compact linked headings, and muted namespace/type metadata.

## Do's and Don'ts

### Do:
- Do use semantic CSS variables for both color schemes.
- Do preserve visible keyboard focus on links and disclosure summaries.
- Do keep guide metadata visible in the stacked guide index.
- Do preserve local horizontal scrolling for code blocks and tables.

### Don't:
- Don't apply the author avatar halo to ordinary cards.
- Don't create form fields, chips, or a general icon library from this documentation; those patterns are not implemented.
- Don't turn the existing system heading face or FAQ character markers into a future display or icon standard.

Not canonized or repaired: the incumbent platform display face and generated FAQ character icons remain source-level craft-floor exceptions; this documentation-only pass preserves the approved scope rather than legitimizing them as reusable house rules. No new raster assets were produced.
