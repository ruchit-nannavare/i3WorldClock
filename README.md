# TimeSpot 🌍⏰

A real-time world clock application with interactive map visualization. Track time across global cities with visual day/night indicators and geographic context.

## What It Does

TimeSpot provides an intuitive interface to monitor time across different time zones:

- **Real-Time Clock Display** - View current time for any selected location with automatic updates
- **City Discovery** - Search and add cities from a comprehensive global database (100+ major cities)
- **Interactive World Map** - Toggle between list view and geographic map visualization
- **Visual Day/Night Zones** - Dynamic shading shows daylight/darkness across the globe
- **Time Format Options** - Switch between 12-hour and 24-hour formats
- **Persistent Storage** - Your saved cities remain available across sessions

## Technology Stack

### Backend (.NET 9.0)
- **Architecture**: Clean Architecture with CQRS pattern
- **Framework**: ASP.NET Core Web API
- **Database**: PostgreSQL with Entity Framework Core
- **Testing**: xUnit with Moq for unit tests
- **Key Features**:
  - City autocomplete with fuzzy search
  - Real-time timezone calculations
  - Weather integration (OpenWeatherMap API)
  - CORS-enabled RESTful endpoints

### Frontend (React 19)
- **Build Tool**: Vite
- **Language**: TypeScript
- **UI Libraries**: 
  - React Simple Maps (SVG-based map rendering)
  - D3-Geo (geographic projections)
  - React Icons
- **State Management**: React Hooks (useState, useEffect, custom hooks)
- **Styling**: CSS Modules with responsive design

## Project Structure

```
TimeSpot/
├── src/
│   ├── TimeSpot.Core/              # Domain entities and interfaces
│   │   ├── Entities/               # Core business entities (City, Weather)
│   │   └── Interfaces/             # Service contracts
│   │
│   ├── TimeSpot.UseCases/          # Application business logic
│   │   ├── DTOs/                   # Data transfer objects
│   │   └── Interfaces/             # Use case abstractions
│   │
│   ├── TimeSpot.Infrastructure/    # External services and data access
│   │   ├── Data/                   # EF Core DbContext, migrations
│   │   ├── Services/               # City search, weather API
│   │   └── Migrations/             # Database schema versions
│   │
│   └── TimeSpot.Server/            # API presentation layer
│       ├── Controllers/            # REST endpoints
│       └── Program.cs              # DI configuration, middleware
│
├── client/
│   ├── src/
│   │   ├── components/             # React components
│   │   │   ├── CityCard.tsx        # City time display card
│   │   │   ├── SearchBar.tsx       # City search with autocomplete
│   │   │   ├── WorldMapView.tsx    # Interactive SVG map
│   │   │   ├── MapPin.tsx          # City location markers
│   │   │   └── MapToggleButton.tsx # View switcher
│   │   │
│   │   ├── hooks/                  # Custom React hooks
│   │   │   ├── useCities.ts        # City search/management state
│   │   │   ├── useMapView.ts       # Map interaction logic
│   │   │   ├── useTime.ts          # Time API integration
│   │   │   └── useWeather.ts       # Weather data fetching
│   │   │
│   │   └── utils/                  # Helper functions
│   │       ├── mapProjection.ts    # Geographic coordinate math
│   │       └── mapAnimations.ts    # View transition animations
│   │
│   └── public/maps/                # TopoJSON map data
│
└── tests/
    ├── TimeSpot.UseCases.Tests/
    ├── TimeSpot.Infrastructure.Tests/
    └── TimeSpot.Server.Tests/
```

## Getting Started

### Prerequisites
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **PostgreSQL** - [Download](https://www.postgresql.org/download/)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **OpenWeatherMap API Key** - [Get Free Key](https://openweathermap.org/api)

### Database Setup

1. Create PostgreSQL database:
```sql
CREATE DATABASE timespot;
```

2. Update connection string in `src/TimeSpot.Server/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=timespot;Username=postgres;Password=yourpassword"
  }
}
```

3. Apply migrations:
```bash
cd src/TimeSpot.Server
dotnet ef database update
```

### Running the Server

```bash
cd src/TimeSpot.Server
dotnet restore
dotnet run
```

Server will start at: **http://localhost:5000**

API endpoints:
- `GET /api/cities/search?query={text}` - Search cities
- `GET /api/time?city={name}&country={code}` - Get current time
- `GET /api/weather?lat={lat}&lon={lon}` - Get weather data

### Running the Client

```bash
cd client
npm install
npm run dev
```

Client will start at: **http://localhost:5173**

### Running Tests

```bash
# Backend tests
dotnet test

# Frontend tests (if added)
cd client
npm test
```

## Workshop Project Status

**This is an active workshop/demo project for i3 Verticals employees**, showcasing AI-assisted development workflows and modern full-stack architecture patterns.

### ✅ Completed Features

- [x] Clean Architecture backend with PostgreSQL
- [x] City search autocomplete with debouncing
- [x] Real-time timezone calculations
- [x] Interactive world map with SVG rendering
- [x] Day/night zone visualization
- [x] City pin markers with tooltips
- [x] Responsive UI with view toggle
- [x] City search bug fix (UI blinking resolved)
- [x] Unit test coverage for critical paths
- [x] CORS configuration for local development

### 🔧 Current Focus

- **Map Feature Refinements**
  - Improve pin clustering for closely located cities
  - Add zoom and pan controls
  - Enhance mobile responsiveness
  - Implement smooth animations for pin interactions

- **Performance Optimization**
  - Debounce search queries (currently 300ms)
  - Optimize map rendering for large city counts
  - Implement lazy loading for city data

### 🚀 Next Session Goals

- [ ] Add user authentication
- [ ] Implement city favorites/collections
- [ ] Add time zone converter tool
- [ ] Enhance weather display with forecasts
- [ ] Deploy to production environment

## Development Notes

### Known Issues & Resolutions

**Issue**: Search dropdown "blinking" during typing  
**Cause**: Loading state set after debounce delay  
**Fix**: Set `isSearching = true` immediately before debounce timer

**Issue**: PostgreSQL LINQ translation error in OrderBy  
**Cause**: `.ToLower().StartsWith()` doesn't translate to SQL  
**Fix**: Use `EF.Functions.ILike()` for case-insensitive matching

### Architecture Decisions

- **Clean Architecture**: Separates domain logic from infrastructure concerns
- **CQRS Pattern**: Clear separation between read and write operations
- **Custom Hooks**: Encapsulates state management and side effects
- **SVG Maps**: Better performance and styling control vs. raster images
- **Debounced Search**: Reduces API calls and improves UX

## Contributing

This is a workshop project for internal i3 Verticals training. If you're participating:

1. Clone the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Follow existing code patterns and architecture
4. Add tests for new functionality
5. Submit a pull request with clear description

## License

Internal workshop project - i3 Verticals © 2026

---

*Built with ☕, .NET, React, and AI collaboration*  
*Workshop facilitator: [Your Name]* | *Session Date: January 2026*
