import { useEffect, useMemo } from 'react';
import { useTime } from './hooks/useTime';
import { useWeather } from './hooks/useWeather';
import { useCities } from './hooks/useCities';
import { useLocalStorage } from './hooks/useLocalStorage';
import { GradientBackground } from './components/GradientBackground';
import { MainClock } from './components/MainClock';
import { TimeFormatToggle } from './components/TimeFormatToggle';
import { DarkModeToggle } from './components/DarkModeToggle';
import { CurrentLocation } from './components/CurrentLocation';
import { SunriseSunset } from './components/SunriseSunset';
import { WeatherDisplay } from './components/WeatherDisplay';
import { SearchBar } from './components/SearchBar';
import { CityCarousel } from './components/CityCarousel';
import { getGradient } from './utils/gradientMapper';
import { getLocalTime } from './utils/timeCalculations';
import './App.css';

function App() {
  const { utcTime } = useTime();
  const { weather, isLoading: isWeatherLoading, fetchWeather } = useWeather();
  const {
    savedCities,
    primaryCityId,
    searchResults,
    isSearching,
    searchCities,
    addCity,
    removeCity,
    setPrimaryCity,
    clearSearch,
  } = useCities();
  const [is24Hour, setIs24Hour] = useLocalStorage('i3worldclock-24hour', true);
  const [isDarkMode, setIsDarkMode] = useLocalStorage('i3worldclock-darkmode', false);

  // Update body data-theme attribute
  useEffect(() => {
    document.body.setAttribute('data-theme', isDarkMode ? 'dark' : 'light');
  }, [isDarkMode]);

  // Get the primary city
  const primaryCity = useMemo(() => {
    return savedCities.find((c) => c.id === primaryCityId) || savedCities[0];
  }, [savedCities, primaryCityId]);

  // Fetch weather when primary city changes
  // Using primitive values (id, lat, lon) as dependencies for reliable triggering
  useEffect(() => {
    if (primaryCity) {
      fetchWeather(primaryCity.latitude, primaryCity.longitude);
    }
  }, [primaryCity?.id, primaryCity?.latitude, primaryCity?.longitude, fetchWeather]);

  // Calculate gradient based on weather
  const gradient = useMemo(() => {
    if (weather) {
      return getGradient(weather.condition, weather.dayNight, weather.temperature, isDarkMode);
    }
    // Default gradient
    if (isDarkMode) {
      return {
        background: 'linear-gradient(180deg, #0a0a0f 0%, #121218 50%, #1a1a24 100%)',
        textColor: '#ffffff',
      };
    }
    return {
      background: 'linear-gradient(180deg, #87CEEB 0%, #FFD700 50%, #FFA500 100%)',
      textColor: '#000000',
    };
  }, [weather, isDarkMode]);

  // Format date
  const formattedDate = useMemo(() => {
    if (!primaryCity) return '';
    const localTime = getLocalTime(utcTime, primaryCity.utcOffsetSeconds);
    return localTime.toLocaleDateString('en-US', {
      weekday: 'long',
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  }, [utcTime, primaryCity]);

  return (
    <GradientBackground gradient={gradient} isDarkMode={isDarkMode}>
      <div className="app">
        <header className="app-header">
          <div className="header-logo">
            <span className="logo-icon">&#9788;</span>
            <span className="logo-text">i3WorldClock</span>
          </div>
          <div className="header-center">
            <SearchBar
              searchResults={searchResults}
              isSearching={isSearching}
              onSearch={searchCities}
              onAddCity={addCity}
              onClear={clearSearch}
              savedCityIds={savedCities.map((c) => c.id)}
            />
          </div>
          <div className="header-actions">
            <DarkModeToggle isDark={isDarkMode} onChange={setIsDarkMode} />
            <button className="header-btn">Log In</button>
            <button className="header-btn header-btn-primary">Get the App</button>
          </div>
        </header>

        <main className="app-main">
          {primaryCity && (
            <>
              <div className="clock-row">
                <MainClock
                  utcTime={utcTime}
                  utcOffsetSeconds={primaryCity.utcOffsetSeconds}
                  is24Hour={is24Hour}
                />
              </div>
              <div className="controls-row">
                <TimeFormatToggle is24Hour={is24Hour} onChange={setIs24Hour} />
              </div>
            </>
          )}
        </main>

        <footer className="app-footer">
          <div className="footer-content">
            <div className="footer-left">
              <div className="weather-info-group">
                {isWeatherLoading ? (
                  <div className="weather-loading">Loading weather...</div>
                ) : weather ? (
                  <>
                    <WeatherDisplay weather={weather} />
                    <SunriseSunset sunrise={weather.sunrise} sunset={weather.sunset} />
                    <div className="date-display">{formattedDate}</div>
                  </>
                ) : null}
              </div>
              <div className="footer-location-row">
                <div className="footer-location-left">
                  <span className="footer-label">Current</span>
                  {primaryCity && (
                    <CurrentLocation
                      name={primaryCity.name}
                      country={primaryCity.country}
                    />
                  )}
                </div>
                <div className="footer-location-right">
                  <button className="add-city-btn" onClick={() => document.querySelector<HTMLInputElement>('.search-bar-input')?.focus()}>
                    Add Another City <span className="add-icon">+</span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </footer>

        <section className="city-section">
          <CityCarousel
            cities={savedCities}
            utcTime={utcTime}
            is24Hour={is24Hour}
            primaryCityId={primaryCityId}
            onSelectCity={setPrimaryCity}
            onRemoveCity={removeCity}
          />
        </section>
      </div>
    </GradientBackground>
  );
}

export default App;
