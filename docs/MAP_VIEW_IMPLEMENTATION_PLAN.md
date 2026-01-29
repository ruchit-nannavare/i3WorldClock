# Interactive World Map View - Implementation Plan

## User Story Summary
**US-007: Interactive World Map View with Game-Inspired Transition**

As a TimeSpot user, I want to toggle between the clock view and an interactive world map view with a smooth, game-inspired animation, so that I can visualize my saved cities geographically and interact with them spatially.

---

## Architecture Overview

### Data Flow
**IMPORTANT:** The map feature leverages existing database data - **NO new API calls required**

```
Database (PostgreSQL)
  └── Cities Table (latitude, longitude, name, country, etc.)
      └── Backend API (/api/cities/search)
          └── Frontend State (useCities hook)
              └── savedCities array (City[])
                  └── Map Components
                      └── mapProjection.ts (lat/long → SVG x/y)
                          └── MapPin rendering at calculated position
```

**Key Points:**
- ✅ City coordinates (latitude/longitude) already exist in the database
- ✅ Already fetched via `/api/cities/search` API endpoint
- ✅ Already stored in frontend state via `useCities` hook
- ✅ Map simply **visualizes existing data geographically**
- ✅ Each city's lat/long converted to SVG coordinates for pin placement

**Existing City Interface:**
```typescript
interface City {
  id: string;
  name: string;
  country: string;
  countryCode: string;
  latitude: number;        // ← Used for map positioning
  longitude: number;       // ← Used for map positioning
  utcOffsetSeconds: number;
  utcOffsetDisplay: string;
}
```

### Core Components to Build
1. **WorldMapView** - Main map container with interactive pins
2. **MiniMap** - Compact corner widget showing map preview
3. **MapToggle** - Button to switch between views
4. **MapPin** - Individual city marker on the map
5. **MapPinTooltip** - Hover tooltip for pins

### Hooks
1. **useMapView** - Manages view state (clock/map) and transition logic
2. **useMapProjection** - Converts lat/long to SVG coordinates

### Utils
1. **mapAnimations.ts** - Animation timing and easing functions
2. **mapProjection.ts** - Geographic coordinate transformations

---

## Technical Implementation

### Phase 1: Setup & Dependencies
**Estimated Tasks: 2 | Priority: Critical**

#### Task 1.1: Install Dependencies
**Description:** Add required npm packages for map rendering
```bash
npm install react-simple-maps d3-geo @types/d3-geo
```

**Acceptance Criteria:**
- [x] `react-simple-maps` installed for map rendering
- [x] `d3-geo` installed for geographic projections
- [x] TypeScript types available
- [x] No dependency conflicts

**Files Modified:**
- `client/package.json`

---

#### Task 1.2: Create TopoJSON World Map Data
**Description:** Add simplified world map data file for rendering

**Acceptance Criteria:**
- [x] Download/create `world-110m.json` (low-resolution world map)
- [x] Place in `client/public/maps/` directory
- [x] File size under 100KB for performance
- [x] Contains continent/country outlines only

**Files Created:**
- `client/public/maps/world-110m.json`

**Notes:** Use Natural Earth 110m resolution data from react-simple-maps CDN or official Natural Earth dataset.

---

### Phase 2: State Management & Hooks
**Estimated Tasks: 2 | Priority: Critical**

#### Task 2.1: Create useMapView Hook
**Description:** Central hook managing view state and transition logic

**Acceptance Criteria:**
- [x] Manages `isMapExpanded` boolean state
- [x] Provides `toggleMapView()` function
- [x] Persists view preference to localStorage (`i3worldclock-mapview`)
- [x] Exposes `isAnimating` state during transitions
- [x] Animation duration constant (400ms)

**Files Created:**
- `client/src/hooks/useMapView.ts`

**Implementation Details:**
```typescript
interface MapViewState {
  isMapExpanded: boolean;
  isAnimating: boolean;
  toggleMapView: () => void;
}

// Animation phases:
// 1. isAnimating = true
// 2. Trigger CSS transforms
// 3. After 400ms: isAnimating = false
```

---

#### Task 2.2: Create Map Projection Utilities
**Description:** Helper functions for coordinate transformations

**CRITICAL:** This utility converts database lat/long values to SVG pixel coordinates for pin placement.

**Acceptance Criteria:**
- [x] `latLngToMapCoords(lat, lng, mapWidth, mapHeight)` function
- [x] Uses Mercator projection (standard for world maps)
- [x] Returns `{x, y}` pixel coordinates
- [x] Handles edge cases (poles, dateline)
- [x] Memoized calculations for performance
- [x] Accepts latitude and longitude from `City` interface
- [x] Works with coordinates from database (range: -90 to 90 lat, -180 to 180 lng)

**Files Created:**
- `client/src/utils/mapProjection.ts`

**Dependencies:**
```typescript
import { geoMercator } from 'd3-geo';
```

**Implementation Example:**
```typescript
// Input: Database city coordinates
const city = {
  name: "London",
  latitude: 51.5074,   // From database
  longitude: -0.1278   // From database
};

// Output: SVG pixel position
const { x, y } = latLngToMapCoords(
  city.latitude, 
  city.longitude, 
  mapWidth, 
  mapHeight
);
// Result: { x: 512, y: 234 } (example pixel coordinates)
```

---

### Phase 3: Map Components
**Estimated Tasks: 5 | Priority: High**

#### Task 3.1: Create WorldMapView Component
**Description:** Full-screen interactive map view that displays saved cities from database

**Acceptance Criteria:**
- [x] Uses `<ComposableMap>` from react-simple-maps
- [x] Renders world outline with `<Geographies>`
- [x] Outline-only styling (no fills)
- [x] **Renders MapPin for each saved city using database lat/long coordinates**
- [x] **Iterates over `cities` array (fetched from database via useCities hook)**
- [x] **Each pin positioned using `city.latitude` and `city.longitude`**
- [x] Responsive sizing (100% container width/height)
- [x] Supports dark/light mode theming
- [x] Smooth zoom/pan disabled (static view for v1)

**Files Created:**
- `client/src/components/WorldMapView.tsx`
- `client/src/components/WorldMapView.css`

**Props Interface:**
```typescript
interface WorldMapViewProps {
  cities: City[];  // ← Passed from App.tsx (savedCities from database)
  primaryCityId: string | null;
  utcTime: Date;
  is24Hour: boolean;
  isDarkMode: boolean;
  isExpanded: boolean;
  onCitySelect: (cityId: string) => void;
}
```

**Implementation Logic:**
```typescript
// Inside WorldMapView component
{cities.map((city) => (
  <MapPin
    key={city.id}
    city={city}  // Contains latitude/longitude from database
    latitude={city.latitude}   // ← Database value
    longitude={city.longitude} // ← Database value
    isPrimary={city.id === primaryCityId}
    onClick={() => onCitySelect(city.id)}
    // ... other props
  />
))}
```

**Styling Requirements:**
- Outline color: `rgba(255, 255, 255, 0.15)` (dark), `rgba(0, 0, 0, 0.08)` (light)
- Stroke width: `0.5px`
- Map background: transparent (inherits gradient)
- Container: `position: absolute` for animation

---

#### Task 3.2: Create MiniMap Component
**Description:** Small corner preview map showing all saved cities

**Acceptance Criteria:**
- [x] Fixed size: 150px × 80px
- [x] Positioned in bottom-right corner (with offset)
- [x] Glassmorphism container:
  - `backdrop-filter: blur(10px)`
  - Semi-transparent background
  - Subtle border
  - Border radius: 8px
- [x] **Simplified city pins (small dots) for all cities in database**
- [x] **Uses same cities array from `savedCities` state**
- [x] **Pins positioned using database lat/long values**
- [x] Click on mini-map expands to full view
- [x] Shows pulse animation on primary city pin

**Files Created:**
- `client/src/components/MiniMap.tsx`
- `client/src/components/MiniMap.css`

**Props Interface:**
```typescript
interface MiniMapProps {
  cities: City[];  // ← Contains latitude/longitude from database
  primaryCityId: string | null;
  isDarkMode: boolean;
  onClick: () => void;
}
```

**Data Flow:**
```typescript
// In App.tsx
<MiniMap 
  cities={savedCities}  // ← From database via useCities hook
  primaryCityId={primaryCityId}
  isDarkMode={isDarkMode}
  onClick={() => toggleMapView()}
/>

// In MiniMap.tsx - render pins using database coordinates
{cities.map(city => (
  <circle
    key={city.id}
    cx={projectLongitude(city.longitude)}  // ← Database value
    cy={projectLatitude(city.latitude)}    // ← Database value
    r={city.id === primaryCityId ? 3 : 2}
  />
))}
```

**Positioning:**
- `position: fixed`
- `bottom: 120px` (above city carousel)
- `right: 40px`
- `z-index: 100`

---

#### Task 3.3: Create MapPin Component
**Description:** Interactive city marker on map positioned using database coordinates

**Acceptance Criteria:**
- [x] Renders as glowing dot (6-8px diameter)
- [x] **Positioned at exact latitude/longitude from database**
- [x] **Uses mapProjection utility to convert lat/long to pixel coordinates**
- [x] Primary city: larger, pulsing animation
- [x] Non-primary cities: smaller, subtle glow
- [x] Hover state: scale up, show tooltip
- [x] Click handler: select city
- [x] Color: accent color from theme

**Files Created:**
- `client/src/components/MapPin.tsx`
- `client/src/components/MapPin.css`

**Props Interface:**
```typescript
interface MapPinProps {
  city: City;  // ← Contains latitude/longitude from database
  utcTime: Date;
  is24Hour: boolean;
  isPrimary: boolean;
  isDarkMode: boolean;
  onClick: (cityId: string) => void;
  onHover: (city: City | null) => void;
  size?: 'small' | 'medium' | 'large'; // For mini-map vs full map
}
```

**Implementation Note:**
```typescript
// MapPin receives city object with database coordinates
const MapPin = ({ city, ... }) => {
  // Use city.latitude and city.longitude to determine position
  // react-simple-maps <Marker> component handles projection internally
  return (
    <Marker coordinates={[city.longitude, city.latitude]}>
      {/* Pin visual */}
    </Marker>
  );
};
```

**Animation:**
- Pulse effect: CSS keyframe animation
- `animation: pulse 2s ease-in-out infinite;`
- Scale range: 1.0 → 1.3 → 1.0

---

#### Task 3.4: Create MapPinTooltip Component
**Description:** Hover tooltip for map pins

**Acceptance Criteria:**
- [x] Shows city name, country
- [x] Displays current local time (formatted)
- [x] Day/Night icon indicator
- [x] Glassmorphism design matching mini-map
- [x] Fade-in animation (200ms)
- [x] Positioned near pin (auto-adjusts for screen edges)
- [x] Only shows on full map view, not mini-map

**Files Created:**
- `client/src/components/MapPinTooltip.tsx`
- `client/src/components/MapPinTooltip.css`

**Props Interface:**
```typescript
interface MapPinTooltipProps {
  city: City;
  localTime: Date;
  is24Hour: boolean;
  isDayTime: boolean;
  position: { x: number; y: number };
  isDarkMode: boolean;
}
```

---

#### Task 3.5: Create MapToggleButton Component
**Description:** Toggle button to switch views

**Acceptance Criteria:**
- [x] Icon changes based on state:
  - Collapsed: Globe/Map expand icon
  - Expanded: Minimize/compress icon
- [x] Positioned near mini-map or in header
- [x] Tooltip on hover ("Expand Map" / "Show Clock")
- [x] Smooth icon transition
- [x] Accessible (keyboard + screen reader)

**Files Created:**
- `client/src/components/MapToggleButton.tsx`
- `client/src/components/MapToggleButton.css`

**Props Interface:**
```typescript
interface MapToggleButtonProps {
  isExpanded: boolean;
  onClick: () => void;
}
```

**Icons:**
- Use `react-icons/fi`: `FiMaximize2` (expand), `FiMinimize2` (collapse)

---

### Phase 4: Animation System
**Estimated Tasks: 2 | Priority: High**

#### Task 4.1: Create Animation Utilities
**Description:** Easing functions and animation helpers

**Acceptance Criteria:**
- [x] `easeOutCubic` function for smooth deceleration
- [x] `springEasing` function for playful bounce
- [x] Animation duration constants
- [x] CSS custom properties for dynamic values

**Files Created:**
- `client/src/utils/mapAnimations.ts`

**Constants:**
```typescript
export const ANIMATION_DURATION = 400; // ms
export const MINI_MAP_SIZE = { width: 150, height: 80 };
export const MINI_MAP_POSITION = { bottom: 120, right: 40 };
```

---

#### Task 4.2: Implement View Transition Animations
**Description:** CSS animations for clock ↔ map transitions

**Acceptance Criteria:**
- [x] Clock scales from 100% → 25% when minimizing
- [x] Clock moves from center to bottom-left corner
- [x] Mini-map scales from 100% → 400% when expanding
- [x] Mini-map moves from bottom-right to center
- [x] Both animations occur simultaneously
- [x] Uses CSS transforms (scale, translate) for performance
- [x] Spring/ease-out timing function
- [x] No layout jank or reflows

**Files Modified:**
- `client/src/App.css`

**CSS Classes:**
```css
.main-clock.map-view-collapsed { /* Full size, centered */ }
.main-clock.map-view-expanded { /* 25% scale, corner position */ }
.world-map.map-view-collapsed { /* Mini size, corner */ }
.world-map.map-view-expanded { /* Full size, centered */ }
```

**Transform Strategy:**
```css
/* Example: Clock transition */
.main-clock {
  transform-origin: center;
  transition: transform 400ms cubic-bezier(0.34, 1.56, 0.64, 1);
}

.main-clock.map-view-expanded {
  transform: scale(0.25) translate(-200%, 150%);
}
```

---

### Phase 5: Integration
**Estimated Tasks: 3 | Priority: Critical**

#### Task 5.1: Integrate Map View into App.tsx
**Description:** Wire up map components using existing database-backed city state

**Acceptance Criteria:**
- [x] Import `useMapView` hook
- [x] Render `MiniMap` component (always visible)
- [x] Render `WorldMapView` component
- [x] Render `MapToggleButton`
- [x] **Pass `savedCities` array to map components (contains database lat/long)**
- [x] **No additional API calls needed - reuse existing city data**
- [x] Pass shared state (primaryCityId, utcTime, is24Hour, isDarkMode)
- [x] Conditional rendering based on `isMapExpanded`
- [x] Layout accommodates both views

**Files Modified:**
- `client/src/App.tsx`

**Integration Code Example:**
```typescript
function App() {
  const { savedCities, primaryCityId, setPrimaryCity, ... } = useCities();
  const { isMapExpanded, toggleMapView, isAnimating } = useMapView();
  // ... other hooks

  return (
    <>
      {/* Clock View - scales down when map expands */}
      <div className={`clock-container ${isMapExpanded ? 'minimized' : ''}`}>
        <MainClock {...clockProps} />
      </div>

      {/* World Map View - scales up when expanded */}
      <WorldMapView
        cities={savedCities}  // ← Database cities with lat/long
        primaryCityId={primaryCityId}
        utcTime={utcTime}
        is24Hour={is24Hour}
        isDarkMode={isDarkMode}
        isExpanded={isMapExpanded}
        onCitySelect={setPrimaryCity}  // Clicking pin selects city
      />

      {/* Mini Map - always visible in corner */}
      <MiniMap
        cities={savedCities}  // ← Same database cities
        primaryCityId={primaryCityId}
        isDarkMode={isDarkMode}
        onClick={toggleMapView}
      />

      {/* Map Toggle Button */}
      <MapToggleButton
        isExpanded={isMapExpanded}
        onClick={toggleMapView}
      />
    </>
  );
}
```

**Layout Changes:**
- Add wrapper div for main-clock with transition classes
- Add wrapper div for world-map with transition classes
- Position both absolutely for smooth transitions

---

#### Task 5.2: Synchronize Map with City Selection
**Description:** Keep map and city carousel in sync

**Acceptance Criteria:**
- [x] Clicking pin on map updates `primaryCityId`
- [x] Clicking city card updates primary pin on map
- [x] Adding new city adds pin to map instantly
- [x] Removing city removes pin from map instantly
- [x] Scrolling city carousel highlights corresponding pin
- [x] No race conditions or stale state

#### Task 5.2: Synchronize Map with City Selection
**Description:** Keep map and city carousel in sync using shared database state

**Acceptance Criteria:**
- [x] **Clicking pin on map calls `setPrimaryCity(cityId)` - updates shared state**
- [x] **Clicking city card updates primary - map pin reflects change via `primaryCityId` prop**
- [x] **Adding new city via search adds to `savedCities` - pin appears on map automatically**
- [x] **Removing city updates `savedCities` - pin disappears from map automatically**
- [x] Scrolling city carousel highlights corresponding pin
- [x] No race conditions or stale state
- [x] **Single source of truth: `useCities` hook state**

**Files Modified:**
- `client/src/App.tsx`
- `client/src/components/CityCarousel.tsx` (add scroll-to-view effect)

**State Synchronization Flow:**
```
User Action                    State Update              UI Updates
─────────────────────────────────────────────────────────────────────
Click map pin                → setPrimaryCity(id)     → CityCard highlights
                                                       → Clock shows new time
                                                       → MapPin scales up

Click city card              → setPrimaryCity(id)     → MapPin highlights
                                                       → Clock shows new time

Add city via search          → addCity(city)          → New MapPin renders
                              (updates savedCities)   → CityCard appears

Remove city                  → removeCity(id)         → MapPin disappears
                              (updates savedCities)   → CityCard removed
```

**Implementation Note:**
All map components receive `cities` and `primaryCityId` as props, ensuring automatic re-renders when database-backed state changes. Use `useRef` on CityCarousel to expose `scrollToCityCard(cityId)` method.

---

#### Task 5.3: Add Keyboard Navigation
**Description:** Accessibility for map view toggle

**Acceptance Criteria:**
- [x] `Escape` key collapses map view
- [x] `Tab` navigation works for pins
- [x] `Enter/Space` activates focused pin
- [x] Focus trap when map is expanded (optional)
- [x] Screen reader announces view changes

**Files Modified:**
- `client/src/hooks/useMapView.ts` (add keyboard listeners)
- `client/src/components/WorldMapView.tsx` (keyboard handlers)

---

### Phase 6: Theming & Polish
**Estimated Tasks: 3 | Priority: Medium**

#### Task 6.1: Implement Dark/Light Mode Theming
**Description:** Map styles adapt to theme

**Acceptance Criteria:**
- [x] Dark mode:
  - Map outline: `rgba(255, 255, 255, 0.12)`
  - Pin glow: soft purple/white
  - Tooltip background: `rgba(0, 0, 0, 0.7)`
- [x] Light mode:
  - Map outline: `rgba(0, 0, 0, 0.08)`
  - Pin glow: soft orange/gold
  - Tooltip background: `rgba(255, 255, 255, 0.9)`
- [x] Theme transition smooth (no flicker)
- [x] All text remains readable

**Files Modified:**
- `client/src/components/WorldMapView.css`
- `client/src/components/MiniMap.css`
- `client/src/components/MapPin.css`
- `client/src/components/MapPinTooltip.css`

**CSS Variables Strategy:**
```css
body[data-theme="dark"] {
  --map-outline-color: rgba(255, 255, 255, 0.12);
  --map-pin-color: rgba(200, 180, 255, 0.9);
}

body[data-theme="light"] {
  --map-outline-color: rgba(0, 0, 0, 0.08);
  --map-pin-color: rgba(255, 180, 100, 0.9);
}
```

---

#### Task 6.2: Optimize Performance
**Description:** Ensure smooth 60fps animations

**Acceptance Criteria:**
- [x] Memoize map projection calculations
- [x] Use `React.memo` for MapPin components
- [x] CSS transforms (not position/width/height)
- [x] `will-change` hints for animated elements
- [x] Lazy load TopoJSON data
- [x] Debounce tooltip hover events

**Files Modified:**
- All map component files

**Optimizations:**
```typescript
// Memoize expensive calculations
const projectionFn = useMemo(() => createProjection(), [mapDimensions]);

// Memo components
export const MapPin = React.memo(MapPinComponent);

// CSS performance hints
.main-clock, .world-map {
  will-change: transform;
}
```

---

#### Task 6.3: Add Loading & Error States
**Description:** Handle edge cases gracefully

**Acceptance Criteria:**
- [x] Show skeleton/placeholder while TopoJSON loads
- [x] Error message if map data fails to load
- [x] Fallback: mini-map shows simplified dots without actual map
- [x] Retry mechanism for failed loads
- [x] No console errors

**Files Modified:**
- `client/src/components/WorldMapView.tsx`
- `client/src/components/MiniMap.tsx`

---

### Phase 7: Testing & Refinement
**Estimated Tasks: 2 | Priority: Medium**

#### Task 7.1: Manual Testing Checklist
**Description:** Verify all acceptance criteria

**Test Scenarios:**
- [x] Toggle between clock and map views (smooth animation)
- [x] Click mini-map to expand
- [x] Click toggle button to expand/collapse
- [x] Hover over pins to see tooltips
- [x] Click pin to select city
- [x] Add new city while map is expanded
- [x] Remove city while map is expanded
- [x] Select city from carousel, see pin highlight
- [x] Toggle dark mode with map expanded
- [x] Resize window with map expanded
- [x] Test on mobile viewport
- [x] Keyboard navigation (Tab, Enter, Escape)
- [x] Test with 1 city, 5 cities, 10+ cities

---

#### Task 7.2: Responsive Design Adjustments
**Description:** Ensure mobile/tablet compatibility

**Acceptance Criteria:**
- [x] Mini-map scales down on mobile (100px × 60px)
- [x] Mini-map repositions to avoid overlap
- [x] Full map view uses full viewport on mobile
- [x] Pins large enough to tap (min 44px touch target)
- [x] Tooltip repositions for screen edges
- [x] Animation duration faster on mobile (300ms)

**Files Modified:**
- All component CSS files with `@media` queries

**Breakpoints:**
```css
@media (max-width: 900px) { /* Tablet */ }
@media (max-width: 600px) { /* Mobile */ }
```

---

## File Structure Summary

```
client/
├── public/
│   └── maps/
│       └── world-110m.json           [NEW] TopoJSON world map data
├── src/
│   ├── components/
│   │   ├── WorldMapView.tsx           [NEW] Full interactive map
│   │   ├── WorldMapView.css           [NEW]
│   │   ├── MiniMap.tsx                [NEW] Corner preview map
│   │   ├── MiniMap.css                [NEW]
│   │   ├── MapPin.tsx                 [NEW] City marker
│   │   ├── MapPin.css                 [NEW]
│   │   ├── MapPinTooltip.tsx          [NEW] Hover tooltip
│   │   ├── MapPinTooltip.css          [NEW]
│   │   ├── MapToggleButton.tsx        [NEW] View toggle
│   │   ├── MapToggleButton.css        [NEW]
│   │   ├── CityCarousel.tsx           [MODIFY] Add scroll-to method
│   │   └── ...existing components
│   ├── hooks/
│   │   ├── useMapView.ts              [NEW] View state & toggle logic
│   │   └── ...existing hooks
│   ├── utils/
│   │   ├── mapProjection.ts           [NEW] Coordinate transformations
│   │   ├── mapAnimations.ts           [NEW] Animation constants
│   │   └── ...existing utils
│   ├── App.tsx                        [MODIFY] Integrate map components
│   └── App.css                        [MODIFY] Add animation classes
```

---

## Dependencies Summary

### New Dependencies
```json
{
  "react-simple-maps": "^3.0.0",
  "d3-geo": "^3.1.0",
  "@types/d3-geo": "^3.1.0"
}
```

**Total Bundle Size Impact:** ~50KB gzipped

---

## Data Flow Architecture (Database → Map Pins)

### Complete Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│ PostgreSQL Database                                                 │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ Cities Table                                                  │   │
│  │ ─────────────────────────────────────────────────────────────│   │
│  │ id           | name      | country | latitude  | longitude  │   │
│  │ ────────────────────────────────────────────────────────────│   │
│  │ "city_001"   | "London"  | "UK"    | 51.5074   | -0.1278    │   │
│  │ "city_002"   | "Tokyo"   | "Japan" | 35.6762   | 139.6503   │   │
│  │ "city_003"   | "NYC"     | "USA"   | 40.7128   | -74.0060   │   │
│  └──────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
                              ↓
                    ┌─────────────────┐
                    │  Backend API    │
                    │  .NET/ASP.NET   │
                    └─────────────────┘
                              ↓
              GET /api/cities/search?query=london
                              ↓
                    ┌─────────────────┐
                    │  JSON Response  │
                    │  [{             │
                    │    id,          │
                    │    name,        │
                    │    latitude,  ← │ Used for map positioning
                    │    longitude  ← │ Used for map positioning
                    │  }]             │
                    └─────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────┐
│ React Frontend                                                      │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ useCities Hook (State Management)                            │  │
│  │ ────────────────────────────────────────────────────────────│  │
│  │ const [savedCities, setSavedCities] = useState<City[]>([]);  │  │
│  │                                                               │  │
│  │ savedCities = [                                               │  │
│  │   { id: "city_001", name: "London", lat: 51.5074, ... },     │  │
│  │   { id: "city_002", name: "Tokyo", lat: 35.6762, ... }       │  │
│  │ ]                                                             │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                              ↓                                      │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ App.tsx (Props Distribution)                                  │  │
│  │ ────────────────────────────────────────────────────────────│  │
│  │ <WorldMapView cities={savedCities} ... />                    │  │
│  │ <MiniMap cities={savedCities} ... />                         │  │
│  └──────────────────────────────────────────────────────────────┘  │
│           ↓                                      ↓                  │
│  ┌────────────────────┐              ┌────────────────────────┐    │
│  │ WorldMapView       │              │ MiniMap                │    │
│  │ ─────────────────  │              │ ────────────────────── │    │
│  │ {cities.map(city =>│              │ {cities.map(city =>    │    │
│  │   <MapPin          │              │   <circle              │    │
│  │     lat={city.lat} │              │     cx={project(       │    │
│  │     lng={city.lng} │              │       city.longitude)} │    │
│  │   />               │              │     cy={project(       │    │
│  │ )}                 │              │       city.latitude)}  │    │
│  └────────────────────┘              │   />                   │    │
│           ↓                          │ )}                     │    │
│  ┌────────────────────┐              └────────────────────────┘    │
│  │ MapPin Component   │                        ↓                   │
│  │ ─────────────────  │              ┌────────────────────────┐    │
│  │ <Marker            │              │ mapProjection.ts       │    │
│  │   coordinates={[   │              │ ────────────────────── │    │
│  │     city.longitude,│←─────────────│ latLngToMapCoords()    │    │
│  │     city.latitude  │              │                        │    │
│  │   ]}               │              │ Input: 51.5074, -0.1278│    │
│  │ >                  │              │ Output: {x: 512, y: 234│    │
│  │   <circle ... />   │              └────────────────────────┘    │
│  │ </Marker>          │                                            │
│  └────────────────────┘                                            │
│           ↓                                                        │
│  ┌────────────────────────────────────────────────────────────┐   │
│  │ SVG Map Rendering                                           │   │
│  │ ──────────────────────────────────────────────────────────│   │
│  │ <svg width="1200" height="800">                            │   │
│  │   <!-- World outline paths -->                             │   │
│  │   <circle cx="512" cy="234" r="8" /> ← London pin          │   │
│  │   <circle cx="870" cy="320" r="8" /> ← Tokyo pin           │   │
│  │   <circle cx="280" cy="310" r="8" /> ← NYC pin             │   │
│  │ </svg>                                                      │   │
│  └────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
```

### Key Takeaways

1. **No Additional API Calls:** Map uses existing city data already fetched for the clock view
2. **Single Source of Truth:** `useCities` hook manages all city state
3. **Automatic Synchronization:** Map re-renders when `savedCities` changes
4. **Efficient Rendering:** react-simple-maps handles projection calculations
5. **Real-time Updates:** Adding/removing cities instantly updates map pins

### State Change Examples

**Example 1: User adds new city**
```typescript
// 1. User searches for "Paris"
searchCities("Paris")
  ↓
// 2. API returns city data (including lat: 48.8566, lng: 2.3522)
  ↓
// 3. User clicks to add
addCity(parisCity)
  ↓
// 4. savedCities state updates
setSavedCities([...savedCities, parisCity])
  ↓
// 5. React re-renders map components with new cities array
  ↓
// 6. New MapPin renders at Paris coordinates
<MapPin latitude={48.8566} longitude={2.3522} />
```

**Example 2: User clicks map pin**
```typescript
// 1. User clicks pin for "Tokyo"
<MapPin onClick={() => onCitySelect("city_002")} />
  ↓
// 2. Calls setPrimaryCity("city_002")
  ↓
// 3. primaryCityId state updates
  ↓
// 4. Multiple components react:
//    - MapPin with id="city_002" gets isPrimary={true} prop
//    - CityCard with id="city_002" highlights
//    - MainClock shows Tokyo time
//    - Weather fetches for Tokyo coordinates
```

---

## Database Schema Reference

**IMPORTANT:** The map feature requires latitude and longitude data that **already exists** in the database schema (defined in US-001 from USER_STORIES.md).

### Existing Cities Table Schema

```sql
Cities
├── Id (Primary Key, Auto-generated)
├── Name (string, required)
├── Country (string, required)
├── CountryCode (string, 2 chars)
├── Latitude (double, required)        ← ✅ Used for map Y-coordinate
├── Longitude (double, required)       ← ✅ Used for map X-coordinate
├── UtcOffsetSeconds (integer)
└── Population (integer, nullable)
```

**Backend API Response (from /api/cities/search):**
```json
{
  "id": "city_123",
  "name": "London",
  "country": "United Kingdom",
  "countryCode": "GB",
  "latitude": 51.5074,           // ← Map uses this
  "longitude": -0.1278,          // ← Map uses this
  "utcOffsetSeconds": 0,
  "utcOffsetDisplay": "UTC+00:00"
}
```

**Frontend TypeScript Interface (already exists):**
```typescript
// client/src/types/index.ts
export interface City {
  id: string;
  name: string;
  country: string;
  countryCode: string;
  latitude: number;    // ← Map uses this
  longitude: number;   // ← Map uses this
  utcOffsetSeconds: number;
  utcOffsetDisplay: string;
}
```

### Validation & Edge Cases

- **Valid Range:** Latitude: -90 to 90, Longitude: -180 to 180
- **Backend Validation:** Already enforced in seed data and API
- **Frontend Handling:** MapPin component should gracefully handle edge cases
- **Fallback:** If coordinates invalid, log warning and skip pin rendering

---

## Design Specifications

### Color Palette

#### Dark Mode
- Map outline: `rgba(255, 255, 255, 0.12)`
- Pin primary: `#b794f6` (purple glow)
- Pin secondary: `rgba(200, 180, 255, 0.6)`
- Pin pulse shadow: `rgba(200, 180, 255, 0.5)`
- Tooltip background: `rgba(20, 20, 30, 0.85)`
- Tooltip border: `rgba(255, 255, 255, 0.1)`

#### Light Mode
- Map outline: `rgba(0, 0, 0, 0.08)`
- Pin primary: `#ff9f5a` (warm orange)
- Pin secondary: `rgba(255, 159, 90, 0.6)`
- Pin pulse shadow: `rgba(255, 159, 90, 0.4)`
- Tooltip background: `rgba(255, 255, 255, 0.9)`
- Tooltip border: `rgba(0, 0, 0, 0.08)`

### Animation Timing
- **View transition:** 400ms `cubic-bezier(0.34, 1.56, 0.64, 1)` (spring easing)
- **Pin pulse:** 2s `ease-in-out` (infinite loop)
- **Tooltip fade:** 200ms `ease-out`
- **Hover scale:** 150ms `ease-out`

### Sizing
- **Mini-map:** 150px × 80px (desktop), 100px × 60px (mobile)
- **Full map:** 100% viewport width/height
- **Clock (minimized):** 25% original scale
- **Pin (full map):** 8px diameter (14px when primary)
- **Pin (mini-map):** 4px diameter (6px when primary)

---

## Implementation Order

### Sprint 1: Foundation (Tasks 1.1 - 2.2)
Focus: Dependencies, hooks, utilities
Duration: 1 day

### Sprint 2: Components (Tasks 3.1 - 3.5)
Focus: Build map components
Duration: 2-3 days

### Sprint 3: Animation (Tasks 4.1 - 4.2)
Focus: Smooth transitions
Duration: 1 day

### Sprint 4: Integration (Tasks 5.1 - 5.3)
Focus: Wire up App.tsx
Duration: 1 day

### Sprint 5: Polish (Tasks 6.1 - 6.3)
Focus: Theming, performance, errors
Duration: 1-2 days

### Sprint 6: Testing (Tasks 7.1 - 7.2)
Focus: QA and responsive design
Duration: 1 day

**Total Estimated Time:** 7-10 days

---

## Risk Assessment

### Technical Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Performance issues with many cities | Medium | Medium | Limit to 20 cities max, use React.memo |
| Animation jank on low-end devices | Medium | High | Reduce animation complexity, add prefers-reduced-motion |
| Map projection inaccuracies | Low | Medium | Use standard Mercator, test edge cases |
| Bundle size increase | Low | Low | Tree-shake unused d3 modules |
| Browser compatibility (backdrop-filter) | Low | Low | Provide fallback styles for older browsers |

### UX Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Users don't discover map toggle | Medium | Medium | Add subtle animation/hint on first load |
| Animation feels too slow/fast | Medium | Low | Make duration configurable, user testing |
| Mini-map blocks content | Low | Medium | Allow drag to reposition (future enhancement) |
| Pins overlap on close cities | Medium | Medium | Implement collision detection (Phase 2 enhancement) |

---

## Future Enhancements (Out of Scope)

- [ ] Zoom/pan on map
- [ ] Draggable mini-map position
- [ ] Pin clustering for overlapping cities
- [ ] 3D globe view option
- [ ] Time zone visualization (gradient overlay)
- [ ] City search from map click
- [ ] Animated flight paths between cities
- [ ] Weather overlay on map
- [ ] Night/day shadow on map

---

## Success Metrics

- [ ] Animation runs at consistent 60fps on desktop
- [ ] View toggle completes in under 500ms
- [ ] No layout shift (CLS = 0)
- [ ] Bundle size increase < 100KB
- [ ] Works on Chrome, Firefox, Safari, Edge (latest versions)
- [ ] Mobile-responsive (tested on iOS Safari, Chrome Android)
- [ ] Lighthouse Performance score > 90
- [ ] Zero console errors/warnings

---

## Notes & Considerations

1. **Why react-simple-maps?**
   - Lightweight (vs. Mapbox/Leaflet)
   - SVG-based (perfect for outlines)
   - React-first API
   - No API keys required
   - Excellent TypeScript support

2. **Why Mercator projection?**
   - Most familiar to users
   - Works well for world maps
   - Standard in web mapping
   - Supported by d3-geo

3. **Why CSS transforms for animation?**
   - GPU-accelerated (60fps)
   - No layout reflows
   - Smoother than JS animation
   - Better mobile performance

4. **Why absolute positioning?**
   - Allows overlapping elements during transition
   - No layout shift
   - Easier to control z-index layering

5. **Accessibility considerations:**
   - ARIA labels on all interactive elements
   - Keyboard navigation support
   - Screen reader announcements for view changes
   - Focus management during transitions
   - Reduced motion preference respected

---

## Developer Checklist

Before starting implementation:
- [x] Review existing codebase structure
- [x] Understand current state management pattern
- [x] Identify integration points in App.tsx
- [ ] Set up react-simple-maps examples locally
- [ ] Test TopoJSON data rendering
- [ ] Prototype animation in CodePen/CodeSandbox

During implementation:
- [ ] Commit after each task completion
- [ ] Test in both dark/light modes
- [ ] Verify responsive breakpoints
- [ ] Check browser DevTools performance tab
- [ ] Run ESLint and fix warnings
- [ ] Update TypeScript types

After implementation:
- [ ] Full regression test (existing features still work)
- [ ] Cross-browser testing
- [ ] Mobile device testing
- [ ] Lighthouse audit
- [ ] Update USER_STORIES.md with completion status
- [ ] Document any deviations from plan

---

## Contact & Questions

For questions or clarifications during implementation:
- Review react-simple-maps docs: https://www.react-simple-maps.io/
- Check d3-geo projections: https://github.com/d3/d3-geo
- Reference existing animation patterns in GradientBackground component

---

**Document Version:** 1.0  
**Last Updated:** 2026-01-29  
**Status:** Ready for Implementation
