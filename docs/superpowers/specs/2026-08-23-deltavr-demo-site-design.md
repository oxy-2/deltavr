# DeltaVR Demo Site — Design Specification

**Date:** 2026-08-23
**Status:** Approved for Implementation
**Target:** `oxygenated.uk/deltavr` (Vercel subdirectory deployment)
**Repo:** `C:\Users\rhime\Documents\GitHub\deltavr`

---

## 1. Project Overview

A demo site for the DeltaVR PCB project, hosted as a subdirectory of `oxygenated.uk`. Showcases:
- Interactive 3D PCB viewer (GLB from KiCad)
- Interactive schematic viewer (SVG from KiCad)
- Gerber viewer + download
- GitHub contributions graph + commit activity
- Wakatime/Hackatime coding hours tracker
- Image gallery (Stardance devlog images + renders)
- **Auth-gated posts/updates** (Stardance-style with time-elapsed)
- **Comments + likes + views** (public read, auth write)

**Design System:** Direct port of `oxygenated.uk` — Space Grotesk + JetBrains Mono, dot-matrix canvas background, folder-tab nav, settings drawer, terminal animation, red accent (#FF2E2E).

---

## 2. Tech Stack

| Layer | Choice |
|-------|--------|
| Framework | Next.js 14+ App Router + TypeScript |
| Styling | Tailwind CSS + custom CSS variables (exact port from mysite) |
| 3D | React Three Fiber + Drei |
| 2D Schematic | SVG + PanZoom (custom) |
| Gerber | gerber.js (web viewer) |
| Database/Auth | Supabase (PostgreSQL + Google OAuth + Realtime) |
| Deployment | Vercel (subdirectory: `oxygenated.uk/deltavr`) |
| CI/CD | GitHub Actions (KiCad export + deploy) |

---

## 3. Information Architecture

```
oxygenated.uk/deltavr
├── /                    # Landing: hero + 3D viewer preview + quick links
├── /pcb                 # 3D PCB viewer (orbit, layers, measure)
├── /pcb/schematic       # Interactive schematic (pan/zoom, net highlight, search)
├── /pcb/gerbers         # Gerber viewer + download links
├── /gallery             # Image grid + lightbox (Stardance + local)
├── /stats               # GitHub contributions + Wakatime charts
├── /updates             # Posts feed (time-elapsed, author, tags)
├── /updates/[slug]      # Post detail + comments + likes + views
├── /auth/callback       # Supabase OAuth callback
└── /api/*               # Server actions (posts, comments, likes, views)
```

---

## 4. Data Model (Supabase)

```sql
-- profiles (extends auth.users)
create table profiles (
  id uuid primary key references auth.users on delete cascade,
  username text unique not null,
  role text not null default 'viewer' check (role in ('owner','collaborator','viewer')),
  avatar_url text,
  created_at timestamptz default now()
);

-- posts (updates/devlogs)
create table posts (
  id uuid primary key default gen_random_uuid(),
  slug text unique not null,
  title text not null,
  content_mdx text not null,          -- MDX for rich content
  excerpt text,
  cover_image text,
  author_id uuid not null references profiles(id),
  status text not null default 'published' check (status in ('draft','published')),
  published_at timestamptz,
  created_at timestamptz default now(),
  updated_at timestamptz default now()
);

-- comments
create table comments (
  id uuid primary key default gen_random_uuid(),
  post_id uuid not null references posts(id) on delete cascade,
  author_id uuid not null references profiles(id),
  content text not null,
  parent_id uuid references comments(id) on delete cascade,  -- threading
  created_at timestamptz default now(),
  updated_at timestamptz default now()
);

-- likes (polymorphic: post or comment)
create table likes (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null references profiles(id) on delete cascade,
  target_type text not null check (target_type in ('post','comment')),
  target_id uuid not null,
  created_at timestamptz default now(),
  unique(user_id, target_type, target_id)
);

-- views (anonymous + authenticated)
create table views (
  id uuid primary key default gen_random_uuid(),
  target_type text not null check (target_type in ('post','page')),
  target_id uuid not null,
  user_id uuid references profiles(id) on delete set null,
  session_id text,  -- for anonymous
  created_at timestamptz default now()
);

-- RLS Policies
alter table profiles enable row level security;
alter table posts enable row level security;
alter table comments enable row level security;
alter table likes enable row level security;
alter table views enable row level security;

-- Profiles: public read, self update
create policy "public read profiles" on profiles for select using (true);
create policy "self update profile" on profiles for update using (auth.uid() = id);

-- Posts: public read published, owner/collaborator CRUD
create policy "public read published posts" on posts for select using (status = 'published');
create policy "owner collaborator insert posts" on posts for insert with check (
  exists (select 1 from profiles where id = auth.uid() and role in ('owner','collaborator'))
);
create policy "author update own posts" on posts for update using (author_id = auth.uid());
create policy "author delete own posts" on posts for delete using (author_id = auth.uid());

-- Comments: public read, auth write, author update/delete
create policy "public read comments" on comments for select using (true);
create policy "auth insert comments" on comments for insert with check (auth.uid() = author_id);
create policy "author update own comments" on comments for update using (author_id = auth.uid());
create policy "author delete own comments" on comments for delete using (author_id = auth.uid());

-- Likes: public read, auth write (upsert), self delete
create policy "public read likes" on likes for select using (true);
create policy "auth upsert likes" on likes for insert with check (auth.uid() = user_id)
  on conflict (user_id, target_type, target_id) do nothing;
create policy "self delete likes" on likes for delete using (auth.uid() = user_id);

-- Views: public insert (anon + auth), public read aggregated
create policy "anyone insert views" on views for insert with check (true);
create policy "public read views" on views for select using (true);

-- Realtime
alter publication supabase_realtime add table comments;
alter publication supabase_realtime add table likes;
alter publication supabase_realtime add table views;
```

---

## 5. Component Architecture

### 5.1 Design System (`components/ui/`)
Ported directly from `mysite`:
- `Button` — `btn-black`, `btn-ghost`, `btn-text`
- `Card` — border, bg-card, hover:border-dark
- `Tab` / `FolderTabs` — folder-tab nav with numbers
- `Drawer` — settings drawer (right slide)
- `Modal` — backdrop + centered card
- `Pill` / `Tag` / `Badge`
- `Terminal` — typing animation
- `DotMatrixBg` — full-screen canvas (reactive to mouse)
- `Shape3D` — heart/icosahedron/cube (draggable)
- `SettingsPanel` — contrast, spacing, theme toggles

### 5.2 3D Viewer (`components/three/PCBViewer.tsx`)
```typescript
interface PCBViewerProps {
  modelUrl: string;           // /models/deltavr-hmd.glb
  layers: PCBLayer[];
  defaultLayer: string;
  onLayerChange: (layer: string) => void;
  onMeasure: (distance: number) => void;
}
```
Features:
- OrbitControls (Drei)
- Layer toggle: Top Copper, Bottom Copper, Silk Top, Silk Bottom, Mask Top, Mask Bottom, Board Outline
- Measurement tool (click two points → distance in mm)
- Exploded view toggle
- Environment: studio lighting + HDRI

### 5.3 Schematic Viewer (`components/three/SchematicViewer.tsx`)
```typescript
interface SchematicViewerProps {
  svgUrl: string;             // /schematics/deltavr-hmd.svg
  nets: SchematicNet[];
  components: SchematicComponent[];
}
```
Features:
- PanZoom (svg-pan-zoom or custom)
- Net highlighting (click net → glow all connected pads)
- Component search (type ref designator → center + highlight)
- Tooltip on hover: ref, value, footprint
- Layer filter (show/hide nets by type)

### 5.4 Gerber Viewer (`components/three/GerberViewer.tsx`)
- Uses `gerber.js` to parse Gerber files in browser
- Layer stack visualization
- Download all as ZIP (existing `Gerber_PCB.zip`)
- Individual layer PNG export

### 5.5 Auth Components (`components/auth/`)
- `UserAvatar` — dropdown with profile, sign out
- `SignInButton` — Google OAuth
- `UserGate` — wrapper: `children` for owner/collaborator, fallback for viewer
- `useAuth()` hook — Supabase client + user + profile

### 5.6 Posts/Comments (`components/posts/`)
- `PostCard` — title, excerpt, time-elapsed, tags, like count, comment count, view count
- `PostDetail` — MDX rendering, author, time-elapsed, cover image
- `CommentThread` — realtime, threaded, like button, reply
- `LikeButton` — optimistic UI, realtime count
- `ViewCounter` — realtime unique views

---

## 6. Key Features Detail

### 6.1 Time-Elapsed Display
```typescript
function formatTimeElapsed(publishedAt: string): string {
  const diff = Date.now() - new Date(publishedAt).getTime();
  const days = Math.floor(diff / 86400000);
  const hours = Math.floor((diff % 86400000) / 3600000);
  const mins = Math.floor((diff % 3600000) / 60000);
  
  if (days > 0) return `${days}d ${hours}h elapsed`;
  if (hours > 0) return `${hours}h ${mins}m elapsed`;
  return `${mins}m elapsed`;
}
```
Shown on every post card and detail page (Stardance style).

### 6.2 GitHub Contributions
- GraphQL API: `contributionsCollection(from:, to:)` → contributionCalendar
- Display: heatmap calendar (like GitHub profile)
- Stats: total commits, PRs, issues, repos contributed to
- Cached at build time (ISR: 1 hour)

### 6.3 Wakatime/Hackatime
- REST API: `/api/v1/users/current/summaries?range=last_7_days`
- Charts: daily hours (bar), language breakdown (pie), editor breakdown
- Total hours all-time
- Cached at build time (ISR: 1 hour)

### 6.4 Gallery
- Sources: Stardance devlog images (scraped once at build) + local renders
- Grid: masonry layout, lazy load
- Lightbox: keyboard nav, zoom, download
- Tags: `hmd`, `controller`, `schematic`, `pcb`, `photo`, `render`

---

## 7. KiCad Export Pipeline

### 7.1 GitHub Action (`.github/workflows/kicad-export.yml`)
```yaml
name: Export KiCad Assets
on:
  push:
    paths:
      - 'kicad/**'
      - 'kicad controllers/**'
  workflow_dispatch:

jobs:
  export:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install KiCad
        run: |
          choco install kicad --version 10.0.0
      - name: Export HMD GLB
        run: |
          kicad-cli pcb export glb "kicad/deltavr hmd.kicad_pcb" -o "public/models/deltavr-hmd.glb"
      - name: Export HMD Schematic SVG
        run: |
          kicad-cli sch export svg "kicad/deltavr hmd.kicad_sch" -o "public/schematics/deltavr-hmd.svg"
      - name: Export Controller GLB
        run: |
          kicad-cli pcb export glb "kicad controllers/deltavr_controller/deltavr_controller.kicad_pcb" -o "public/models/deltavr-controller.glb"
      - name: Export Controller Schematic SVG
        run: |
          kicad-cli sch export svg "kicad controllers/deltavr_controller/deltavr_controller.kicad_sch" -o "public/schematics/deltavr-controller.svg"
      - name: Commit assets
        run: |
          git config user.name "github-actions"
          git config user.email "actions@github.com"
          git add public/models public/schematics
          git diff --staged --quiet || git commit -m "chore: update KiCad exports"
          git push
```

### 7.2 Local Export Script (`scripts/export-kicad.js`)
For local development without CI.

---

## 8. Styling Specification

### 8.1 CSS Variables (Exact Port)
```css
:root {
  --bg: #ffffff;
  --bg-subtle: #fafafa;
  --bg-card: #ffffff;
  --border: #e5e5e5;
  --border-dark: #111111;
  --text: #111111;
  --text-muted: #555555;
  --text-dim: #888888;
  --red-accent: #ff2e2e;
  --nav-bg: rgba(255, 255, 255, 0.92);
  --font-sans: 'Space Grotesk', system-ui, -apple-system, sans-serif;
  --font-mono: 'JetBrains Mono', monospace;
  --container: 1120px;
  --nav-h: 58px;
}

[data-theme="dark"] {
  --bg: #0c0c0e;
  --bg-subtle: #131316;
  --bg-card: #131316;
  --border: #2a2a32;
  --border-dark: #f0f0f4;
  --text: #f5f5fa;
  --text-muted: #a9a9b8;
  --text-dim: #71717f;
  --nav-bg: rgba(12, 12, 14, 0.92);
}
```

### 8.2 Tailwind Config
```js
// tailwind.config.ts
export default {
  darkMode: 'class',
  content: ['./app/**/*.{ts,tsx}', './components/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        bg: 'var(--bg)',
        'bg-subtle': 'var(--bg-subtle)',
        'bg-card': 'var(--bg-card)',
        border: 'var(--border)',
        'border-dark': 'var(--border-dark)',
        text: 'var(--text)',
        'text-muted': 'var(--text-muted)',
        'text-dim': 'var(--text-dim)',
        'red-accent': 'var(--red-accent)',
      },
      fontFamily: {
        sans: ['var(--font-sans)'],
        mono: ['var(--font-mono)'],
      },
    },
  },
}
```

---

## 9. Environment Variables

```env
# .env.local
NEXT_PUBLIC_SUPABASE_URL=...
NEXT_PUBLIC_SUPABASE_ANON_KEY=...
SUPABASE_SERVICE_ROLE_KEY=...

GITHUB_TOKEN=...                    # PAT for GraphQL
WAKATIME_API_KEY=...                # or HACKATIME_API_KEY

NEXT_PUBLIC_SITE_URL=https://oxygenated.uk/deltavr
```

---

## 10. Deployment

### 10.1 Vercel Configuration
```json
// vercel.json
{
  "rewrites": [
    { "source": "/deltavr/:path*", "destination": "/deltavr/:path*" }
  ]
}
```
Or separate Vercel project linked to `deltavr` repo with custom domain `oxygenated.uk/deltavr`.

### 10.2 Supabase Setup
1. Create project
2. Enable Google OAuth provider
3. Run SQL migrations (data model above)
4. Add `NEXT_PUBLIC_SUPABASE_URL` + `NEXT_PUBLIC_SUPABASE_ANON_KEY` to Vercel

---

## 11. Acceptance Criteria

| Feature | Criteria |
|---------|----------|
| 3D Viewer | Loads GLB, orbit/pan/zoom, layer toggle, measure tool, 60fps on desktop |
| Schematic | Loads SVG, pan/zoom, net highlight on click, component search |
| Gerber | Viewer renders layers, download ZIP works |
| GitHub Stats | Heatmap + counts, updates within 1hr |
| Wakatime | Charts render, total hours shown |
| Gallery | Masonry grid, lightbox with nav |
| Auth | Google sign-in, role-based post creation |
| Posts | MDX rendering, time-elapsed, cover image |
| Comments | Realtime, threaded, likes, auth required |
| Likes | Optimistic UI, realtime count |
| Views | Unique count (anon + auth), realtime |
| Design | Matches oxygenated.uk exactly (fonts, colors, spacing, animations) |
| Deploy | Accessible at `oxygenated.uk/deltavr` |

---

## 12. Out of Scope (Future)

- In-browser PCB editor (use KiCad native)
- Firmware build pipeline
- BOM cost calculator
- Multi-language
- Email notifications

---

## 13. Implementation Order

1. **Foundation** — Next.js + Tailwind + design system port + Supabase client
2. **KiCad Export** — GitHub Action + local script + commit GLB/SVG
3. **3D Viewer** — PCBViewer with layers + measure
4. **Schematic Viewer** — SVG + pan/zoom + net highlight
5. **Gerber Viewer** — gerber.js + download
6. **Stats** — GitHub + Wakatime (ISR)
7. **Gallery** — Grid + lightbox
8. **Auth** — Supabase Google OAuth + profiles + RLS
9. **Posts** — CRUD + MDX + time-elapsed
10. **Comments/Likes/Views** — Realtime + optimistic UI
11. **Polish** — SEO, OG images, error boundaries, loading states
12. **Deploy** — Vercel + custom domain + monitoring