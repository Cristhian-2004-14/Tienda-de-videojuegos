---
name: Kinetic Console
colors:
  surface: '#131313'
  surface-dim: '#131313'
  surface-bright: '#3a3939'
  surface-container-lowest: '#0e0e0e'
  surface-container-low: '#1c1b1b'
  surface-container: '#201f1f'
  surface-container-high: '#2a2a2a'
  surface-container-highest: '#353534'
  on-surface: '#e5e2e1'
  on-surface-variant: '#becab7'
  inverse-surface: '#e5e2e1'
  inverse-on-surface: '#313030'
  outline: '#899482'
  outline-variant: '#3f4a3b'
  surface-tint: '#79dd68'
  primary: '#79dd68'
  on-primary: '#003a01'
  primary-container: '#107c10'
  on-primary-container: '#b5ffa2'
  inverse-primary: '#006e06'
  secondary: '#c8c6c5'
  on-secondary: '#303030'
  secondary-container: '#474746'
  on-secondary-container: '#b7b5b4'
  tertiary: '#ffb0cd'
  on-tertiary: '#640039'
  tertiary-container: '#b83a77'
  on-tertiary-container: '#ffe6ed'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#94fa81'
  primary-fixed-dim: '#79dd68'
  on-primary-fixed: '#002200'
  on-primary-fixed-variant: '#005303'
  secondary-fixed: '#e4e2e1'
  secondary-fixed-dim: '#c8c6c5'
  on-secondary-fixed: '#1b1c1c'
  on-secondary-fixed-variant: '#474746'
  tertiary-fixed: '#ffd9e4'
  tertiary-fixed-dim: '#ffb0cd'
  on-tertiary-fixed: '#3e0021'
  on-tertiary-fixed-variant: '#890f52'
  background: '#131313'
  on-background: '#e5e2e1'
  surface-variant: '#353534'
typography:
  display-lg:
    fontFamily: Inter
    fontSize: 48px
    fontWeight: '800'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '700'
    lineHeight: 32px
  headline-md:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-lg:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-caps:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '700'
    lineHeight: 16px
    letterSpacing: 0.05em
  mono-label:
    fontFamily: JetBrains Mono
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 8px
  xs: 4px
  sm: 12px
  md: 24px
  lg: 40px
  xl: 64px
  container-max: 1440px
  gutter: 24px
---

## Brand & Style
This design system captures the focused, high-performance energy of modern gaming consoles, translated into a high-productivity administrative environment. The brand personality is authoritative yet lean, emphasizing speed of thought and clarity of action. 

The style is **Modern Corporate with a High-Tech lean**, drawing heavily from the Xbox "Dashboard" aesthetic. It utilizes a deep-space dark mode to reduce eye strain during long sessions, relying on vibrant primary accents to guide the user's intent. The interface prioritizes "Content over Chrome," stripping away gradients, heavy borders, and skeuomorphic details in favor of flat, dimensional surfaces and crisp typography. The emotional response is one of total control and professional efficiency.

## Colors
The palette is rooted in a "Pure Dark" philosophy. The background uses a near-black (#101010) to create a void-like canvas, while surfaces use a slightly lighter grey (#181818) to define spatial relationships without harsh contrast.

**Xbox Green (#107C10)** is reserved exclusively for primary calls to action, active states, and critical success indicators. Secondary actions use a subtle mid-grey (#252525) to remain discoverable but visually recessive. Text hierarchy is maintained through high-contrast white for headers and a muted "Atmospheric Silver" (#AAAAAA) for metadata and body copy.

## Typography
The typography system uses **Inter** for its neutral, highly legible Swiss-style characteristics, which mirror the utilitarian nature of system fonts. Headlines are set with tight tracking and heavy weights to provide an "architectural" feel to the page layout.

For technical data, ID strings, and system status, **JetBrains Mono** is introduced sparingly to provide a "developer-tool" precision. Large display type should use a slight negative letter-spacing to maintain a compact, impactful look on dashboard summaries.

## Layout & Spacing
The layout follows a **Fixed-Fluid Hybrid Grid**. The sidebar and navigation elements occupy fixed widths to ensure muscle memory, while the main content area utilizes a fluid 12-column grid. 

A strict **8px linear scale** governs all spatial decisions. Margins between major cards are kept generous (24px or 40px) to allow the "Pure Dark" background to act as a natural separator, eliminating the need for divider lines. On mobile, horizontal padding shifts from 40px to 16px, and complex grids reflow into a single-column vertical stack.

## Elevation & Depth
In this design system, depth is communicated through **Tonal Stepping** rather than traditional shadows. 

1.  **Level 0 (Background):** #101010 - The base canvas.
2.  **Level 1 (Cards/Surfaces):** #181818 - Floating 1px above the background.
3.  **Level 2 (Hover States/Overlays):** #252525 - Used for interactive elements that are being engaged with.

When a shadow is necessary (e.g., for modal windows), use a large, 0%-blur "block shadow" or an extremely subtle 10% black glow to maintain the flat, digital aesthetic. Avoid soft, ambient, colored shadows.

## Shapes
The shape language is disciplined and geometric. A standard **8px (0.5rem)** radius is applied to all primary containers, buttons, and input fields. This provides enough softness to feel modern and approachable without veering into the "playful" territory of fully rounded or pill-shaped designs.

Iconography should be "Line-Weight Balanced," using 2px stroke widths to match the visual weight of the Inter typeface.

## Components

### Buttons
- **Primary:** Background #107C10, Text #FFFFFF, Bold weight. No border.
- **Secondary:** Background #252525, Text #FFFFFF. 
- **Ghost:** No background, #AAAAAA text, shifts to #FFFFFF on hover.

### Cards
Cards are the primary structural unit. They must use the #181818 surface color with 8px rounded corners. Do not use borders; use spacing (24px) to separate cards.

### Input Fields
Inputs use a slightly darker-than-surface background (#121212) with a 1px border of #252525. On focus, the border changes to #107C10 with a subtle 2px outer glow of the same color.

### Chips & Tags
Small, rectangular elements with a 4px radius. Status chips use low-opacity versions of their semantic colors (e.g., a dark green tint for "Online") with high-intensity text.

### Focus Indicators
Crucial for an Xbox-inspired system: any keyboard or controller-focused element should receive a high-visibility 2px solid #107C10 offset border when focused, ensuring the "Active State" is never in doubt.