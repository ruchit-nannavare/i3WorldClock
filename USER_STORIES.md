# i3WorldClock - User Stories & Development Tasks

## Overview
This document outlines the user stories for the i3WorldClock application, divided into Backend (4 stories) and Frontend (2 stories) development tracks.

---

## Backend User Stories

### US-001: City Search API
**As a** user
**I want to** search for cities by name
**So that I** can find and add cities to track their local time

#### Tasks

##### Task 1.1: Create City Entity and Database Schema
**Given** the application needs to store city data
**When** a developer sets up the database
**Then** a `Cities` table should be created with columns:
- `Id` (Primary Key, Auto-generated)
- `Name` (string, required, max 100 chars)
- `Country` (string, required, max 100 chars)
- `CountryCode` (string, required, 2 chars)
- `Latitude` (double, required)
- `Longitude` (double, required)
- `UtcOffsetSeconds` (integer, required)
- `Population` (integer, nullable, for sorting relevance)

**Acceptance Criteria:**
- Entity Framework Core migrations create the table successfully
- Database supports PostgreSQL
- Indexes exist on `Name` column for search performance

##### Task 1.2: Implement City Seed Service
**Given** the database is empty on first run
**When** the application starts
**Then** the database should be seeded with major world cities

**Acceptance Criteria:**
- Minimum 500 cities seeded from reliable data source
- Seed only runs when cities table is empty
- UTC offsets are accurate for each city's timezone

##### Task 1.3: Implement City Search Service
**Given** a user wants to search for cities
**When** they enter a search query with 2+ characters
**Then** the service should return matching cities ordered by relevance

**Acceptance Criteria:**
- Search is case-insensitive
- Results limited to top 10 matches
- Cities with higher population ranked first
- Search matches beginning of city name (prefix search)

##### Task 1.4: Create Cities API Controller
**Given** the frontend needs to search for cities
**When** a GET request is made to `/api/cities/search?query={searchTerm}`
**Then** the API should return a JSON array of matching cities

**Acceptance Criteria:**
- Returns 200 OK with array of `CityDto` objects
- Returns empty array if no matches found
- Returns 400 Bad Request if query is less than 2 characters
- Response includes: id, name, country, countryCode, latitude, longitude, utcOffsetSeconds, utcOffsetDisplay

---

### US-002: Weather Data API
**As a** user
**I want to** see current weather for my selected city
**So that I** can know the weather conditions at that location

#### Tasks

##### Task 2.1: Define Weather Domain Models
**Given** the application needs to represent weather data
**When** weather information is processed
**Then** the following models should be available:
- `WeatherCondition` enum: Clear, PartlyCloudy, Cloudy, Rain, Snow, Thunderstorm, Fog
- `DayNightStatus` enum: Day, Night
- `WeatherDto` record: Temperature, Condition, DayNight, Sunrise, Sunset

**Acceptance Criteria:**
- Enums are defined in Core layer
- DTOs are defined in UseCases layer
- Models support JSON serialization

##### Task 2.2: Implement Open-Meteo Weather Service
**Given** the application needs real weather data
**When** weather is requested for coordinates
**Then** the service should fetch data from Open-Meteo API

**Acceptance Criteria:**
- Uses Open-Meteo free API (no API key required)
- Fetches current temperature in Celsius
- Maps Open-Meteo weather codes to `WeatherCondition` enum
- Extracts sunrise/sunset times
- Determines day/night status based on current time vs sunrise/sunset
- Handles API errors gracefully with appropriate exceptions

##### Task 2.3: Create Weather API Controller
**Given** the frontend needs weather data
**When** a GET request is made to `/api/weather?lat={latitude}&lon={longitude}`
**Then** the API should return current weather for those coordinates

**Acceptance Criteria:**
- Returns 200 OK with `WeatherDto` object
- Returns 400 Bad Request for invalid coordinates
- Returns 503 Service Unavailable if Open-Meteo is unreachable
- Temperature rounded to 1 decimal place
- Sunrise/sunset in ISO 8601 format

---

### US-003: UTC Time Synchronization API
**As a** user
**I want to** see accurate time synchronized with the server
**So that I** can trust the displayed times are correct regardless of my device's clock

#### Tasks

##### Task 3.1: Implement Time Service
**Given** the application needs a reliable time source
**When** time is requested
**Then** the service should provide current UTC time

**Acceptance Criteria:**
- Returns server's current UTC time
- Time is in ISO 8601 format
- Service is registered as singleton for consistency

##### Task 3.2: Create Time API Controller
**Given** the frontend needs to synchronize time
**When** a GET request is made to `/api/time/utc`
**Then** the API should return the current UTC time

**Acceptance Criteria:**
- Returns 200 OK with `UtcTimeDto` object
- Response includes `utcTime` field in ISO 8601 format
- Endpoint is lightweight with minimal processing
- No caching headers (always fresh time)

---

### US-004: Application Infrastructure & Configuration
**As a** developer
**I want to** have a well-structured backend architecture
**So that** the codebase is maintainable and follows best practices

#### Tasks

##### Task 4.1: Configure Clean Architecture Project Structure
**Given** the backend needs proper separation of concerns
**When** the solution is structured
**Then** it should follow Clean Architecture pattern

**Acceptance Criteria:**
- `TimeSpot.Core` - Domain entities and enums (no dependencies)
- `TimeSpot.UseCases` - Business logic, interfaces, DTOs (depends on Core)
- `TimeSpot.Infrastructure` - Data access, external services (depends on UseCases)
- `TimeSpot.Server` - API controllers, DI configuration (depends on all)

##### Task 4.2: Configure Dependency Injection
**Given** services need to be injected into controllers
**When** the application starts
**Then** all dependencies should be properly registered

**Acceptance Criteria:**
- `IWeatherService` registered with `HttpClient` for API calls
- `ICitySearchService` registered as scoped
- `TimeService` registered as singleton
- `WorldTimeDbContext` registered with PostgreSQL connection
- CORS configured to allow frontend origin

##### Task 4.3: Configure Database Connection
**Given** the application needs persistent storage
**When** the application connects to the database
**Then** it should use PostgreSQL with Entity Framework Core

**Acceptance Criteria:**
- Connection string stored in user secrets (development)
- Connection string from environment variable (production)
- Automatic migrations on startup (development only)
- Database seeding runs after migrations

##### Task 4.4: Configure API Error Handling
**Given** API calls may fail
**When** an error occurs
**Then** appropriate HTTP status codes and messages should be returned

**Acceptance Criteria:**
- 400 Bad Request for validation errors
- 404 Not Found for missing resources
- 500 Internal Server Error for unhandled exceptions
- Error responses include meaningful messages
- No stack traces exposed in production

---

## Frontend User Stories

### US-005: World Clock Display & City Management
**As a** user
**I want to** view the current time for multiple cities and manage my city list
**So that I** can track time across different locations important to me

#### Tasks

##### Task 5.1: Implement Main Clock Component
**Given** a user has selected a primary city
**When** the main clock is displayed
**Then** it should show the local time for that city

**Acceptance Criteria:**
- Large, readable digital clock display
- Shows hours, minutes, seconds
- Supports both 12-hour (AM/PM) and 24-hour format
- Updates every second smoothly
- Time calculated from UTC + city's offset

##### Task 5.2: Implement Time Synchronization Hook
**Given** the application needs accurate time
**When** the app initializes
**Then** it should sync with the server's UTC time

**Acceptance Criteria:**
- Fetches UTC time from `/api/time/utc` on mount
- Calculates offset between server time and client time
- Uses offset to maintain accuracy without constant API calls
- Falls back to client time if API unavailable
- Updates internal clock every 1000ms

##### Task 5.3: Implement City Management Hook
**Given** users want to save their preferred cities
**When** they add or remove cities
**Then** the changes should persist across sessions

**Acceptance Criteria:**
- Cities stored in localStorage
- Primary city tracked separately
- Default city (London) if no cities saved
- Prevents duplicate cities
- Auto-selects new primary when current is removed

##### Task 5.4: Implement City Search Component
**Given** a user wants to add a new city
**When** they type in the search bar
**Then** matching cities should appear in a dropdown

**Acceptance Criteria:**
- Debounced search (300ms delay)
- Minimum 2 characters to trigger search
- Shows city name and country in results
- Indicates if city already saved (checkmark vs plus icon)
- Clicking result adds city and selects it as primary
- Dropdown closes on click outside

##### Task 5.5: Implement City Carousel Component
**Given** a user has multiple saved cities
**When** they view the city section
**Then** they should see cards for each saved city

**Acceptance Criteria:**
- Horizontally scrollable list of city cards
- Each card shows: city name, UTC offset, current local time, day/night indicator
- Primary city visually highlighted
- Clicking card sets it as primary city
- Remove button (X) to delete city from list
- Responsive layout adapts to screen size

##### Task 5.6: Implement Time Format Toggle
**Given** users have different time format preferences
**When** they toggle the time format
**Then** all times should switch between 12h and 24h format

**Acceptance Criteria:**
- Toggle button shows current format (12h/24h)
- Preference persisted in localStorage
- Applies to main clock and all city cards
- Default to 24-hour format

---

### US-006: Weather Display & Dynamic Theming
**As a** user
**I want to** see weather information and have the UI reflect the current conditions
**So that I** get a visually immersive experience of the selected city

#### Tasks

##### Task 6.1: Implement Weather Hook
**Given** a city is selected
**When** the primary city changes
**Then** weather data should be fetched for that city

**Acceptance Criteria:**
- Fetches from `/api/weather?lat={lat}&lon={lon}`
- Caches results for 5 minutes per location
- Handles race conditions when rapidly switching cities
- Exposes loading and error states
- Only updates state if request is still current

##### Task 6.2: Implement Weather Display Component
**Given** weather data is available
**When** displayed in the UI
**Then** it should show temperature and conditions

**Acceptance Criteria:**
- Shows temperature in Celsius with degree symbol
- Weather condition icon (sun, cloud, rain, etc.)
- Condition text (Clear, Cloudy, Rain, etc.)
- Uses react-icons/wi for weather icons
- Gracefully handles missing data

##### Task 6.3: Implement Sunrise/Sunset Component
**Given** weather data includes sun times
**When** displayed in the UI
**Then** it should show sunrise and sunset times with daylight duration

**Acceptance Criteria:**
- Sun icon for sunrise, moon icon for sunset
- Times formatted according to user's 12h/24h preference
- Calculates and displays daylight duration (e.g., "12h 07m")
- Responsive layout (horizontal on wide screens)

##### Task 6.4: Implement Dynamic Gradient Background
**Given** weather affects the visual experience
**When** weather data is loaded
**Then** the background gradient should reflect conditions

**Acceptance Criteria:**
- Different gradients for: Clear, Cloudy, Rain, Snow, Thunderstorm, Fog
- Day vs Night variations for each condition
- Temperature influences gradient warmth (warmer colors for hot, cooler for cold)
- Dark mode overrides with subtle, dark gradients
- Smooth transition between gradients (CSS transition)
- Text color adapts for readability

##### Task 6.5: Implement Dark Mode Toggle
**Given** users may prefer dark mode
**When** they toggle dark mode
**Then** the entire UI should switch to dark theme

**Acceptance Criteria:**
- Moon/sun icon toggle in header
- Preference persisted in localStorage
- Sets `data-theme` attribute on body
- All components respect dark mode styling
- Gradient background adapts to dark mode

##### Task 6.6: Integrate Weather with City Selection
**Given** the user clicks a city or adds a new one
**When** the primary city changes
**Then** weather should update immediately

**Acceptance Criteria:**
- Weather fetches when `primaryCity.id` changes
- Shows "Loading weather..." during fetch
- New cities automatically become primary (triggers weather fetch)
- Cache serves instant data for recently viewed cities
- Background gradient updates with new weather

---

## Development Phases

### Phase 1: Backend Foundation
- US-004: Application Infrastructure & Configuration
- US-003: UTC Time Synchronization API

### Phase 2: Backend Features
- US-001: City Search API
- US-002: Weather Data API

### Phase 3: Frontend Core
- US-005: World Clock Display & City Management

### Phase 4: Frontend Enhancement
- US-006: Weather Display & Dynamic Theming

---

## Technical Stack

### Backend
- .NET 10 / ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Open-Meteo API (Weather)

### Frontend
- React 18 with TypeScript
- Vite (Build tool)
- Styled Components
- React Icons
- LocalStorage for persistence

---

## API Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/time/utc` | Get current UTC time |
| GET | `/api/cities/search?query={q}` | Search cities by name |
| GET | `/api/weather?lat={lat}&lon={lon}` | Get weather for coordinates |

---

## Definition of Done

- [ ] Code compiles without errors
- [ ] All acceptance criteria met
- [ ] Unit tests written and passing (where applicable)
- [ ] Code reviewed
- [ ] API endpoints documented
- [ ] No console errors in browser
- [ ] Responsive on mobile and desktop
